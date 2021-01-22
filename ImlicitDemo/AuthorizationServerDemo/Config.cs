using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        // 模拟备案用户
        public static List<TestUser> GetTestUsers()
        {
            // 模拟两个用户
            return new List<TestUser>
            {
                new TestUser
                {
                     SubjectId="1",
                     Username="Zoe", // 用户名
                     Password="123456",// 密码
                      Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Zoe"),
                            new Claim(JwtClaimTypes.GivenName, "Zoe11"),
                            new Claim(JwtClaimTypes.FamilyName, "ZZZ"),
                            new Claim(JwtClaimTypes.Email, "Zoe@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com")
                    }

                },
                new TestUser
                {
                     SubjectId="2",
                     Username="ZoeCode",// 用户名
                     Password="666999"// 用户名
                }
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
                },
                new Client
                {
                    // 客户端的唯一标识
                    ClientId="WinForm",
                    // 客户端认证密码
                    ClientSecrets=
                    {
                        new Secret("winformsecret".Sha256())
                    },
                   // 指定授权模式，这里指定为客户端凭据模式
                   AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                   // 如果要获取refreshtoken，将AllowOfflineAccess要设置为true
                   AllowOfflineAccess=true,
                   // 指定客户端获取的Token能访问到的作用域
                   AllowedScopes={ "orderApi" }

                }
            };
        }
    }
}
