using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Wes.Utils.Converter;

namespace Wes.ViewModel.SystemManage
{
    public class RoleModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 角色权限字符串
        /// </summary>
        public string RoleKey { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int RoleSort { get; set; }

        /// <summary>
        /// 数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限）
        /// </summary>
        public string DataScope { get; set; }

        /// <summary>
        /// 菜单树选择项是否关联显示
        /// </summary>
        public bool MenuCheckStrictly { get; set; }

        /// <summary>
        /// 部门树选择项是否关联显示
        /// </summary>
        public bool DeptCheckStrictly { get; set; }

        /// <summary>
        /// 角色状态（0正常 1停用）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// </summary>
        public int? IsDel { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public List<long> MenuIds { set; get; }
    }
}
