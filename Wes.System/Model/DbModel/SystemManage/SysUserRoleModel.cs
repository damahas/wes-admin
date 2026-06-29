using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;
using Wes.Utils;

namespace Wes.DbModel
{
    /// <summary>
    /// 用户和角色关联
    /// <summary>
    [SugarTable("sys_user_role", "用户和角色关联", IsDisabledUpdateAll = true)]
    public class SysUserRoleModel
    {
        /// <summary>
        /// 用户ID
        /// <summary>
        [SugarColumn(ColumnName = "user_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "用户ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// <summary>
        [SugarColumn(ColumnName = "role_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "角色ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { get; set; }

    }
}