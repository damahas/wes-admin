using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 角色和部门关联
    /// <summary>
    [SugarTable("sys_role_dept", "角色和部门关联", IsDisabledUpdateAll = true)]
    public class SysRoleDeptModel
    {
        /// <summary>
        /// 角色ID
        /// <summary>
        [SugarColumn(ColumnName = "role_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "角色ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { get; set; }

        /// <summary>
        /// 部门ID
        /// <summary>
        [SugarColumn(ColumnName = "dept_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "部门ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long DeptId { get; set; }

    }
}