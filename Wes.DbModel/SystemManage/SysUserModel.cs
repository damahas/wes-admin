using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Wes.Utils;
using Wes.Utils.JsonConverter;
using System.Text.Json.Serialization;

namespace Wes.DbModel
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [SugarTable("sys_user", "用户信息", IsDisabledUpdateAll = true)]
    public class SysUserModel
    {
        /// <summary>
        /// 用户ID
        /// <summary>
        [SugarColumn(ColumnName = "user_id", IsPrimaryKey = true, Length = 20, ColumnDescription = "用户ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long UserId { get; set; }

        /// <summary>
        /// 部门ID
        /// <summary>
        [SugarColumn(ColumnName = "dept_id", IsNullable = true, Length = 20, ColumnDescription = "部门ID")]
        [JsonConverter(typeof(LongToStringConverter))]
        public long? DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [ExportFieldAttr(FieldName = "部门名称", SortBy = 20)]
        public string DeptName
        {
            get
            {
                return Dept?.DeptName ?? "";
            }
        }

        /// <summary>
        /// 部门
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(DeptId))]
        public SysDeptModel Dept { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [SugarColumn(ColumnName = "user_name", Length = 30, ColumnDescription = "用户名称")]
        [ExportFieldAttr(FieldName = "用户名称", SortBy = 10)]
        public string UserName { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [SugarColumn(ColumnName = "account", Length = 100, ColumnDescription = "用户账户")]
        [ExportFieldAttr(FieldName = "用户账户", SortBy = 12)]
        public string Account { get; set; }

        /// <summary>
        /// 用户类型（00系统用户）
        /// </summary>
        [SugarColumn(ColumnName = "user_type", IsNullable = true, Length = 2, ColumnDescription = "用户类型（00系统用户）")]
        public string UserType { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        [SugarColumn(ColumnName = "email", IsNullable = true, Length = 50, ColumnDescription = "用户邮箱")]
        [ExportFieldAttr(FieldName = "用户邮箱", SortBy = 14)]
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// <summary>
        [SugarColumn(ColumnName = "phonenumber", IsNullable = true, Length = 11, ColumnDescription = "手机号码")]
        public string Phonenumber { get; set; }

        /// <summary>
        /// 用户性别（0男 1女 2未知）
        /// <summary>
        [SugarColumn(ColumnName = "sex", IsNullable = true, Length = 1, ColumnDescription = "用户性别（0男 1女 2未知）")]
        public string Sex { get; set; }

        /// <summary>
        /// 头像地址
        /// <summary>
        [SugarColumn(ColumnName = "avatar", IsNullable = true, Length = 100, ColumnDescription = "头像地址")]
        public string Avatar { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(ColumnName = "password", IsNullable = true, Length = 100, ColumnDescription = "密码")]
        [JsonIgnoreAttribute]
        public string Password { get; set; }

        /// <summary>
        /// 帐号状态（0正常 1停用）
        /// <summary>
        [SugarColumn(ColumnName = "status", IsNullable = true, Length = 1, ColumnDescription = "帐号状态（0正常 1停用）")]
        public string Status { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 1代表删除）
        /// <summary>
        [SugarColumn(ColumnName = "is_del", IsNullable = true, Length = 1, ColumnDescription = "删除标志（0代表存在 1代表删除）")]
        public int? IsDel { get; set; }

        /// <summary>
        /// 最后登录IP
        /// <summary>
        [SugarColumn(ColumnName = "login_ip", IsNullable = true, Length = 128, ColumnDescription = "最后登录IP")]
        public string LoginIp { get; set; }

        /// <summary>
        /// 最后登录时间
        /// <summary>
        [SugarColumn(ColumnName = "login_date", IsNullable = true, ColumnDescription = "最后登录时间")]
        public DateTime? LoginDate { get; set; }

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
        /// 备注
        /// <summary>
        [SugarColumn(ColumnName = "remark", IsNullable = true, Length = 500, ColumnDescription = "备注")]
        public string Remark { get; set; }

    }
}
