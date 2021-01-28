using IdentityModel;
using IdentityServer4;
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

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
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
                            new Claim(JwtClaimTypes.WebSite, "https://www.cnblogs.com/zoe-zyq/")
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

                },
                new Client
                {
                    // 客户端ID
                    ClientId="JsClient",
                    // 客户端名称
                    ClientName="JsTestClient",
                    // 授权模式为 Implicit
                    AllowedGrantTypes=GrantTypes.Implicit,
                    // 授权操作页面支持，为true表示显示授权界面，否则不显示
                    RequireConsent=true,
                    // 认证成功之后重定向到客户端的地址
                    RedirectUris={ "http://localhost:8080/callback.html"},
                    // 登出时重定向到客户端的地址
                    PostLogoutRedirectUris={"http://localhost:8080/index.html"},
                    // 由于跨域操作，所以设置允许跨域的站点地址，即客户端地址
                    AllowedCorsOrigins={"http://localhost:8080" ,"http://127.0.0.1:8080"},
                    // 允许浏览器传递AccessToken，如果不设置，如果通过浏览器操作就会报错
                    AllowAccessTokensViaBrowser=true,
                    // 允许返回刷新Token
                    AllowOfflineAccess=true,
                    // 指定客户端获取的Token能访问到的作用域
                    AllowedScopes={ 
                        "orderApi" ,// API 访问权限
                        IdentityServerConstants.StandardScopes.OpenId,// 身份信息权限
                        IdentityServerConstants.StandardScopes.Profile // 身份信息权限
                    }
                }
            };
        }
    }
}
