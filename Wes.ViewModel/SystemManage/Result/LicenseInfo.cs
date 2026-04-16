using System;
using System.Collections.Generic;
using System.Text;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.ViewModel.SystemManage
{
    public class LicenseInfo
    {
        /// <summary>
        /// 平台系统
        /// </summary>
        public string PlatformOS { set; get; }

        /// <summary>
        /// 平台代码
        /// </summary>
        public string PlatformCode { set; get; }

        public string LicenseCode { set; get; }

        /// <summary>
        /// 许可证
        /// </summary>
        public LicenseModel LicenseModel
        {
            get
            {
                return GlobalContext.LicenseModel;
            }
        }
    }
}
