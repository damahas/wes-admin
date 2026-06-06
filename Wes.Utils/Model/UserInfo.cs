using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Wes.Utils.Converter;

namespace Wes.Utils.Model
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
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
        public long DeptId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户昵称
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
        /// 帐号状态（0正常 1停用）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.Dept?.DeptName;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return this.Roles != null && this.Roles.Select(p => p.RoleKey).Contains("admin");
            }
        }

        /// <summary>
        /// 用户角色
        /// </summary>
        public List<RoleInfo> Roles { set; get; }

        /// <summary>
        /// 用户岗位
        /// </summary>
        public List<PostInfo> Posts { set; get; }

        /// <summary>
        /// 用户所属部门
        /// </summary>
        public DeptInfo Dept { set; get; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public List<string> Permissions { set; get; }

        public DateTime ExpirationTime
        {
            get
            {
                //TODO 时间取值
                return DateTime.Now.AddDays(7);
            }
        }

    }

    /// <summary>
    /// 角色信息
    /// </summary>
    public class RoleInfo
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
        /// 数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限）
        /// </summary>
        public string DataScope { get; set; }

    }

    /// <summary>
    /// 岗位信息
    /// </summary>
    public class PostInfo
    {
        /// <summary>
        /// 岗位ID
        /// </summary>
        [JsonConverter(typeof(LongToStringConverter))]
        public long PostId { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PostName { get; set; }
    }

    /// <summary>
    /// 部门信息
    /// </summary>
    public class DeptInfo
    {
        /// <summary>
        /// 部门id
        /// </summary>
        [JsonConverter(typeof(LongToStringConverter))]
        public long DeptId { get; set; }

        /// <summary>
        /// 父部门id
        /// </summary>
        [JsonConverter(typeof(LongToStringConverter))]
        public long? ParentId { get; set; }

        /// <summary>
        /// 祖级列表
        /// </summary>
        public string Ancestors { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string Leader { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 部门状态（0正常 1停用）
        /// </summary>
        public string Status { get; set; }

    }
}
