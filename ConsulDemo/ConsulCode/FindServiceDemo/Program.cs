using Consul;
using System;

namespace FindServiceDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            FindServices();
            Console.ReadLine();
        }
        private static void FindServices()
        {
            ConsulClient consulClient = new ConsulClient(c =>
            {
                // 指定地址
                c.Address = new Uri("http://localhost:8500/");
                // 指定数据中心
                c.Datacenter = "dc1";
            });
            // 获取服务信息
            var result = consulClient.Agent.Services().Result.Response;
            // 遍历服务
            foreach (var service in result)
            {
                Console.WriteLine(service.Key);
                Console.WriteLine($"IP地址：{service.Value.Address}-端口:{service.Value.Port}");
                Console.WriteLine("===============================");
            }
            //接下来就可以根据业务调用对应服务的业务接口啦
        }
    }
}
