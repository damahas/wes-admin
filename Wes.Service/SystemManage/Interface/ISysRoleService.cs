using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysRoleService
    {

        public SysRoleModel GetByRoleKey(string roleKey);

        public List<SysRoleModel> GetByUserId(long userId);

        #region 角色操作

        public SysRoleModel GetById(long roleId);

        public List<SysRoleModel> GetList(ParamData<RoleParam> param, out int total);

        public List<SysRoleModel> GetAll();

        public bool Save(SysRoleModel role);

        public bool Delete(List<long> ids);

        #endregion

        #region 角色用户操作

        public bool SaveRoleUser(long roleId, List<long> userIds);

        public bool DeleteRoleUser(long roleId, List<long> userIds);

        #endregion

        #region 角色部门操作（数据权限）

        public List<long> GetLeafRoleDeptIds(long roleId);

        public List<long> GetRoleDeptIds(long roleId);

        public bool SaveRoleDept(List<SysRoleDeptModel> roleDepts);

        public bool DeleteRoleDept(long roleId, List<long> deptIds);

        #endregion

        #region 角色菜单操作

        public List<long> GetLeafRoleMenuIds(long roleId);

        public List<long> GetRoleMenuIds(long roleId);

        public bool SaveRoleMenu(List<SysRoleMenuModel> roleMenus);

        public bool DeleteRoleMenu(long roleId, List<long> menuIds);

        #endregion
    }
}
