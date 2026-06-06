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
    /// 岗位信息
    /// </summary>
    [SugarTable("sys_post", "岗位信息", IsDisabledUpdateAll = true)]
    public class SysPostModel
    {
        /// <summary>
        /// 岗位ID
        /// </summary>
        [SugarColumn(ColumnName = "post_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "岗位ID")]
        [ExportFieldAttr(FieldName = "岗位编号")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long PostId { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        [SugarColumn(ColumnName = "post_code", Length = 64, ColumnDescription = "岗位编码")]
        [ExportFieldAttr(FieldName = "岗位编码")]
        public string PostCode { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        [SugarColumn(ColumnName = "post_name", Length = 50, ColumnDescription = "岗位名称")]
        [ExportFieldAttr(FieldName = "岗位名称")]
        public string PostName { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        [SugarColumn(ColumnName = "post_sort", Length = 4, ColumnDescription = "显示顺序")]
        public int PostSort { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// </summary>
        [SugarColumn(ColumnName = "status", Length = 1, ColumnDescription = "状态（0正常 1停用）")]
        [ExportFieldAttr(FieldName = "岗位状态", DicName = "sys_normal_disable")]
        public string Status { get; set; }

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

