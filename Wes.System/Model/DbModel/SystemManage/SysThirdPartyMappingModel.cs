using SqlSugar;
using System;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;
using Wes.Utils;

namespace Wes.DbModel
{
    /// <summary>
    /// 三方平台映射表（本地实体与三方实体的对应关系）
    /// </summary>
    [SugarTable("sys_third_party_mapping", "三方平台映射", IsDisabledUpdateAll = true)]
    public class SysThirdPartyMappingModel
    {
        /// <summary>主键</summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>平台标识：DingTalk / WeCom / Feishu</summary>
        [SugarColumn(ColumnName = "provider", Length = 50)]
        public string Provider { get; set; }

        /// <summary>实体类型：Dept / User</summary>
        [SugarColumn(ColumnName = "entity_type", Length = 50)]
        public string EntityType { get; set; }

        /// <summary>三方平台 ID</summary>
        [SugarColumn(ColumnName = "third_party_id", Length = 100)]
        public string ThirdPartyId { get; set; }

        /// <summary>本地系统 ID</summary>
        [SugarColumn(ColumnName = "local_id", Length = 100)]
        public string LocalId { get; set; }

        /// <summary>显示名称（冗余，方便排查）</summary>
        [SugarColumn(ColumnName = "display_name", Length = 200, IsNullable = true)]
        public string DisplayName { get; set; }

        /// <summary>创建时间</summary>
        [SugarColumn(ColumnName = "create_time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
