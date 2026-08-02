using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysDeptService
    {
        public SysDeptEntity GetById(long deptId);

        public List<SysDeptEntity> GetList(DeptParam param);

        public List<SysDeptEntity> GetAll();

        public bool Save(SysDeptEntity dic);

        public bool Delete(List<long> ids);

        public List<SysDeptEntity> GetExcludeById(long id);
    }
}

