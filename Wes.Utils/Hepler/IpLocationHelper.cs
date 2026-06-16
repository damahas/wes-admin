using System;
using System.IO;
using System.Linq;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;

namespace Wes.Utils.Hepler
{
    /// <summary>
    /// IP 归属地识别工具（基于 ip2region 离线库）
    /// 使用前请下载 ip2region.xdb 放到应用程序根目录
    /// </summary>
    public class IpLocationHelper
    {
        private static Searcher _searcher;
        private static bool _initialized;

        /// <summary>
        /// 初始化 IP 数据库
        /// </summary>
        public static void Initialize(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentNullException(nameof(dbPath));

            dbPath = Path.GetFullPath(dbPath);
            if (!File.Exists(dbPath))
                throw new FileNotFoundException($"ip2region 数据库文件不存在: {dbPath}，请从 https://gitee.com/lionsoul/ip2region/raw/master/data/ip2region.xdb 下载");

            _searcher = new Searcher(CachePolicy.File, dbPath);
            _initialized = true;
        }

        /// <summary>
        /// 根据 IP 获取归属地信息
        /// </summary>
        /// <returns>归属地字符串，如 "中国-广东省-深圳市"，未初始化或失败返回空</returns>
        public static string GetLocation(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return string.Empty;

            if (IsPrivateIp(ip))
                return "内网";

            if (!_initialized)
                return string.Empty;

            try
            {
                var raw = _searcher.Search(ip);
                return ParseLocation(raw);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ParseLocation(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw) || raw == "0|0|0|内网IP|内网IP")
                return "内网";

            var parts = raw.Split('|');
            if (parts.Length < 5)
                return raw;

            var country = CleanField(parts[0]);
            var province = CleanField(parts[2]);
            var city = CleanField(parts[3]);

            country = StripBrackets(country);
            province = StripBrackets(province);
            city = StripBrackets(city);

            if (province == city && !string.IsNullOrWhiteSpace(city))
                city = string.Empty;

            var result = string.Join("-",
                new[] { country, province, city }.Where(p => !string.IsNullOrWhiteSpace(p)));

            return string.IsNullOrWhiteSpace(result) ? raw : result;
        }

        private static string CleanField(string field)
        {
            if (string.IsNullOrWhiteSpace(field) || field == "0")
                return string.Empty;
            return field.Trim();
        }

        private static string StripBrackets(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            var idx = text.IndexOf('(');
            return idx > 0 ? text.Substring(0, idx) : text;
        }

        public static bool IsPrivateIp(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return true;
            if (ip == "::1" || ip == "0.0.0.0")
                return true;
            if (ip.StartsWith("127.") || ip.StartsWith("10.") || ip.StartsWith("192.168."))
                return true;
            if (ip.StartsWith("172."))
            {
                var parts = ip.Split('.');
                if (parts.Length >= 2 && int.TryParse(parts[1], out int s) && s >= 16 && s <= 31)
                    return true;
            }
            return false;
        }
    }
}
