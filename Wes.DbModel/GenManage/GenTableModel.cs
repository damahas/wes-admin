using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    [SugarTable("gen_table", IsDisabledUpdateAll = true)]
    public class GenTableModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnName = "table_id", IsPrimaryKey = true)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long TableId { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        [SugarColumn(ColumnName = "table_name")]
        public string TableName { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        [SugarColumn(ColumnName = "table_comment")]
        public string TableComment { get; set; }

        /// <summary>
        /// 关联子表的表名
        /// </summary>
        [SugarColumn(ColumnName = "sub_table_name")]
        public string SubTableName { get; set; }

        /// <summary>
        /// 子表关联的外键名
        /// </summary>
        [SugarColumn(ColumnName = "sub_table_fk_name")]
        public string SubTableFkName { get; set; }

        /// <summary>
        /// 实体类名称
        /// </summary>
        [SugarColumn(ColumnName = "class_name")]
        public string ClassName { get; set; }

        /// <summary>
        /// 使用的模板（crud单表操作 tree树表操作）
        /// </summary>
        [SugarColumn(ColumnName = "tpl_category")]
        public string TplCategory { get; set; }

        /// <summary>
        /// 生成包路径
        /// </summary>
        [SugarColumn(ColumnName = "package_name")]
        public string PackageName { get; set; }

        /// <summary>
        /// 生成模块名
        /// </summary>
        [SugarColumn(ColumnName = "module_name")]
        public string ModuleName { get; set; }

        /// <summary>
        /// 生成业务名
        /// </summary>
        [SugarColumn(ColumnName = "business_name")]
        public string BusinessName { get; set; }

        /// <summary>
        /// 生成功能名
        /// </summary>
        [SugarColumn(ColumnName = "function_name")]
        public string FunctionName { get; set; }

        /// <summary>
        /// 生成功能作者
        /// </summary>
        [SugarColumn(ColumnName = "function_author")]
        public string FunctionAuthor { get; set; }

        /// <summary>
        /// 生成代码方式（0zip压缩包 1项目路径）
        /// </summary>
        [SugarColumn(ColumnName = "gen_type")]
        public string GenType { get; set; }

        /// <summary>
        /// 生成路径（不填默认项目路径）
        /// </summary>
        [SugarColumn(ColumnName = "gen_path")]
        public string GenPath { get; set; }

        /// <summary>
        /// 其它生成选项
        /// </summary>
        [SugarColumn(ColumnName = "options")]
        public string Options { get; set; }

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

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(TableId))]
        public List<GenTableColumnModel> Columns { get; set; }

        ///// <summary>
        ///// 选项配置
        ///// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public GenTableOption Params
        //{
        //    set
        //    {
        //        if (value != null)
        //        {
        //            Options = JsonConvert.SerializeObject(value);
        //        }
        //    }
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(Options))
        //            return null;
        //        return JsonConvert.DeserializeObject<GenTableOption>(Options);
        //    }
        //}
    }

    public class GenTableOption
    {
        public string parentMenuId { set; get; }
    }
}
