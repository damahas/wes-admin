using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 部门
    /// </summary>
    [SugarTable("sys_dept", "部门", IsDisabledDelete = true)]
    public class SysDeptModel
    {
        /// <summary>
        /// 部门id
        /// <summary>
        [SugarColumn(ColumnName = "dept_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "部门id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long DeptId { get; set; }

        /// <summary>
        /// 父部门id
        /// <summary>
        [SugarColumn(ColumnName = "parent_id", Length = 20, ColumnDescription = "父部门id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ParentId { get; set; }

        /// <summary>
        /// 祖级列表
        /// <summary>
        [SugarColumn(ColumnName = "ancestors", IsNullable = true, Length = 50, ColumnDescription = "祖级列表")]
        public string Ancestors { get; set; }

        /// <summary>
        /// 部门名称
        /// <summary>
        [SugarColumn(ColumnName = "dept_name", IsNullable = true, Length = 30, ColumnDescription = "部门名称")]
        public string DeptName { get; set; }

        /// <summary>
        /// 显示顺序
        /// <summary>
        [SugarColumn(ColumnName = "order_num", IsNullable = true, Length = 4, ColumnDescription = "显示顺序")]
        public int? OrderNum { get; set; }

        /// <summary>
        /// 负责人
        /// <summary>
        [SugarColumn(ColumnName = "leader_user_id", IsNullable = true, Length = 20, ColumnDescription = "负责人")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long? LeaderUserId { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(LeaderUserId))]
        public SysUserModel LeaderUser { get; set; }

        /// <summary>
        /// 联系电话
        /// <summary>
        [SugarColumn(ColumnName = "phone", IsNullable = true, Length = 11, ColumnDescription = "联系电话")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// <summary>
        [SugarColumn(ColumnName = "email", IsNullable = true, Length = 50, ColumnDescription = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 部门状态（0正常 1停用）
        /// <summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "部门状态（0正常 1停用）")]
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
        /// <summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

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

    }
}