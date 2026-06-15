using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 数据服务
    /// </summary>
    [SugarTable("sys_data_service", "数据服务", IsDisabledDelete = true)]
    public class SysDataServiceModel
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(ColumnName = "ds_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "主键id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long DsId { get; set; }

        /// <summary>
        /// 数据服务编码
        /// </summary>
        [SugarColumn(ColumnName = "service_code", IsNullable = true, Length = 50, ColumnDescription = "数据服务编码")]
        public string ServiceCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "service_name", IsNullable = true, Length = 100, ColumnDescription = "名称")]
        public string ServiceName { get; set; }

        /// <summary>
        /// 分类字典
        /// </summary>
        [SugarColumn(ColumnName = "category", IsNullable = true, Length = 20, ColumnDescription = "分类字典")]
        public string Category { get; set; }

        /// <summary>
        /// 参数配置
        /// </summary>
        [SugarColumn(ColumnName = "param_config", IsNullable = true, Length = 1024, ColumnDescription = "参数配置")]
        public string ParamConfig { get; set; }

        /// <summary>
        /// 是否有效（0代表无效 1代表有效）
        /// </summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, Length = 11, ColumnDescription = "是否有效（0代表无效 1代表有效）")]
        public string Status { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 50, ColumnDescription = "描述")]
        public string Remark { get; set; }

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
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        [SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64, ColumnDescription = "更新者")]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(DsId))]
        public List<SysDataServiceNodeModel> Nodes { get; set; }
    }
}
