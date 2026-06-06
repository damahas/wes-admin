using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 站内信
    /// <summary>
    [SugarTable("sys_message", "站内信", IsDisabledDelete = true)]
    public class SysMessageModel
    {
        /// <summary>
        /// 主键id
        /// <summary>
        [SugarColumn(ColumnName = "message_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "主键id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long MessageId { get; set; }

        /// <summary>
        /// 消息类型
        /// <summary>
        [SugarColumn(ColumnName = "message_type", Length = 100, ColumnDescription = "消息类型")]
        public string MessageType { get; set; }

        /// <summary>
        /// 打开类型（对话框dialog，内部in，外部out）
        /// <summary>
        [SugarColumn(ColumnName = "open_type", Length = 20, ColumnDescription = "对话框dialog，内部in，外部out")]
        public string OpenType { get; set; }

        /// <summary>
        /// 消息标题
        /// <summary>
        [SugarColumn(ColumnName = "message_title", Length = 200, ColumnDescription = "消息标题")]
        public string MessageTitle { get; set; }

        /// <summary>
        /// 消息内容
        /// <summary>
        [SugarColumn(ColumnName = "message_body", Length = 255, ColumnDescription = "消息内容")]
        public string MessageBody { get; set; }

        /// <summary>
        /// 接收人
        /// <summary>
        [SugarColumn(ColumnName = "user_id", Length = 20, ColumnDescription = "接收人")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long UserId { get; set; }

        /// <summary>
        /// 发送人
        /// <summary>
        [SugarColumn(ColumnName = "send_user_id", Length = 20, ColumnDescription = "发送人")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long SendUserId { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(SendUserId))]
        public SysUserModel SendUser { get; set; }

        /// <summary>
        /// 是否已读
        /// <summary>
        [SugarColumn(ColumnName = "is_read", Length = 10, ColumnDescription = "是否已读")]
        public int IsRead { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int IsDel { get; set; }

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
        /// 阅读时间
        /// <summary>
        [SugarColumn(ColumnName = "read_time", IsNullable = true, ColumnDescription = "阅读时间")]
        public DateTime? ReadTime { get; set; }
    }
}
