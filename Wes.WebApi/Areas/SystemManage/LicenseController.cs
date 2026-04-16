using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Utils.Model;
using Wes.DbModel;
using Wes.ViewModel.SystemManage;
using Microsoft.AspNetCore.Http;
using Wes.Utils;
using Wes.Utils.Security;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/license")]
    public class LicenseController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public ReturnData GetById(long id)
        {
            LicenseInfo result = new LicenseInfo()
            {
                PlatformOS = NetHepler.Os,
                PlatformCode = GlobalContext.DeviceId
            };
            return new ResultData<LicenseInfo>(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public ReturnData Save([FromBody]LicenseInfo licenseInfo)
        {
            string licensePath = $"{GlobalContext.AppSettings.FilePath.TrimEnd('/')}/license.key";
            try
            {
                var md5 = MD5Utils.EncryptX32(GlobalContext.DeviceId);
                LicenseModel licenseModel = JsonConvert.DeserializeObject<LicenseModel>(AESUtils.Decrypt(licenseInfo.LicenseCode, md5, md5.Substring(8, 16)));
                if (licenseModel == null)
                {
                    return new ReturnData(501, "许可证格式错误，请输入正确的许可证！");
                }
                licenseModel.ActivateTime = DateTime.Now;
                if (licenseModel.ExpireTime < DateTime.Now)
                {
                    return new ReturnData(501, "许可证已过期！");
                }
                if (!Directory.Exists(GlobalContext.AppSettings.FilePath.TrimEnd('/')))
                {
                    Directory.CreateDirectory(GlobalContext.AppSettings.FilePath.TrimEnd('/'));
                }
                System.IO.File.WriteAllText(licensePath, AESUtils.Encrypt(JsonConvert.SerializeObject(licenseModel), md5, md5.Substring(8, 16)));
                GlobalContext.LicenseModel = null;
                return new ResultData<LicenseInfo>(licenseInfo);
            }
            catch (Exception ex)
            {
                return new ReturnData(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("expireTime")]
        [AllowAnonymous]
        public ReturnData GetExpireTime()
        {
            if (GlobalContext.AppSettings.IsLicense) {
                return new ResultData<DateTime?>(DateTime.Now.AddYears(100));
            }
            return new ResultData<DateTime?>(GlobalContext.LicenseModel?.ExpireTime);
        }
    }
}