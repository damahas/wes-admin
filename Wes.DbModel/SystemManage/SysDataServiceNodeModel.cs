using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.JsonConverter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 数据服务节点
    /// </summary>
    [SugarTable("sys_data_service_node", "数据服务节点", IsDisabledUpdateAll = true)]
    public class SysDataServiceNodeModel
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(ColumnName = "ds_part_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "主键id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long DsPartId { get; set; }

        /// <summary>
        /// 关联主表id
        /// </summary>
        [SugarColumn(ColumnName = "ds_id", IsNullable = true, Length = 20, ColumnDescription = "关联主表id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long? DsId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [SugarColumn(ColumnName = "part_name", IsNullable = true, Length = 100, ColumnDescription = "节点名称")]
        public string PartName { get; set; }

        /// <summary>
        /// 变量名
        /// </summary>
        [SugarColumn(ColumnName = "var_name", IsNullable = true, Length = 100, ColumnDescription = "变量名")]
        public string VarName { get; set; }

        /// <summary>
        /// 变量类型
        /// </summary>
        [SugarColumn(ColumnName = "var_type", IsNullable = true, Length = 1, ColumnDescription = "变量类型")]
        public DataServiceNodeReturnEnum VarType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [SugarColumn(ColumnName = "part_config", IsNullable = true, ColumnDescription = "内容")]
        public string PartConfig { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        [SugarColumn(ColumnName = "sort_by", IsNullable = true, Length = 11, ColumnDescription = "排序字段")]
        public int? SortBy { get; set; }

        /// <summary>
        /// 类型 0 sql，1 js
        /// </summary>
        [SugarColumn(ColumnName = "part_type", IsNullable = true, Length = 1, ColumnDescription = "类型 0 sql，1 js")]
        public DataServiceNodeTypeEnum? PartType { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// </summary>
        [SugarColumn(ColumnName = "is_del", IsNullable = true, Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int? IsDel { get; set; }
    }
}
