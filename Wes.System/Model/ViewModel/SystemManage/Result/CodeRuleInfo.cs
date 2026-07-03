using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Wes.DbModel;
using Wes.Utils.Converter;

namespace Wes.ViewModel.SystemManage
{
    public class CodeRuleInfo
    {
        /// <summary>
        /// 规则id
        /// <summary>
        [JsonConverter(typeof(LongToStringConverter))]
        public long RuleId { get; set; }

        /// <summary>
        /// 规则编码
        /// <summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// 规则名称
        /// <summary>
        public string RuleName { get; set; }

        /// <summary>
        /// 规则类型
        /// <summary>
        public string RuleType { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// <summary>
        public string Status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        public int? IsDel { get; set; }

        /// <summary>
        /// 创建者
        /// <summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// <summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// <summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// <summary>
        public DateTime? UpdateTime { get; set; }

        public List<SysCodeRulePartModel> Parts { set; get; }
    }
}
