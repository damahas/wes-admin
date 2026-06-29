using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysMessageBiz
    {
        public RowData<SysMessageModel> GetList(ParamData<MessageParam> param);

        public ResultData<List<SysMessageModel>> GetAll();

        public ResultData<SysMessageModel> GetById(long id);

        public ReturnData Save(SysMessageModel model);

        public ReturnData Delete(string ids);

        public ReturnData ReadAll();

        public ReturnData Read(long id);

        public ReturnData Read(List<long> ids);
    }
}
