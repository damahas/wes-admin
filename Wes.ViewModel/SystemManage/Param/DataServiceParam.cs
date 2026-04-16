using System;

namespace Wes.ViewModel.SystemManage
{
    public class DataServiceParam
    {
        /// <summary>
        /// 数据服务编码
        /// </summary>
        public string ServiceCode { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ServiceName { set; get; }

        /// <summary>
        /// 服务类型 sql api
        /// </summary>
        public string ServiceType { set; get; }

        /// <summary>
        /// 是否有效（0代表无效 1代表有效）
        /// </summary>
        public string IsEnabled { set; get; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { set; get; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { set; get; }

        /// <summary>
        /// 分类id
        /// </summary>
        public string Category { set; get; }
    }
}
