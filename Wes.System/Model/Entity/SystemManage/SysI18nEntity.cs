using System;
using SqlSugar;
using Newtonsoft.Json;
using Wes.Utils.Extension;
using Wes.Utils.Converter;

namespace Wes.Entity
{
    [SugarTable("sys_i18n", "国际化翻译", IsDisabledUpdateAll = true)]
    public class SysI18nEntity
    {
        [SugarColumn(ColumnName = "i18n_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "翻译ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long I18nId { get; set; }

        [SugarColumn(ColumnName = "i18n_key", IsNullable = true, Length = 200, ColumnDescription = "翻译键名")]
        public string I18nKey { get; set; }

        [SugarColumn(ColumnName = "lang", IsNullable = true, Length = 10, ColumnDescription = "语言")]
        public string Lang { get; set; }

        [SugarColumn(ColumnName = "i18n_value", IsNullable = true, Length = 500, ColumnDescription = "翻译文本")]
        public string I18nValue { get; set; }

        [SugarColumn(ColumnName = "is_del", DefaultValue = "0", IsNullable = true, Length = 1, ColumnDescription = "删除标志")]
        public int IsDel { get; set; }

        [SugarColumn(ColumnName = "create_by", IsNullable = true, Length = 64)]
        public string CreateBy { get; set; }

        [SugarColumn(ColumnName = "create_time", IsNullable = true)]
        public DateTime CreateTime { get; set; }

        [SugarColumn(ColumnName = "update_by", IsNullable = true, Length = 64)]
        public string UpdateBy { get; set; }

        [SugarColumn(ColumnName = "update_time", IsNullable = true)]
        public DateTime? UpdateTime { get; set; }

        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500)]
        public string Remark { get; set; }
    }
}
