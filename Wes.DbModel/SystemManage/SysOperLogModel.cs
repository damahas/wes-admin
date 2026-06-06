using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 操作日志记录
    /// </summary>
    [SugarTable("sys_oper_log", "操作日志记录", IsDisabledUpdateAll = true)]
    public class SysOperLogModel
    {
        /// <summary>
        /// 日志主键
        /// <summary>
        [SugarColumn(ColumnName = "oper_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "日志主键")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long OperId { get; set; }

        /// <summary>
        /// 模块标题
        /// <summary>
        [SugarColumn(ColumnName = "title", IsNullable = true, Length = 50, ColumnDescription = "模块标题")]
        public string Title { get; set; }

        /// <summary>
        /// 业务类型（0其它 1新增 2修改 3删除）
        /// <summary>
        [SugarColumn(ColumnName = "business_type", IsNullable = true, Length = 2, ColumnDescription = "业务类型（0其它 1新增 2修改 3删除）")]
        public int? BusinessType { get; set; }

        /// <summary>
        /// 方法名称
        /// <summary>
        [SugarColumn(ColumnName = "method", IsNullable = true, Length = 100, ColumnDescription = "方法名称")]
        public string Method { get; set; }

        /// <summary>
        /// 请求方式
        /// <summary>
        [SugarColumn(ColumnName = "request_method", IsNullable = true, Length = 10, ColumnDescription = "请求方式")]
        public string RequestMethod { get; set; }

        /// <summary>
        /// 操作类别（0其它 1后台用户 2手机端用户）
        /// <summary>
        [SugarColumn(ColumnName = "operator_type", IsNullable = true, Length = 1, ColumnDescription = "操作类别（0其它 1后台用户 2手机端用户）")]
        public int? OperatorType { get; set; }

        /// <summary>
        /// 操作人员
        /// <summary>
        [SugarColumn(ColumnName = "oper_name", IsNullable = true, Length = 50, ColumnDescription = "操作人员")]
        public string OperName { get; set; }

        /// <summary>
        /// 部门名称
        /// <summary>
        [SugarColumn(ColumnName = "dept_name", IsNullable = true, Length = 50, ColumnDescription = "部门名称")]
        public string DeptName { get; set; }

        /// <summary>
        /// 请求URL
        /// <summary>
        [SugarColumn(ColumnName = "oper_url", IsNullable = true, Length = 255, ColumnDescription = "请求URL")]
        public string OperUrl { get; set; }

        /// <summary>
        /// 主机地址
        /// <summary>
        [SugarColumn(ColumnName = "oper_ip", IsNullable = true, Length = 128, ColumnDescription = "主机地址")]
        public string OperIp { get; set; }

        /// <summary>
        /// 操作地点
        /// <summary>
        [SugarColumn(ColumnName = "oper_location", IsNullable = true, Length = 255, ColumnDescription = "操作地点")]
        public string OperLocation { get; set; }

        /// <summary>
        /// 请求参数
        /// <summary>
        [SugarColumn(ColumnName = "oper_param", IsNullable = true, Length = 2000, ColumnDescription = "请求参数")]
        public string OperParam { get; set; }

        /// <summary>
        /// 返回参数
        /// <summary>
        [SugarColumn(ColumnName = "json_result", IsNullable = true, Length = 2000, ColumnDescription = "返回参数")]
        public string JsonResult { get; set; }

        /// <summary>
        /// 操作状态（0正常 1异常）
        /// <summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "操作状态（0正常 1异常）")]
        public int? Status { get; set; }

        /// <summary>
        /// 错误消息
        /// <summary>
        [SugarColumn(ColumnName = "error_msg", IsNullable = true, Length = 2000, ColumnDescription = "错误消息")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 操作时间
        /// <summary>
        [SugarColumn(ColumnName = "oper_time", IsNullable = true, ColumnDescription = "操作时间")]
        public DateTime? OperTime { get; set; }

        /// <summary>
        /// 操作时间
        /// <summary>
        [SugarColumn(ColumnName = "cost_time", IsNullable = true, ColumnDescription = "消耗时间")]
        public int CostTime { get; set; }
    }
}
