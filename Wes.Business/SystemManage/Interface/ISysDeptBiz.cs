using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysDeptBiz
    {
        #region 部门操作

        public ResultData<List<SysDeptModel>> GetList(DeptParam param);

        public ResultData<List<SysDeptModel>> GetAll();

        public ResultData<SysDeptModel> GetById(long id);

        public ReturnData Save(SysDeptModel dic);

        public ReturnData Delete(string ids);

        #endregion

        public ResultData<List<SysDeptModel>> GetExcludeById(long id);

        public ResultData<RoleTreeInfo> GetRoleDept(long roleId);
    }
}
