using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsulCodeDemo
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
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            RegistService();
        }

        private void RegistService()
        {
            var configConsul = Configuration.GetSection("Consul");
            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri(configConsul["ConsulAddr"]);
                c.Datacenter = "dc1";
            });

            string serviceIp = configConsul["ServiceIP"];
            string servicePort = configConsul["ServicePort"];
            string serviceTags = configConsul["Tags"];
            client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                ID = "Code6688",
                Name = "Code6688Name",
                Address = serviceIp,
                Port = string.IsNullOrEmpty(servicePort) ? 0 : int.Parse(servicePort),
                Tags = serviceTags.Split(','),
                Check = new AgentServiceCheck
                {
                    HTTP=$"http://{serviceIp}:{servicePort}/api/Health",
                    Interval = TimeSpan.FromSeconds(5),
                    Timeout = TimeSpan.FromSeconds(10),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20)
                }
            });
        }
    }
}
