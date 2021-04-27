using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServerDemo
{
    public class Config
    {
        // API权限范围作用域
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("orderApi","Order Api")
            };
        }
        // 模拟备案客户端，不是任意一个应用都能到认证授权中心获取Token
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                   // 客户端的唯一标识
                   ClientId="client", 
                   // 客户端认证密码
                   ClientSecrets =
                    {
                        new Secret("ordersecret".Sha256())
                    },
                   // 指定授权模式，这里指定为客户端凭据模式
                   AllowedGrantTypes = GrantTypes.ClientCredentials,
                   // 指定客户端获取的Token能访问到的作用域
                   AllowedScopes={ "orderApi" }
                }
            };
        }
    }
}
