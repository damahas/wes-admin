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
    /// 文件
    /// <summary>
    [SugarTable("sys_file", "文件", IsDisabledUpdateAll = true)]
    public class SysFileModel
    {
        /// <summary>
        /// 文件id
        /// <summary>
        [SugarColumn(ColumnName = "file_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "文件id")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long FileId { get; set; }

        /// <summary>
        /// 文件名
        /// <summary>
        [SugarColumn(ColumnName = "file_name", Length = 100, ColumnDescription = "文件名")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型
        /// <summary>
        [SugarColumn(ColumnName = "file_type", IsNullable = true, Length = 50, ColumnDescription = "文件类型")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件大小
        /// <summary>
        [SugarColumn(ColumnName = "file_size", Length = 20, ColumnDescription = "文件大小")]
        public long FileSize { get; set; }

        /// <summary>
        /// 文件路径
        /// <summary>
        [SugarColumn(ColumnName = "file_path", Length = 600, ColumnDescription = "文件路径")]
        public string FilePath { get; set; }

        /// <summary>
        /// 所属业务表
        /// <summary>
        [SugarColumn(ColumnName = "table_name", IsNullable = true, Length = 100, ColumnDescription = "所属业务表")]
        public string TableName { get; set; }

        /// <summary>
        /// 所属业务表id
        /// <summary>
        [SugarColumn(ColumnName = "table_id", IsNullable = true, Length = 20, ColumnDescription = "所属业务表id")]
        public long? TableId { get; set; }

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

    }
}
