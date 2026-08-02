using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Wes.Utils.Converter;

namespace Wes.ViewModel.SystemManage
{
    public class UserModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonConverter(typeof(LongToStringConverter))]
        public long UserId { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [JsonConverter(typeof(LongToStringConverter))]
        public long? DeptId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户类型（00系统用户）
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phonenumber { get; set; }

        /// <summary>
        /// 用户性别（0男 1女 2未知）
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// 帐号状态（0正常 1停用）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// </summary>
        public int? IsDel { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LoginIp { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LoginDate { get; set; }

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

        public List<long> PostIds { set; get; }

        public List<long> RoleIds { set; get; }
    }
}
