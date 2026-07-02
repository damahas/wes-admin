using DeviceId;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Wes.Utils.Cache;
using Wes.Utils.Model;
using System.IO;
using Wes.Utils.Security;
using Newtonsoft.Json;
using DeviceId.Linux;

namespace Wes.Utils
{
    /// <summary>
    /// 全局数据
    /// </summary>
    public class GlobalContext
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static AppSettings AppSettings { set; get; }

        public static JwtSettings JwtSettings { set; get; }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        //public static AsyncLocal<UserInfo> CurrentUser = new AsyncLocal<UserInfo>();
        public static UserInfo CurrentUser
        {
            get
            {
                return CacheFactory.Cache.GetCache<UserInfo>($"{CacheKey.AccessToken}{Token.Value}");
            }
        }

        /// <summary>
        /// 当前token
        /// </summary>
        public static AsyncLocal<string> Token = new AsyncLocal<string>();

        /// <summary>
        /// 设备序列号
        /// </summary>
        public static string DeviceId
        {
            get
            {
                string deviceCode = CacheFactory.Cache.GetCache<string>(CacheKey.DeviceId);
                if (!string.IsNullOrWhiteSpace(deviceCode))
                {
                    return deviceCode;
                }
                deviceCode = new DeviceIdBuilder().AddOsVersion().OnLinux(linux => linux.AddMachineId()).ToString();
                //deviceCode = new DeviceIdBuilder().AddOsVersion().AddMacAddress().ToString();
                //deviceCode = new DeviceIdBuilder().AddMachineName().AddOsVersion().AddMacAddress().ToString();
                CacheFactory.Cache.SetCache(CacheKey.DeviceId, deviceCode);
                return deviceCode;
            }
        }

        private static LicenseModel _licenseModel { set; get; }

        /// <summary>
        /// 许可证信息
        /// </summary>
        public static LicenseModel LicenseModel
        {
            set
            {
                _licenseModel = null;
            }
            get
            {
                if (!AppSettings.IsLicense)
                {
                    _licenseModel = new LicenseModel
                    {
                        LicenseType = "enterprise",
                        ActivateTime = DateTime.MinValue,
                        ExpireTime = DateTime.MaxValue,
                        RefreshTime = DateTime.MaxValue
                    };
                    return _licenseModel;
                }
                if (_licenseModel != null && _licenseModel.RefreshTime > DateTime.Now)
                {
                    return _licenseModel;
                }
                string licensePath = $"{AppSettings.FilePath.TrimEnd('/')}/UploadFile/DataProtection/{DeviceId}.key";
                if (!File.Exists(licensePath))
                {
                    return null;
                }
                var license = File.ReadAllText(licensePath);
                try
                {
                    var md5 = MD5Utils.EncryptX32(DeviceId);
                    _licenseModel = JsonConvert.DeserializeObject<LicenseModel>(AESUtils.Decrypt(license, md5, md5.Substring(8, 16)));
                    _licenseModel.RefreshTime = DateTime.Now.Date.AddDays(1);
                    return _licenseModel;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }

    /// <summary>
    /// AppSettings中配置
    /// </summary>
    public class AppSettings
    {
        public string CacheDrive { set; get; }
        public string RedisConnectionString { set; get; }
        public string FilePath => AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
        public long SystemId { set; get; }
        public bool IsLicense
        {
            get
            {
                #if DEBUG
                return false;
                #else
                return true;
                #endif
            }
        }
    }

    public class JwtSettings
    {
        public string Issuer { set; get; }
        public string Audience { set; get; }
        public int Expires { set; get; }
        public string SecretKey { set; get; }
    }

    /// <summary>
    /// 缓存key前缀
    /// </summary>
    public class CacheKey
    {
        public const string AccessToken = "token_";
        public const string Config = "config_";
        public const string Dic = "dic_";
        public const string CaptchaImg = "captcha_";
        public const string CaptchaVaildImg = "captcha_v_";
        public const string CodeRule = "code_";
        public const string DeviceId = "deviceId";
    }

    /// <summary>
    /// 数据库默认字段
    /// </summary>
    public class DbKey
    {
        public const string IsDel = "is_del";
        public const string CreateBy = "create_by";
        public const string CreateTime = "create_time";
        public const string UpdateBy = "update_by";
        public const string UpdateTime = "update_time";
    }
}
