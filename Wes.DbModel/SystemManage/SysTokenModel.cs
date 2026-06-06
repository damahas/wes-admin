using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 登录密钥
    /// </summary>
    [SugarTable("sys_token", "登录token", IsDisabledUpdateAll = true)]
    public class SysTokenModel
    {
        /// <summary>
        /// 主键自增id
        /// <summary>
        [SugarColumn(ColumnName = "token_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "主键自增id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long TokenId { get; set; }

        /// <summary>
        /// 用户id
        /// <summary>
        [SugarColumn(ColumnName = "user_id", Length = 20, ColumnDescription = "用户id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long UserId { get; set; }

        /// <summary>
        /// 登录token
        /// <summary>
        [SugarColumn(ColumnName = "token", Length = 100, ColumnDescription = "登录token")]
        public string Token { get; set; }

        /// <summary>
        /// 状态（0正常 1停用 2过期）
        /// <summary>
        [SugarColumn(ColumnName = "status", Length = 1, ColumnDescription = "状态（0正常 1停用 2过期）")]
        public int Status { get; set; }

        /// <summary>
        /// 生成Token的IP
        /// <summary>
        [SugarColumn(ColumnName = "login_ip", IsNullable = true, Length = 128, ColumnDescription = "生成Token的IP")]
        public string LoginIp { get; set; }

        /// <summary>
        /// 登录地点
        /// <summary>
        [SugarColumn(ColumnName = "login_location", IsNullable = true, Length = 255, ColumnDescription = "登录地点")]
        public string LoginLocation { get; set; }

        /// <summary>
        /// 浏览器类型
        /// <summary>
        [SugarColumn(ColumnName = "browser", IsNullable = true, Length = 50, ColumnDescription = "浏览器类型")]
        public string Browser { get; set; }

        /// <summary>
        /// 操作系统
        /// <summary>
        [SugarColumn(ColumnName = "os", IsNullable = true, Length = 50, ColumnDescription = "操作系统")]
        public string Os { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int IsDel { get; set; }

        /// <summary>
        /// 过期时间
        /// <summary>
        [SugarColumn(ColumnName = "expiration_time", ColumnDescription = "过期时间")]
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// 创建时间
        /// <summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 来源 web app
        /// <summary>
        [SugarColumn(ColumnName = "source", DefaultValue = "web", Length = 50, ColumnDescription = "来源 web app")]
        public string Source { get; set; }
    }
}

