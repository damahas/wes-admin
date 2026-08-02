using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysRoleService
    {

        public SysRoleEntity GetByRoleKey(string roleKey);

        public List<SysRoleEntity> GetByUserId(long userId);

        #region 角色操作

        public SysRoleEntity GetById(long roleId);

        public List<SysRoleEntity> GetList(ParamData<RoleParam> param, out int total);

        public List<SysRoleEntity> GetAll();

        public bool Save(SysRoleEntity role);

        public bool Delete(List<long> ids);

        #endregion

        #region 角色用户操作

        public bool SaveRoleUser(long roleId, List<long> userIds);

        public bool DeleteRoleUser(long roleId, List<long> userIds);

        #endregion

        #region 角色部门操作（数据权限）

        public List<long> GetLeafRoleDeptIds(long roleId);

        public List<long> GetRoleDeptIds(long roleId);

        public bool SaveRoleDept(List<SysRoleDeptEntity> roleDepts);

        public bool DeleteRoleDept(long roleId, List<long> deptIds);

        #endregion

        #region 角色菜单操作

        public List<long> GetLeafRoleMenuIds(long roleId);

        public List<long> GetRoleMenuIds(long roleId);

        public bool SaveRoleMenu(List<SysRoleMenuEntity> roleMenus);

        public bool DeleteRoleMenu(long roleId, List<long> menuIds);

        #endregion
    }
}
