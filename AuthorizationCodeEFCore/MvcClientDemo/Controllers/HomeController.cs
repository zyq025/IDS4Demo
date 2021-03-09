using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MvcClientDemo.Models;

namespace MvcClientDemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UserInfo()
        {
            return View();
        }
        public async Task<IActionResult> CallAPI()
        {
            // 获取accessToken方式
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            // 获取idToken
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            // 获取刷新Token
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // 开始请求API
            var apiClient = new HttpClient();
            // 将获取到的AccessToken以Bearer的方案设置在请求头中
            apiClient.SetBearerToken(accessToken);
            // 向资源服务器中请求受保护的API
            var contentResp = await apiClient.GetAsync("http://localhost:5000/api/Order");
            if (contentResp.IsSuccessStatusCode)
            {
                var content = await contentResp.Content.ReadAsStringAsync();
                ViewData["ApiData"] = content;
            }
            else
            {
                ViewData["ApiData"] = "获取失败";
            }


            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
