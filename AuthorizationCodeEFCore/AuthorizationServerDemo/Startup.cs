using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AuthorizationServerDemo.Data;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthorizationServerDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // 数据库连接字符串，方便演示就写在这了，项目中一般放在配置文件，这里用到的是localdb 
            string strConn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IdentityServer4DB;
                    Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;
                    ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            // 指定迁移的程序集，这里指定Startup类所在的程序为迁移程序集
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(strConn));
            services.AddIdentity<IdentityUser<int>, IdentityRole<int>>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer()
                // 配置ConfigurationDbContext上下文
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbBuilder =>
                    {
                        // 只需指定对应的数据库、连接字符串、迁移程序集即可
                        dbBuilder.UseSqlServer(strConn, t_builder => t_builder.MigrationsAssembly(migrationAssembly));
                    };
                })
                // 配置PersistedGrantDbContext上下文
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbBuilder =>
                    {
                        // 只需指定对应的数据库、连接字符串、迁移程序集即可
                        dbBuilder.UseSqlServer(strConn, t_tbuilder => t_tbuilder.MigrationsAssembly(migrationAssembly));
                    };
                })
                  // 用户暂时还从内存中取，后面单独说
                  //.AddTestUsers(Config.GetTestUsers());
                  .AddAspNetIdentity<IdentityUser<int>>();

            // 注入对应的IdentityServer相关组件， 然后加载模拟数据
            //var builder = services.AddIdentityServer()
            //     .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddInMemoryApiScopes(Config.GetApiScopes())
            //    .AddInMemoryClients(Config.GetClients())
            //    // 模拟备案用户，即资源拥有者
            //    .AddTestUsers(Config.GetTestUsers());
               



            // 指定Token签名和验证的秘钥方式， 开发测试使用临时秘钥保存在本地；
            // 生产用AddSigingCredential, 这里后续会说到
            builder.AddDeveloperSigningCredential();

            services.Configure<CookiePolicyOptions>(options =>
            {
                //https://docs.microsoft.com/zh-cn/aspnet/core/security/samesite?view=aspnetcore-3.1&viewFallbackFrom=aspnetcore-3
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;

            });
            services.ConfigureNonBreakingSameSiteCookies();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            InitDatabase(app);
            InitDatabaseUser(app);
            // 增加静态文件
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            // 开启中间件
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitDatabase(IApplicationBuilder app)
        {
            // 从根作用域中创建一个子作用域，这个在之前的依赖注入生命周期中有说到
            using(var scope = app.ApplicationServices.CreateScope())
            {
                // 每个子作用域中有对应的容器，可以取到对应的对象
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                // 这里取到配置数据上下文
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                // 先判断Clients表中有数据没，没有就将内存中的数据存进去
                if(!configurationDbContext.Clients.Any())
                {
                    // 遍历内存中配置的客户端数据，直接存进去即可
                    foreach(var client in Config.GetClients())
                    {
                        configurationDbContext.Clients.Add(client.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                // 存ApiScopes
                if (!configurationDbContext.ApiScopes.Any())
                {
                    foreach (var apiScope in Config.GetApiScopes())
                    {
                        configurationDbContext.ApiScopes.Add(apiScope.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                //存IdentityResources
                if (!configurationDbContext.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResources())
                    {
                        configurationDbContext.IdentityResources.Add(identity.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
            }
        }

        private void InitDatabaseUser(IApplicationBuilder app)
        {
            // 从根作用域中创建一个子作用域，这个在之前的依赖注入生命周期中有说到
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // 每个子作用域中有对应的容器，可以取到对应的对象
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                // 这里直接使用微软封装好的manager
                var userManager  = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<int>>>();
                IdentityUser<int> user = new IdentityUser<int>
                {
                    UserName = "Zoe",
                    Email = "Zoe@qq.com"
                };
                var u = userManager.FindByNameAsync(user.UserName).Result;
                if(u!=null)
                {
                    return;
                }
                // 默认情况下密码要求比较严，如果不符合规则就不能添加成功
                var res = userManager.CreateAsync(user, "Zoe123456&").Result;
                if(!res.Succeeded)
                {
                    throw new Exception("同步用户失败");
                }
                var claims = new List<Claim>{
                                    new Claim(JwtClaimTypes.Name, user.UserName),
                                    new Claim(JwtClaimTypes.Email, user.Email),
                                    new Claim(JwtClaimTypes.GivenName, "Zoe11"),
                                    new Claim(JwtClaimTypes.FamilyName, "ZZZ"),
                                    new Claim(JwtClaimTypes.Email, "Zoe@email.com"),
                                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                                    new Claim(JwtClaimTypes.WebSite, "https://www.cnblogs.com/zoe-zyq/")
                                };
                res=userManager.AddClaimsAsync(user, claims).Result;
                if (!res.Succeeded)
                {
                    throw new Exception("同步Claim失败");
                }
            }
        }
    }
}
