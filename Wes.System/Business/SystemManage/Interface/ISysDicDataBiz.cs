using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysDicDataBiz
    {
        public RowData<SysDicDataEntity> GetList(ParamData<DicDataParam> param);

        public ResultData<SysDicDataEntity> GetById(long id);

        public ReturnData Save(SysDicDataEntity dic);

        public ReturnData Delete(string ids);

        public List<SysDicDataEntity> GetListByDicType(string dicType);

        public byte[] Export(ParamData<DicDataParam> param);
    }
}

