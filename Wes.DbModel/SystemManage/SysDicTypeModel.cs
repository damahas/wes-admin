using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 字典项
    /// </summary>
    [SugarTable("sys_dict_type", "字典类型", IsDisabledDelete = true)]
    public class SysDicTypeModel
    {
        /// <summary>
        /// 字典主键
        /// </summary>
        [SugarColumn(ColumnName = "dict_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "字典主键")]
        [ExportFieldAttr(FieldName = "字典编号")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long DictId { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        [SugarColumn(ColumnName = "dict_name", IsNullable = true, Length = 100, ColumnDescription = "字典名称")]
        [ExportFieldAttr(FieldName = "字典名称")]
        public string DictName { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        [SugarColumn(ColumnName = "dict_type", IsNullable = true, Length = 100, ColumnDescription = "字典类型")]
        [ExportFieldAttr(FieldName = "字典类型")]
        public string DictType { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// </summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "状态（0正常 1停用）")]
        [ExportFieldAttr(FieldName = "字典状态", DicName = "sys_normal_disable")]
        public string Status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// </summary>
        [SugarColumn(ColumnName = "is_del", IsNullable = true, Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int? IsDel { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
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
        /// </summary>
        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
        [ExportFieldAttr(FieldName = "备注")]
        public string Remark { get; set; }
    }
}

