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
    /// 字典值
    /// </summary>
    [SugarTable("sys_dict_data", "字典数据", IsDisabledUpdateAll = true)]
    public class SysDicDataModel
    {
        /// <summary>
        /// 字典主键
        /// </summary>
        [SugarColumn(ColumnName = "dict_data_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "字典编码")]
        [ExportFieldAttr(FieldName = "字典编码")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long DictDataId { get; set; }

        /// <summary>
        /// 字典排序
        /// </summary>
        [SugarColumn(ColumnName = "dict_sort", IsNullable = true, Length = 4, ColumnDescription = "字典排序")]
        public int DictSort { get; set; }

        /// <summary>
        /// 字典标签
        /// </summary>
        [SugarColumn(ColumnName = "dict_label", IsNullable = true, Length = 100, ColumnDescription = "字典标签")]
        [ExportFieldAttr(FieldName = "字典标签")]
        public string DictLabel { get; set; }

        /// <summary>
        /// 字典键值
        /// </summary>
        [SugarColumn(ColumnName = "dict_value", IsNullable = true, Length = 100, ColumnDescription = "字典键值")]
        [ExportFieldAttr(FieldName = "字典值")]
        public string DictValue { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        [SugarColumn(ColumnName = "dict_type", IsNullable = true, Length = 100, ColumnDescription = "字典类型")]
        public string DictType { get; set; }

        /// <summary>
        /// 样式属性（其他样式扩展）
        /// </summary>
        [SugarColumn(ColumnName = "css_class", IsNullable = true, Length = 100, ColumnDescription = "样式属性（其他样式扩展）")]
        public string CssClass { get; set; }

        /// <summary>
        /// 表格回显样式
        /// </summary>
        [SugarColumn(ColumnName = "list_class", IsNullable = true, Length = 100, ColumnDescription = "表格回显样式")]
        public string ListClass { get; set; }

        /// <summary>
        /// 是否默认（Y是 N否）
        /// </summary>
        [SugarColumn(ColumnName = "is_default", IsNullable = true, Length = 1, ColumnDescription = "是否默认（Y是 N否）")]
        public string IsDefault { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// </summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "状态（0正常 1停用）")]
        [ExportFieldAttr(FieldName = "字典状态", DicName = "sys_normal_disable")]
        public string Status { get; set; }

        /// <summary>
        /// 父节点id
        /// </summary>
        [SugarColumn(ColumnName = "parent_id", ColumnDescription = "父节点id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ParentId { get; set; }

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
        /// <summary>
        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
        public string Remark { get; set; }
    }
}

