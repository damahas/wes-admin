using SqlSugar;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysRoleService : Repository<SysRoleEntity>, ISysRoleService
    {
        public SysRoleService(ISqlSugarClient db) : base(db) { }

        /// <summary>
        /// 通过roleKey获取角色
        /// </summary>
        /// <param name="roleKey"></param>
        /// <returns></returns>
        public SysRoleEntity GetByRoleKey(string roleKey)
        {
            return GetFirst(p => p.RoleKey == roleKey && p.IsDel == 0);
        }

        /// <summary>
        /// 通过userId获取用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SysRoleEntity> GetByUserId(long userId)
        {
            return this.Context.Queryable<SysRoleEntity>()
                .LeftJoin<SysUserRoleEntity>((o, p) => o.RoleId == p.RoleId)
                .Where((o, p) => o.IsDel == 0 && p.UserId == userId && o.Status == "0").ToList();
        }

        #region 角色基本操作

        public SysRoleEntity GetById(long roleId)
        {
            return GetFirst(p => p.RoleId == roleId && p.IsDel == 0);
        }

        public List<SysRoleEntity> GetList(ParamData<RoleParam> param, out int total)
        {
            Expressionable<SysRoleEntity> express = Expressionable.Create<SysRoleEntity>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.RoleName))
                {
                    express.And(p => p.RoleName.Contains(param.Params.RoleName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.RoleKey))
                {
                    express.And(p => p.RoleKey.Contains(param.Params.RoleKey.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Status))
                {
                    express.And(p => p.Status == param.Params.Status);
                }
                if (param.Params.BeginTime != null)
                {
                    express.And(p => p.CreateTime >= param.Params.BeginTime);
                }
                if (param.Params.EndTime != null)
                {
                    express.And(p => p.CreateTime <= param.Params.EndTime);
                }
            }
            total = 0;
            var query = Context.Queryable<SysRoleEntity>().Where(express.ToExpression()).OrderBy(p => p.RoleSort, OrderByType.Desc);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysRoleEntity> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public bool Save(SysRoleEntity role)
        {
            if (role.RoleId > 0)
            {
                role.UpdateTime = DateTime.Now;
                role.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(role);
            }
            role.IsDel = 0;
            role.CreateTime = DateTime.Now;
            role.CreateBy = GlobalContext.CurrentUser.Account;
            role.RoleId = Context.Insertable(role).ExecuteReturnSnowflakeId();
            return true;
        }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysRoleEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.RoleId)).ExecuteCommand() > 0;
        }

        #endregion

        #region 角色用户操作

        public bool SaveRoleUser(long roleId, List<long> userIds)
        {
            var roleUsers = userIds.Select(p =>
            {
                return new SysUserRoleEntity()
                {
                    RoleId = roleId,
                    UserId = p
                };
            }).ToList();
            return this.Context.Insertable(roleUsers).ExecuteReturnSnowflakeIdList().Count > 0;
        }

        public bool DeleteRoleUser(long roleId, List<long> userIds)
        {
            return Context.Deleteable<SysUserRoleEntity>(p => p.RoleId == roleId && userIds.Contains(p.UserId)).ExecuteCommand() > 0;
        }

        #endregion

        #region 角色部门操作（数据权限）

        public List<long> GetLeafRoleDeptIds(long roleId)
        {
            return this.Context.SqlQueryable<SysRoleDeptEntity>($@"SELECT
	                        srd.dept_id
                        FROM
	                        sys_role_dept srd
	                        LEFT JOIN sys_dept d ON srd.dept_id = d.dept_id 
                        WHERE
	                        d.is_del = 0 
	                        AND d.dept_id NOT IN ( SELECT DISTINCT parent_id FROM sys_dept WHERE is_del = 0 ) 
	                        AND srd.role_id = {roleId}").Select(p => p.DeptId).ToList() ?? new List<long>();
        }

        public List<long> GetRoleDeptIds(long roleId)
        {
            return this.Context.Queryable<SysRoleDeptEntity>()
                .Where(p => p.RoleId == roleId)?.Select(p => p.DeptId).ToList() ?? new List<long>();
        }

        public bool SaveRoleDept(List<SysRoleDeptEntity> roleDepts)
        {
            return this.Context.Insertable(roleDepts).ExecuteReturnSnowflakeIdList().Count > 0;
        }

        public bool DeleteRoleDept(long roleId, List<long> deptIds)
        {
            Expressionable<SysRoleDeptEntity> express = Expressionable.Create<SysRoleDeptEntity>();
            express.And(p => p.RoleId == roleId);
            if (deptIds != null && deptIds.Count > 0)
            {
                express.And(p => !deptIds.Contains(p.DeptId));
            }
            return Context.Deleteable<SysRoleDeptEntity>().Where(express.ToExpression()).ExecuteCommand() > 0;
        }

        #endregion

        #region 角色菜单操作

        public List<long> GetLeafRoleMenuIds(long roleId)
        {
            return this.Context.SqlQueryable<SysRoleMenuEntity>($@"SELECT
	                            srm.menu_id 
                            FROM
	                            sys_role_menu srm
	                            LEFT JOIN sys_menu m ON srm.menu_id = m.menu_id 
                            WHERE
	                            m.is_del = 0 
	                            AND m.menu_id NOT IN ( SELECT DISTINCT parent_id FROM sys_menu WHERE is_del = 0 ) 
	                            AND srm.role_id = {roleId}").Select(p => p.MenuId).ToList() ?? new List<long>();
        }

        public List<long> GetRoleMenuIds(long roleId)
        {
            return this.Context.Queryable<SysRoleMenuEntity>()
                        .Where(p => p.RoleId == roleId)?.Select(p => p.MenuId).ToList() ?? new List<long>();
        }

        public bool SaveRoleMenu(List<SysRoleMenuEntity> roleMenus)
        {
            return this.Context.Insertable(roleMenus).ExecuteReturnSnowflakeIdList().Count > 0;
        }

        public bool DeleteRoleMenu(long roleId, List<long> menuIds)
        {
            Expressionable<SysRoleMenuEntity> express = Expressionable.Create<SysRoleMenuEntity>();
            express.And(p => p.RoleId == roleId);
            if (menuIds != null && menuIds.Count > 0)
            {
                express.And(p => !menuIds.Contains(p.MenuId));
            }
            return Context.Deleteable<SysRoleMenuEntity>().Where(express.ToExpression()).ExecuteCommand() > 0;
        }

        #endregion
    }
}