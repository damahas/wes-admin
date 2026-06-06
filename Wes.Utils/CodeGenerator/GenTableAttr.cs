using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils.Model
{
    public class GenTableAttr
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        public string TableComment { get; set; }

        /// <summary>
        /// 关联子表的表名
        /// </summary>
        public string SubTableName { get; set; }

        /// <summary>
        /// 子表关联的外键名
        /// </summary>
        public string SubTableFkName { get; set; }

        /// <summary>
        /// 实体类名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 使用的模板（crud单表操作 tree树表操作）
        /// </summary>
        public string TplCategory { get; set; }

        /// <summary>
        /// 生成包路径
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// 生成模块名
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 生成业务名
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// 生成功能名
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 生成功能作者
        /// </summary>
        public string FunctionAuthor { get; set; }

        /// <summary>
        /// 其它生成选项
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public List<GenTableColumnAttr> Columns { get; set; }
    }

    public class GenTableColumnAttr
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 列描述
        /// </summary>
        public string ColumnComment { get; set; }

        /// <summary>
        /// 列类型
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// 列长度
        /// </summary>
        public long ColumnLength { get; set; }

        /// <summary>
        /// 列长度
        /// </summary>
        public long ColumnPrecision { get; set; }

        /// <summary>
        /// C#类型
        /// </summary>
        public string CType { get; set; }

        /// <summary>
        /// C#字段名
        /// </summary>
        public string CField { get; set; }

        /// <summary>
        /// 是否主键（1是）
        /// </summary>
        public string IsPk { get; set; }

        /// <summary>
        /// 是否自增（1是）
        /// </summary>
        public string IsIncrement { get; set; }

        /// <summary>
        /// 是否必填（1是）
        /// </summary>
        public string IsRequired { get; set; }

        /// <summary>
        /// 是否为插入字段（1是）
        /// </summary>
        public string IsInsert { get; set; }

        /// <summary>
        /// 是否编辑字段（1是）
        /// </summary>
        public string IsEdit { get; set; }

        /// <summary>
        /// 是否列表字段（1是）
        /// </summary>
        public string IsList { get; set; }

        /// <summary>
        /// 是否查询字段（1是）
        /// </summary>
        public string IsQuery { get; set; }

        /// <summary>
        /// 查询方式（等于、不等于、大于、小于、范围）
        /// </summary>
        public string QueryType { get; set; }

        /// <summary>
        /// 显示类型（文本框、文本域、下拉框、复选框、单选框、日期控件）
        /// </summary>
        public string HtmlType { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        public string DictType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
    }
}
