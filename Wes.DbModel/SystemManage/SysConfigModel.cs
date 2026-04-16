using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils;
using Wes.Utils.JsonConverter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 参数配置
    /// </summary>
    [SugarTable("sys_config", "参数配置", IsDisabledUpdateAll = true)]
    public class SysConfigModel
    {
        /// <summary>
        /// 参数主键
        /// </summary>
        [SugarColumn(ColumnName = "config_id", IsPrimaryKey = true, Length = 5, ColumnDescription = "参数主键")]
        [ExportFieldAttr(FieldName = "参数编码")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ConfigId { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        [SugarColumn(ColumnName = "config_name", IsNullable = true, Length = 100, ColumnDescription = "参数名称")]
        [ExportFieldAttr(FieldName = "参数名称")]
        public string ConfigName { get; set; }

        /// <summary>
        /// 参数键名
        /// </summary>
        [SugarColumn(ColumnName = "config_key", IsNullable = true, Length = 100, ColumnDescription = "参数键名")]
        [ExportFieldAttr(FieldName = "参数键名")]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 参数键值
        /// </summary>
        [SugarColumn(ColumnName = "config_value", IsNullable = true, Length = 500, ColumnDescription = "参数键值")]
        [ExportFieldAttr(FieldName = "参数键值")]
        public string ConfigValue { get; set; }

        /// <summary>
        /// 系统内置（Y是 N否）
        /// </summary>
        [SugarColumn(ColumnName = "config_type", IsNullable = true, Length = 1, ColumnDescription = "系统内置（Y是 N否）")]
        [ExportFieldAttr(FieldName = "系统内置", DicName = "sys_yes_no")]
        public string ConfigType { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", IsNullable = true, Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int? IsDel { get; set; }

        /// <summary>
        /// 创建者
        /// <summary>
        [SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64, ColumnDescription = "创建者")]
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
        [ExportFieldAttr(FieldName = "创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// <summary>
        [SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64, ColumnDescription = "更新者")]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// <summary>
        [SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// <summary>
        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
        public string Remark { get; set; }
    }
}
