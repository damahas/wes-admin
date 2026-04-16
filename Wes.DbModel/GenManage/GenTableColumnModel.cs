using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.JsonConverter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    [SugarTable("gen_table_column", IsDisabledUpdateAll = true)]
    public class GenTableColumnModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnName = "column_id", IsPrimaryKey = true)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ColumnId { get; set; }

        /// <summary>
        /// 归属表编号
        /// </summary>
        [SugarColumn(ColumnName = "table_id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long TableId { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        [SugarColumn(ColumnName = "column_name")]
        public string ColumnName { get; set; }

        /// <summary>
        /// 列描述
        /// </summary>
        [SugarColumn(ColumnName = "column_comment")]
        public string ColumnComment { get; set; }

        /// <summary>
        /// 列类型
        /// </summary>
        [SugarColumn(ColumnName = "column_type")]
        public string ColumnType { get; set; }

        /// <summary>
        /// 列长度
        /// </summary>
        [SugarColumn(ColumnName = "column_length")]
        public long ColumnLength { get; set; }

        /// <summary>
        /// 列长度
        /// </summary>
        [SugarColumn(ColumnName = "column_precision")]
        public long ColumnPrecision { get; set; }

        /// <summary>
        /// C#类型
        /// </summary>
        [SugarColumn(ColumnName = "c_type")]
        public string CType { get; set; }

        /// <summary>
        /// C#字段名
        /// </summary>
        [SugarColumn(ColumnName = "c_field")]
        public string CField { get; set; }

        /// <summary>
        /// 是否主键（1是）
        /// </summary>
        [SugarColumn(ColumnName = "is_pk")]
        public string IsPk { get; set; }

        /// <summary>
        /// 是否自增（1是）
        /// </summary>
        [SugarColumn(ColumnName = "is_increment")]
        public string IsIncrement { get; set; }

        /// <summary>
        /// 是否必填（1是）
        /// </summary>
        [SugarColumn(ColumnName = "is_required")]
        public string IsRequired { get; set; }

        /// <summary>
        /// 是否为插入字段（1是）
        /// </summary>
        [SugarColumn(ColumnName = "is_insert")]
        public string IsInsert { get; set; }

        /// <summary>
        /// 是否编辑字段（1是）
        /// </summary>
        [SugarColumn(ColumnName = "is_edit")]
        public string IsEdit { get; set; }

        /// <summary>
        /// 是否列表字段（1是）
        /// </summary>
        [SugarColumn(ColumnName = "is_list")]
        public string IsList { get; set; }

        /// <summary>
        /// 是否查询字段（1是）
        /// </summary>
        [SugarColumn(ColumnName = "is_query")]
        public string IsQuery { get; set; }

        /// <summary>
        /// 查询方式（等于、不等于、大于、小于、范围）
        /// </summary>
        [SugarColumn(ColumnName = "query_type")]
        public string QueryType { get; set; }

        /// <summary>
        /// 显示类型（文本框、文本域、下拉框、复选框、单选框、日期控件）
        /// </summary>
        [SugarColumn(ColumnName = "html_type")]
        public string HtmlType { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        [SugarColumn(ColumnName = "dict_type")]
        public string DictType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnName = "sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnName = "create_by")]
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        [SugarColumn(ColumnName = "update_by")]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}
