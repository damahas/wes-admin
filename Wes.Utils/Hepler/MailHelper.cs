using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Wes.Utils.Model;

namespace Wes.Utils.Hepler
{
    public class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="config">邮箱配置</param>
        /// <param name="to">收件人（多个用逗号分隔）</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容（支持HTML）</param>
        /// <param name="cc">抄送人（可选，多个用逗号分隔）</param>
        /// <returns>是否发送成功</returns>
        public static bool Send(MailModel config, string to, string subject, string body, string cc = null)
        {
            return SendWithAttachment(config, to, subject, body, cc, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="config">邮箱配置</param>
        /// <param name="to">收件人（多个用逗号分隔）</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容（支持HTML）</param>
        /// <param name="cc">抄送人（可选，多个用逗号分隔）</param>
        /// <param name="errorMsg">异常信息</param>
        /// <returns>是否发送成功</returns>
        public static bool Send(MailModel config, string to, string subject, string body, string cc, out string errorMsg)
        {
            return SendWithAttachment(config, to, subject, body, cc, null, out errorMsg);
        }

        /// <summary>
        /// 发送邮件（支持附件）
        /// </summary>
        /// <param name="config">邮箱配置</param>
        /// <param name="to">收件人（多个用逗号分隔）</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容（支持HTML）</param>
        /// <param name="cc">抄送人（可选，多个用逗号分隔）</param>
        /// <param name="attachments">附件文件路径列表（可选）</param>
        /// <returns>是否发送成功</returns>
        public static bool SendWithAttachment(MailModel config, string to, string subject, string body, string cc = null, List<string> attachments = null)
        {
            return SendWithAttachment(config, to, subject, body, cc, attachments, out _);
        }

        /// <summary>
        /// 发送邮件（支持附件）
        /// </summary>
        /// <param name="config">邮箱配置</param>
        /// <param name="to">收件人（多个用逗号分隔）</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容（支持HTML）</param>
        /// <param name="cc">抄送人（可选，多个用逗号分隔）</param>
        /// <param name="attachments">附件文件路径列表（可选）</param>
        /// <param name="errorMsg">异常信息</param>
        /// <returns>是否发送成功</returns>
        public static bool SendWithAttachment(MailModel config, string to, string subject, string body, string cc, List<string> attachments, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (config == null)
                    throw new ArgumentNullException(nameof(config));

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(Encoding.UTF8, "", config.MailAccount));
                message.Subject = subject;

                // 收件人
                foreach (var addr in to.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(new MailboxAddress(Encoding.UTF8, "", addr.Trim()));
                }

                // 抄送人
                if (!string.IsNullOrWhiteSpace(cc))
                {
                    foreach (var addr in cc.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.Cc.Add(new MailboxAddress(Encoding.UTF8, "", addr.Trim()));
                    }
                }

                // 邮件正文（支持HTML）
                var builder = new BodyBuilder { HtmlBody = body };

                // 附件
                if (attachments != null)
                {
                    foreach (var filePath in attachments)
                    {
                        if (File.Exists(filePath))
                            builder.Attachments.Add(filePath);
                    }
                }

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();

                // 跳过证书验证（部分企业邮箱使用自签名证书）
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // 设置超时（企业邮箱有时响应较慢，给 30 秒）
                client.Timeout = 30000;

                var port = int.Parse(config.MailPort.Trim());
                SecureSocketOptions secureOption;

                if (!config.EnableSsl)
                {
                    secureOption = SecureSocketOptions.None;
                }
                else if (port == 465 || port == 994)
                {
                    secureOption = SecureSocketOptions.SslOnConnect;  // 465/994：隐式 SSL
                }
                else
                {
                    secureOption = SecureSocketOptions.StartTls;       // 587/25：显式 STARTTLS
                }

                client.Connect(config.MailHost.Trim(), port, secureOption);

                client.Authenticate(config.MailAccount.Trim(), config.MailPassword);
                client.Send(message);
                client.Disconnect(true);

                return true;
            }
            catch (Exception ex)
            {
                errorMsg = GetExceptionMessage(ex);
                return false;
            }
        }

        /// <summary>
        /// 递归获取所有异常信息
        /// </summary>
        private static string GetExceptionMessage(Exception ex)
        {
            if (ex == null) return string.Empty;
            var msg = ex.Message;
            if (ex.InnerException != null)
                msg += " -> " + GetExceptionMessage(ex.InnerException);
            return msg;
        }
    }
}
