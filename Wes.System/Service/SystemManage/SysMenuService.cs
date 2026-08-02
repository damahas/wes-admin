using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using Wes.Entity;
using Wes.Utils;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysMenuService : Repository<SysMenuEntity>, ISysMenuService
    {
        public SysMenuService(ISqlSugarClient db) : base(db) { }

        public SysMenuEntity GetById(long id)
        {
            return GetFirst(p => p.MenuId == id && p.IsDel == 0);
        }

        public List<SysMenuEntity> GetList(MenuParam param)
        {
            Expressionable<SysMenuEntity> express = Expressionable.Create<SysMenuEntity>();
            express.And(p => p.IsDel == 0);
            if (param != null)
            {
                if (!string.IsNullOrWhiteSpace(param.MenuName))
                {
                    express.And(p => p.MenuName.Contains(param.MenuName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Status))
                {
                    express.And(p => p.Status == param.Status);
                }
                if (param.IsNoBtn)
                {
                    express.And(p => p.MenuType != "F");
                }
            }
            return Context.Queryable<SysMenuEntity>().Where(express.ToExpression()).OrderBy(p => p.OrderNum).ToList();
        }

        public bool Save(SysMenuEntity menu)
        {
            if (menu.MenuId > 0)
            {
                menu.UpdateTime = DateTime.Now;
                menu.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(menu);
            }
            menu.IsDel = 0;
            menu.CreateTime = DateTime.Now;
            menu.CreateBy = GlobalContext.CurrentUser.Account;
            return InsertReturnSnowflakeId(menu) > 0;
        }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysMenuEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.MenuId)).ExecuteCommand() > 0;
        }

        public List<SysMenuEntity> GetByUserId(long userId, string menuType)
        {
            return this.Context.Queryable<SysMenuEntity, SysRoleMenuEntity, SysRoleEntity, SysUserRoleEntity>((o, p, q, u) => new JoinQueryInfos(
                 JoinType.Inner, o.MenuId == p.MenuId,
                 JoinType.Inner, p.RoleId == q.RoleId,
                 JoinType.Inner, q.RoleId == u.RoleId
                 ))
                .Where((o, p, q, u) => q.IsDel == 0
                && q.Status == "0"
                && u.UserId == userId
                && o.MenuType == menuType
                && o.IsDel == 0
                && o.SystemId == GlobalContext.AppSettings.SystemId)
                .Select(o => o).ToList();
        }

        public List<SysMenuEntity> GetByMenuType(string menuType)
        {
            return GetList(p => p.MenuType == menuType && p.IsDel == 0 && p.SystemId == GlobalContext.AppSettings.SystemId);
        }

        public List<string> GetPermissionsByUserId(long userId)
        {
            string sql = $@"SELECT
	                            m.perms 
                            FROM
	                            sys_menu m
	                            LEFT JOIN sys_role_menu rm ON m.menu_id = rm.menu_id
	                            LEFT JOIN sys_role r ON rm.role_id = r.role_id
	                            LEFT JOIN sys_user_role ur ON rm.role_id = ur.role_id 
                            WHERE
	                            m.is_del = 0
	                            AND r.is_del = 0 
	                            AND m.perms IS NOT NULL 
	                            AND m.perms != '' 
	                            AND ur.user_id = {userId}";
            return Context.SqlQueryable<SysMenuEntity>(sql).Select(p => p.Perms).ToList();
        }
    }
}

