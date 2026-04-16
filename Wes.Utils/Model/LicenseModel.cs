using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils.Model
{
    public class LicenseModel
    {
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { set; get; }

        /// <summary>
        /// trial 试用版 enterprise 企业版
        /// </summary>
        public string LicenseType { set; get; }

        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime ActivateTime { set; get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { set; get; }

        /// <summary>
        /// 刷新时间
        /// </summary>
        public DateTime RefreshTime { set; get; }
    }
}
