using System;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Wes.Utils
{
    /// <summary>
    /// SMTP 连接诊断工具，用于排查企业邮箱连接问题
    /// </summary>
    public static class SmtpDiagnostic
    {
        public static void TestConnection(string host, int port, bool useSsl, string account, string password)
        {
            Console.WriteLine("========== SMTP 连接诊断 ==========");
            Console.WriteLine($"目标: {host}:{port}, SSL: {useSsl}");

            // 步骤1：TCP 连接
            Console.Write("\n[1] TCP 连接... ");
            using var tcp = new TcpClient();
            try
            {
                tcp.Connect(host, port);
                Console.WriteLine("成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"失败: {ex.Message}");
                Console.WriteLine("    -> 请检查防火墙/安全组是否放行此端口");
                return;
            }

            // 步骤2：SSL 握手（仅 465 端口）
            if (useSsl && port == 465)
            {
                Console.Write("[2] SSL 握手... ");
                try
                {
                    using var ssl = new SslStream(tcp.GetStream(), false,
                        (s, c, ch, e) => true);
                    ssl.AuthenticateAsClient(host);
                    Console.WriteLine("成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"失败: {ex.Message}");
                    Console.WriteLine("    -> SSL/TLS 协商失败，可能是证书问题或端口不支持 SSL");
                }
                return;
            }

            // 步骤3：MailKit 连接（587 端口用 STARTTLS）
            Console.Write("[2] MailKit Connect... ");
            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Timeout = 30000;
                var option = useSsl && port == 587 ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
                client.Connect(host, port, option);
                Console.WriteLine("成功");

                // 步骤4：认证
                Console.Write("[3] 认证... ");
                try
                {
                    client.Authenticate(account, password);
                    Console.WriteLine("成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"失败: {ex.Message}");
                    Console.WriteLine("    -> 请检查账号密码是否正确，或是否开启了授权码");
                }

                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"失败: {ex.Message}");
                Console.WriteLine("    -> 请确认 SMTP 服务器地址和端口是否正确");
            }

            Console.WriteLine("\n========== 诊断结束 ==========");
        }
    }
}
