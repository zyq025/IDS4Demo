using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using CacheManager.Core;
using Ocelot.Cache;
using Ocelot.Provider.Polly;
using IdentityServer4.AccessTokenValidation;
using Microsoft.IdentityModel.Tokens;

namespace OcelotCodeDemo
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
            string authenticationProviderKey = "TestGatewaykey";
            services.AddAuthentication("Bearer")
                   .AddJwtBearer(authenticationProviderKey, options =>
                   {
                       // 指定要接入的授权服务器地址
                       options.Authority = "http://localhost:6100";
                       // 在验证token时，不验证Audience
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateAudience = false
                       };
                       // 不适用Https
                       options.RequireHttpsMetadata = false;
                   });

            services.AddOcelot()
                .AddConsul()
                .AddPolly();
            // 注册相关服务
            //.AddCacheManager(x=> {
            //    // 配置Redis相关信息
            //    //x.WithRedisConfiguration("redis", config =>
            //    //{
            //    //    config.WithAllowAdmin() // 运行管理员相关操作
            //    //    .WithPassword("redispwd") // 如果Redis需要密码就配置密码
            //    //    //将数据保存在哪个数据库中，Redis默认有16个，这里指定索引位13的数据库
            //    //    .WithDatabase(13)
            //    //    .WithEndpoint("192.168.30.250",6379);//指定Redis的主机和端口
            //    //}).WithRedisCacheHandle("redis",true); // 指定配置
            //    //x.WithJsonSerializer();//指定数据序列化形式
            //    x.WithDictionaryHandle();//使用Dictonary方式处理
            //});
            //services.AddSingleton<IOcelotCache<CachedResponse>, MyCache>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseOcelot();
        }
    }
}
