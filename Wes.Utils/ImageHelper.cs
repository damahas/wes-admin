using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Wes.Utils.Model;

namespace Wes.Utils
{
    public static class ImageHelper
    {
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="width">宽（比例）</param>
        /// <param name="height">高（比例）</param>
        /// <returns></returns>
        public static CaptchaModel GetCaptcha(string path, decimal width, decimal height)
        {
            CaptchaModel captchaModel = new CaptchaModel()
            {
                Code = Guid.NewGuid().ToString("N")
            };
            using (Image image = Image.Load(path))
            {
                int sliderWidth = Convert.ToInt32(image.Width * width);
                int sliderHeight = Convert.ToInt32(image.Height * height);
                Random rangdomRed = new Random(DateTime.Now.Second);
                var SliderPositionX = rangdomRed.Next(1, image.Width - sliderWidth);
                var SliderPositionY = rangdomRed.Next(1, image.Height - sliderHeight);
                using (Image slider = image.Clone(p => { }))
                {
                    slider.Mutate(x => x.Crop(new Rectangle(SliderPositionX, SliderPositionY, sliderWidth, sliderHeight)));
                    captchaModel.SliderImg = slider.ToBase64String(JpegFormat.Instance);
                    slider.Mutate(x => x.GaussianBlur(80));
                    image.Mutate(x => x.DrawImage(slider, new Point(SliderPositionX, SliderPositionY), 1));
                }
                captchaModel.CaptchaImg = image.ToBase64String(JpegFormat.Instance);
                captchaModel.SliderPositionY = Math.Round(Convert.ToDecimal(SliderPositionY) / Convert.ToDecimal(image.Height) * 100, 2);
                captchaModel.SliderPositionX = Convert.ToDecimal(SliderPositionX) / Convert.ToDecimal(image.Width);
            }
            return captchaModel;
        }

    }
}
