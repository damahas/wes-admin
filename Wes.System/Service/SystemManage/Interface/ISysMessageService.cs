using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysMessageService
    {
        public List<SysMessageModel> GetList(ParamData<MessageParam> param, out int total);

        public List<SysMessageModel> GetAll();

        public SysMessageModel GetById(long id);

        public bool Save(SysMessageModel model);

        public bool Delete(List<long> ids);

        public bool ReadAll();

        public bool Read(List<long> ids);
    }
}
