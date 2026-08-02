using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysMessageBiz
    {
        public RowData<SysMessageEntity> GetList(ParamData<MessageParam> param);

        public ResultData<List<SysMessageEntity>> GetAll();

        public ResultData<SysMessageEntity> GetById(long id);

        public ReturnData Save(SysMessageEntity model);

        public ReturnData Delete(string ids);

        public ReturnData ReadAll();

        public ReturnData Read(long id);

        public ReturnData Read(List<long> ids);
    }
}
