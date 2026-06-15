using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 角色和菜单关联
    /// <summary>
    [SugarTable("sys_role_menu", "角色和菜单关联", IsDisabledDelete = true)]
    public class SysRoleMenuModel
    {
        /// <summary>
        /// 角色ID
        /// <summary>
        [SugarColumn(ColumnName = "role_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "角色ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { get; set; }

        /// <summary>
        /// 菜单ID
        /// <summary>
        [SugarColumn(ColumnName = "menu_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "菜单ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long MenuId { get; set; }

    }
}