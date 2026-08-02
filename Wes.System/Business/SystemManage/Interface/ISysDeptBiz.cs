using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysDeptBiz
    {
        #region 部门操作

        public ResultData<List<SysDeptEntity>> GetList(DeptParam param);

        public ResultData<List<SysDeptEntity>> GetAll();

        public ResultData<SysDeptEntity> GetById(long id);

        public ReturnData Save(SysDeptEntity dic);

        public ReturnData Delete(string ids);

        #endregion

        public ResultData<List<SysDeptEntity>> GetExcludeById(long id);

        public ResultData<RoleTreeInfo> GetRoleDept(long roleId);
    }
}
