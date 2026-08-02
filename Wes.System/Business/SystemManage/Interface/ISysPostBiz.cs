using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysPostBiz
    {
        #region 岗位操作

        public RowData<SysPostEntity> GetList(ParamData<PostParam> param);

        public ResultData<SysPostEntity> GetById(long id);

        public ReturnData Save(SysPostEntity post);

        public ReturnData Delete(string ids);

        public byte[] Export(ParamData<PostParam> param);

        #endregion
    }
}
