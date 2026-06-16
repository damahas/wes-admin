using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    [SugarTable("sys_user_post", "用户与岗位关联", IsDisabledDelete = true)]
    public class SysUserPostModel
    {
        /// <summary>
        /// 用户ID
        /// <summary>
        [SugarColumn(ColumnName = "user_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "用户ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long UserId { get; set; }

        /// <summary>
        /// 岗位ID
        /// <summary>
        [SugarColumn(ColumnName = "post_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "岗位ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long PostId { get; set; }

    }
}