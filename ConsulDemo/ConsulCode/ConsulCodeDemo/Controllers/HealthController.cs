using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;

namespace ConsulCodeDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost("Notice")]
        public IActionResult Notice()
        {
            var bytes = new byte[1024];
            // 读取请求体内数据，解析并发送
            var i = Request.Body.ReadAsync(bytes, 0, bytes.Length);
            string content = Encoding.UTF8.GetString(bytes).Trim('\0');
            // 发送邮件
            SendMail(content);
            return Ok();
        }

        private void SendMail(string content)
        {
            dynamic list = JsonConvert.DeserializeObject(content);
            if (list != null && list.Count > 0)
            {
                #region 构造消息内容
                StringBuilder sbMsg = new StringBuilder();
                sbMsg.AppendLine("服务发生故障,信息如下：");
                foreach (var msgItem in list)
                {
                    sbMsg.AppendLine($"NodeInfo：{msgItem.Node}");
                    sbMsg.AppendLine($"ServiceID：{msgItem.ServiceID}");
                    sbMsg.AppendLine($"ServiceName：{msgItem.ServiceName}");
                    sbMsg.AppendLine($"CheckName：{msgItem.Name}");
                    sbMsg.AppendLine($"CheckStatus：{msgItem.Status}");
                    sbMsg.AppendLine($"CheckOutput：{msgItem.Output}");
                }
                #endregion
                #region 构造邮件发送信息
                var mailMsg = new MimeMessage();
                // 发送方
                mailMsg.From.Add(new MailboxAddress("ZoeQQMail", "发送的邮箱地址"));
                // 接收方
                mailMsg.To.Add(new MailboxAddress("ZoeCompanyMail", "接收的邮箱地址"));
                //主题
                mailMsg.Subject = "服务故障报警";
                // 内容
                mailMsg.Body = new TextPart("plain")
                {
                    Text = sbMsg.ToString()
                };
                // 内容可以是html 
                ///var bodyBuilder = new BodyBuilder();
                //bodyBuilder.HtmlBody = @"<b>msgtext <i>italic</i></b>";
                //message.Body = bodyBuilder.ToMessageBody();
                #endregion
                using (var client = new SmtpClient())
                {
                    //允许所有SSL证书
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    //连接服务器
                    client.Connect("smtp.qq.com", 587, false);
                    //不需要OAuth2验证
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    // SMTP 服务器需要认证,输入邮箱地址和获取到的授权码即可
                    client.Authenticate("发送方的邮箱地址", "drcihukbidmag开启服务时获取到授权码");
                    // 发送邮件
                    client.Send(mailMsg);
                    // 断开连接
                    client.Disconnect(true);

                }
            }
        }
    }
}
