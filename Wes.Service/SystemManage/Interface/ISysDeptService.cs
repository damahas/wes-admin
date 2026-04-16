using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysDeptService
    {
        public SysDeptModel GetById(long deptId);

        public List<SysDeptModel> GetList(DeptParam param);

        public List<SysDeptModel> GetAll();

        public bool Save(SysDeptModel dic);

        public bool Delete(List<long> ids);

        public List<SysDeptModel> GetExcludeById(long id);
    }
}

