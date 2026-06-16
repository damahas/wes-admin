using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace Wes.Utils.Hepler
{
    public class NetHepler
    {
        private static IHttpContextAccessor _contextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public static HttpContext HttpContext => _contextAccessor.HttpContext;

        public static string Os
        {
            get
            {
                if (UserAgent.Contains("NT 10"))
                {
                    return "Windows 10";
                }
                if (UserAgent.Contains("NT 6.3"))
                {
                    return "Windows 8";
                }
                if (UserAgent.Contains("NT 6.1"))
                {
                    return "Windows 7";
                }
                if (UserAgent.Contains("NT 6.0"))
                {
                    return "Windows Vista/Server 2008";
                }
                if (UserAgent.Contains("NT 5.2"))
                {
                    return "Windows Server 2003";
                }
                if (UserAgent.Contains("NT 5.1"))
                {
                    return "Windows XP";
                }
                if (UserAgent.Contains("NT 5"))
                {
                    return "Windows 2000";
                }
                if (UserAgent.Contains("NT 4"))
                {
                    return "Windows NT4";
                }
                if (UserAgent.Contains("Android"))
                {
                    return "Android";
                }
                if (UserAgent.Contains("Me"))
                {
                    return "Windows Me";
                }
                if (UserAgent.Contains("Mac"))
                {
                    return "Mac";
                }
                if (UserAgent.Contains("Unix"))
                {
                    return "UNIX";
                }
                if (UserAgent.Contains("Linux"))
                {
                    return "Linux";
                }
                if (UserAgent.Contains("SunOS"))
                {
                    return "SunOS";
                }
                if (UserAgent.Contains("98"))
                {
                    return "Windows 98";
                }
                if (UserAgent.Contains("95"))
                {
                    return "Windows 95";
                }
                return string.Empty;
            }
        }

        public static string Ip
        {
            get
            {
                string result = string.Empty;
                try
                {
                    if (HttpContext != null)
                    {
                        result = WebClientIp;
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        result = LanIp;
                    }
                }
                catch (Exception ex)
                {
                }
                return result;
            }
        }

        /// <summary>
        /// 获取 IP 归属地（同步，可能阻塞）
        /// </summary>
        public static string IpLocation
        {
            get
            {
                try
                {
                    return IpLocationHelper.GetLocation(Ip);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public static string Browser
        {
            get
            {
                var agent = UserAgent.ToLower();
                try
                {
                    if (agent.Contains("firefox"))
                        return "Firefox";
                    if (agent.Contains("chrome"))
                        return "Chrome";
                    if (agent.Contains("edge"))
                        return "Edge";
                    if (agent.Contains("safari"))
                        return "Safari";
                    if (agent.Contains("opera"))
                        return "Opera12";
                    if (agent.Contains("opr"))
                        return "Opera15";
                    if (agent.Contains("msie"))
                        return "IE10";
                    if (agent.Contains("ie 11.0"))
                        return "IE11";
                    if (agent.Contains("rv:") && UserAgent.Contains("trident"))
                        return "IE";
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        private static string UserAgent
        {
            get
            {
                try
                {
                    return HttpContext?.Request?.Headers["User-Agent"];
                }
                catch (Exception ex)
                {
                }
                return string.Empty;
            }
        }

        private static string WebRemoteIp
        {
            get
            {
                try
                {
                    string ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
                    if (HttpContext != null && HttpContext.Request != null)
                    {
                        if (HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
                        {
                            ip = HttpContext.Request.Headers["X-Real-IP"].ToString();
                        }

                        if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                        {
                            ip = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                        }
                    }
                    return ip;
                }
                catch (Exception ex)
                {
                }
                return string.Empty;
            }
        }

        private static string WebClientIp
        {
            get
            {
                try
                {
                    foreach (var hostAddress in Dns.GetHostAddresses(WebRemoteIp))
                    {
                        if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return hostAddress.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                return string.Empty;
            }
        }

        private static string LanIp
        {
            get
            {
                try
                {
                    foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                    {
                        if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return hostAddress.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                return string.Empty;
            }
        }
    }
}
