using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcClientDemo
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
     
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            // 去除映射，保留Jwt原有的Claim名称
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                // 使用Cookies 
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; //Cookies
                // 使用OpenID Connect 
                options.DefaultChallengeScheme = "oidc";//OpenIdConnectDefaults.AuthenticationScheme; //oidc
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)//Cookies
            .AddOpenIdConnect("oidc", options =>//oidc
            {
                options.SignInScheme = "Cookies";
                options.ClientId = "MvcClient";
                // 客户端密码
                options.ClientSecret = "codetest_secret";
                // 授权服务器地址
                options.Authority = "http://localhost:6100";
                // 返回授权码
                options.ResponseType = "code";
                // 不使用https
                options.RequireHttpsMetadata = false;
                // 允许Token保存的Cookies中
                options.SaveTokens = true;

                //需要的权限范围
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                // 允许获取刷新Token
                options.Scope.Add("offline_access");

            });

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    //https://docs.microsoft.com/zh-cn/aspnet/core/security/samesite?view=aspnetcore-3.1&viewFallbackFrom=aspnetcore-3
            //    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;

            //});
            //services.ConfigureNonBreakingSameSiteCookies();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
