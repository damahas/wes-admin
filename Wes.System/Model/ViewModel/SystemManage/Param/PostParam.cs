using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    public class PostParam
    {
        /// <summary>
        /// 岗位编码
        /// </summary>
        public string? PostCode { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string? PostName { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// </summary>
        public string? Status { get; set; }
    }
}
