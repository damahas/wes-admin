using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using Wes.Utils.JsonConverter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 菜单
    /// </summary>
    [SugarTable("sys_menu", "菜单权限", IsDisabledUpdateAll = true)]
    public class SysMenuModel
    {
        /// <summary>
        /// 菜单ID
        /// <summary>
        [SugarColumn(ColumnName = "menu_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "菜单ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long MenuId { get; set; }

        /// <summary>
        /// 菜单名称
        /// <summary>
        [SugarColumn(ColumnName = "menu_name", Length = 50, ColumnDescription = "菜单名称")]
        public string MenuName { get; set; }

        /// <summary>
        /// 父菜单ID
        /// <summary>
        [SugarColumn(ColumnName = "parent_id", Length = 20, ColumnDescription = "父菜单ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ParentId { get; set; }

        /// <summary>
        /// 显示顺序
        /// <summary>
        [SugarColumn(ColumnName = "order_num", IsNullable = true, Length = 4, ColumnDescription = "显示顺序")]
        public int? OrderNum { get; set; }

        /// <summary>
        /// 路由地址
        /// <summary>
        [SugarColumn(ColumnName = "path", IsNullable = true, Length = 200, ColumnDescription = "路由地址")]
        public string Path { get; set; }

        /// <summary>
        /// 组件路径
        /// <summary>
        [SugarColumn(ColumnName = "component", IsNullable = true, Length = 255, ColumnDescription = "组件路径")]
        public string Component { get; set; }

        /// <summary>
        /// 路由参数
        /// <summary>
        [SugarColumn(ColumnName = "query", IsNullable = true, Length = 255, ColumnDescription = "路由参数")]
        public string Query { get; set; }

        /// <summary>
        /// 是否为外链（0是 1否）
        /// <summary>
        [SugarColumn(ColumnName = "is_frame", IsNullable = true, Length = 1, ColumnDescription = "是否为外链（0是 1否）")]
        public int? IsFrame { get; set; }

        /// <summary>
        /// 是否缓存（0缓存 1不缓存）
        /// <summary>
        [SugarColumn(ColumnName = "is_cache", IsNullable = true, Length = 1, ColumnDescription = "是否缓存（0缓存 1不缓存）")]
        public int? IsCache { get; set; }

        /// <summary>
        /// 菜单类型（M目录 C菜单 F按钮）
        /// <summary>
        [SugarColumn(ColumnName = "menu_type", IsNullable = true, Length = 1, ColumnDescription = "菜单类型（M目录 C菜单 F按钮）")]
        public string MenuType { get; set; }

        /// <summary>
        /// 菜单状态（0显示 1隐藏）
        /// <summary>
        [SugarColumn(ColumnName = "visible", IsNullable = true, Length = 1, ColumnDescription = "菜单状态（0显示 1隐藏）")]
        public string Visible { get; set; }

        /// <summary>
        /// 菜单状态（0正常 1停用）
        /// <summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "菜单状态（0正常 1停用）")]
        public string Status { get; set; }

        /// <summary>
        /// 权限标识
        /// <summary>
        [SugarColumn(ColumnName = "perms", IsNullable = true, Length = 100, ColumnDescription = "权限标识")]
        public string Perms { get; set; }

        /// <summary>
        /// 菜单图标
        /// <summary>
        [SugarColumn(ColumnName = "icon", IsNullable = true, Length = 100, ColumnDescription = "菜单图标")]
        public string Icon { get; set; }

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

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", IsNullable = true, Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int? IsDel { get; set; }

        /// <summary>
        /// 系统id
        /// <summary>
        [SugarColumn(ColumnName = "system_id", IsNullable = true, Length = 20, ColumnDescription = "系统id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long? SystemId { get; set; }
    }
}
