using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysDicDataBiz
    {
        public RowData<SysDicDataModel> GetList(ParamData<DicDataParam> param);

        public ResultData<SysDicDataModel> GetById(long id);

        public ReturnData Save(SysDicDataModel dic);

        public ReturnData Delete(string ids);

        public List<SysDicDataModel> GetListByDicType(string dicType);

        public byte[] Export(ParamData<DicDataParam> param);
    }
}

