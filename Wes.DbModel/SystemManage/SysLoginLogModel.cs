using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 系统访问记录
    /// </summary>
    [SugarTable("sys_login_log", "系统访问记录", IsDisabledDelete = true)]
    public class SysLoginLogModel
    {
        /// <summary>
        /// 访问ID
        /// <summary>
        [SugarColumn(ColumnName = "login_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "访问ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long LoginId { get; set; }

        /// <summary>
        /// 用户账号
        /// <summary>
        [SugarColumn(ColumnName = "user_name", IsNullable = true, Length = 50, ColumnDescription = "用户账号")]
        public string UserName { get; set; }

        /// <summary>
        /// 登录IP地址
        /// <summary>
        [SugarColumn(ColumnName = "ipaddr", IsNullable = true, Length = 128, ColumnDescription = "登录IP地址")]
        public string Ipaddr { get; set; }

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
        /// 登录状态（0成功 1失败）
        /// <summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "登录状态（0成功 1失败）")]
        public string Status { get; set; }

        /// <summary>
        /// 提示消息
        /// <summary>
        [SugarColumn(ColumnName = "msg", IsNullable = true, Length = 255, ColumnDescription = "提示消息")]
        public string Msg { get; set; }

        /// <summary>
        /// 访问时间
        /// <summary>
        [SugarColumn(ColumnName = "login_time", IsNullable = true, ColumnDescription = "访问时间")]
        public DateTime? LoginTime { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", IsNullable = true, Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int? IsDel { get; set; }
    }
}
