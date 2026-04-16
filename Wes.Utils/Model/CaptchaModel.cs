using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils.Model
{
    /// <summary>
    /// 验证码图片
    /// </summary>
    public class CaptchaModel
    {
        /// <summary>
        /// 验证码唯一标识
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 验证码图片
        /// </summary>
        public string CaptchaImg { set; get; }

        /// <summary>
        /// 滑块图片
        /// </summary>
        public string SliderImg { set; get; }

        /// <summary>
        /// 滑块Y
        /// </summary>
        public decimal SliderPositionY { set; get; }

        /// <summary>
        /// 滑块X
        /// </summary>
        public decimal SliderPositionX { set; get; }
    }
}
