using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysPostBiz
    {
        #region 岗位操作

        public RowData<SysPostModel> GetList(ParamData<PostParam> param);

        public ResultData<SysPostModel> GetById(long id);

        public ReturnData Save(SysPostModel post);

        public ReturnData Delete(string ids);

        public byte[] Export(ParamData<PostParam> param);

        #endregion
    }
}
