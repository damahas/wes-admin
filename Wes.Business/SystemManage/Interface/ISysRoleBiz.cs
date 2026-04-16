using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysRoleBiz
    {
        #region 权限操作

        public RowData<SysRoleModel> GetList(ParamData<RoleParam> param);

        public ResultData<SysRoleModel> GetById(long id);

        public ReturnData Save(RoleModel role);

        public ReturnData Delete(string ids);

        public byte[] Export(ParamData<RoleParam> param);

        public ReturnData ChangeStatus(long roleId, string status);

        #endregion

        #region 权限关联人员

        public ReturnData SaveRoleUser(long roleId, string userIds);

        public ReturnData DeleteRoleUser(long roleId, string userIds);

        #endregion

        #region 权限关联数据权限

        public ReturnData SaveRoleDept(RoleDeptModel roleDeptModel);

        #endregion
    }
}

