using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysMessageService
    {
        public List<SysMessageEntity> GetList(ParamData<MessageParam> param, out int total);

        public List<SysMessageEntity> GetAll();

        public SysMessageEntity GetById(long id);

        public bool Save(SysMessageEntity model);

        public bool Delete(List<long> ids);

        public bool ReadAll();

        public bool Read(List<long> ids);
    }
}
