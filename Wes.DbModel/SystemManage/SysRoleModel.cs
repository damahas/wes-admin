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
    /// 角色
    /// </summary>
    [SugarTable("sys_role", "角色信息", IsDisabledUpdateAll = true)]
    public class SysRoleModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ExportFieldAttr(FieldName = "角色编号")]
        [SugarColumn(ColumnName = "role_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "角色ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [ExportFieldAttr(FieldName = "角色名称")]
        [SugarColumn(ColumnName = "role_name", Length = 30, ColumnDescription = "角色名称")]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色权限字符串
        /// </summary>
        [ExportFieldAttr(FieldName = "权限字符串")]
        [SugarColumn(ColumnName = "role_key", Length = 100, ColumnDescription = "角色权限字符串")]
        public string RoleKey { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        [SugarColumn(ColumnName = "role_sort", Length = 4, ColumnDescription = "显示顺序")]
        public int RoleSort { get; set; }

        /// <summary>
        /// 数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限）
        /// </summary>
        [SugarColumn(ColumnName = "data_scope", IsNullable = true, Length = 1, ColumnDescription = "数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限）")]
        public string DataScope { get; set; }

        /// <summary>
        /// 菜单树选择项是否关联显示
        /// </summary>
        [SugarColumn(ColumnName = "menu_check_strictly", IsNullable = true, ColumnDescription = "菜单树选择项是否关联显示")]
        public bool MenuCheckStrictly { get; set; }

        /// <summary>
        /// 部门树选择项是否关联显示
        /// </summary>
        [SugarColumn(ColumnName = "dept_check_strictly", IsNullable = true, ColumnDescription = "部门树选择项是否关联显示")]
        public bool DeptCheckStrictly { get; set; }

        /// <summary>
        /// 角色状态（0正常 1停用）
        /// </summary>
        [ExportFieldAttr(FieldName = "角色状态", DicName = "sys_normal_disable")]
        [SugarColumn(ColumnName = "status", Length = 1, ColumnDescription = "角色状态（0正常 1停用）")]
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

        /// <summary>
        /// 备注
        /// <summary>
        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
        public string Remark { get; set; }

    }
}