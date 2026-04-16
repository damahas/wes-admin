using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { set; get; }

        /// <summary>
        /// 验证码唯一code
        /// </summary>
        public string code { set; get; }

        /// <summary>
        /// 验证码值
        /// </summary>
        public decimal positionX { set; get; }
    }
}
