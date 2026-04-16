using Wes.Business;
using Wes.Utils;
using Wes.Utils.Cache;
using Wes.Utils.Model;
using Wes.Utils.Security;
using Microsoft.AspNetCore.Mvc;
using Wes.ViewModel.SystemManage;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;

namespace Wes.WebApi.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;

        private ISysUserBiz _sysUserBiz { set; get; }
        private ISysMenuBiz _sysMenuBiz { set; get; }
        private ISysConfigBiz _sysConfigBiz { set; get; }

        public HomeController(IHostEnvironment hostEnvironment, ISysUserBiz sysUserBiz, ISysMenuBiz sysMenuBiz, ISysConfigBiz sysConfigBiz)
        {
            _hostEnvironment = hostEnvironment;
            _sysMenuBiz = sysMenuBiz;
            _sysUserBiz = sysUserBiz;
            _sysConfigBiz = sysConfigBiz;

        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public ReturnData Login([FromBody] LoginInfo login)
        {
            return _sysUserBiz.Login(login);
        }

        /// <summary>
        /// 是否开启验证码接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("isCaptchaOn")]
        [AllowAnonymous]
        public ReturnData IsCaptchaOn()
        {
            return new ResultData<bool>("true".Equals(_sysConfigBiz.GetByConfigKey("sys.login.isCaptchaOn")));
        }

        /// <summary>
        /// 获取验证码图片接口
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("captchaImage")]
        [AllowAnonymous]
        public ReturnData CaptchaImage(decimal width, decimal height)
        {
            CaptchaModel captchaModel = ImageHelper.GetCaptcha($"{_hostEnvironment.ContentRootPath}/UploadFile/Captcha/captcha1.jpg", width, height);
            CacheFactory.Cache.SetCache($"{CacheKey.CaptchaImg}{captchaModel.Code}", captchaModel.SliderPositionX, DateTime.Now.AddHours(10));
            CacheFactory.Cache.SetCache($"{CacheKey.CaptchaVaildImg}{captchaModel.Code}", captchaModel.SliderPositionX, DateTime.Now.AddHours(10));
            captchaModel.SliderPositionX = 0;
            return new ResultData<CaptchaModel>(captchaModel);
        }

        /// <summary>
        /// 验证码验证接口（只能验证一次）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="sliderPositionX"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("validCaptchaImage")]
        [AllowAnonymous]
        public ReturnData CaptchaImage(string code, decimal sliderPositionX)
        {
            decimal positionX = CacheFactory.Cache.GetCache<decimal>($"{CacheKey.CaptchaImg}{code}");
            if (positionX == 0)
            {
                return new ReturnData(408, "验证码已过期");
            }
            CacheFactory.Cache.RemoveCache($"{CacheKey.CaptchaImg}{code}");
            if (sliderPositionX < positionX - 0.01m || sliderPositionX > positionX + 0.01m)
            {
                return new ReturnData(500, "验证失败");
            }
            return new ReturnData(200, "验证成功");
        }

        /// <summary>
        /// 获取用户信息接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getInfo")]
        public ReturnData GetInfo()
        {
            return new UserInfoData(GlobalContext.CurrentUser);
        }

        [HttpGet]
        [Route("getRouters")]
        public ReturnData GetRouters()
        {
            return new ResultData<List<MenuRootInfo>>(_sysMenuBiz.GetUserMenu());
        }

        [HttpGet]
        [Route("getSystems")]
        public ReturnData GetSystems()
        {
            //return _sysSystemBiz.GetSytems();
            //TODO 获得系统列表接口，暂时没有系统，所以先返回null
            return null;
        }

        [HttpPost]
        [Route("logout")]
        [AllowAnonymous]
        public ReturnData Logout()
        {
            return _sysUserBiz.Logout(GlobalContext.Token.Value);
        }

        [HttpGet]
        [Route("token")]
        public string token(string p)
        {
            return AESUtils.Encrypt(MD5Utils.Encrypt(p));
        }

        [HttpGet]
        [Route("snowId")]
        [AllowAnonymous]
        public ReturnData GetSnowId()
        {
            string s = SqlSugar.SnowFlakeSingle.Instance.NextId().ToString();
            for (int i = 0; i < 9; i++)
            {
                s += "," + SqlSugar.SnowFlakeSingle.Instance.NextId().ToString();
            }
            return new ReturnData(200, s);
        }
    }
}
