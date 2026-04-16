using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysPostService
    {
        public List<SysPostModel> GetByUserId(long userId);

        #region 角色基本操作

        public SysPostModel GetById(long postId);

        public SysPostModel GetByPostCode(string postCode);

        public List<SysPostModel> GetList(ParamData<PostParam> param, out int total);

        public List<SysPostModel> GetAll();

        public bool Save(SysPostModel post);

        public bool Delete(List<long> ids);

        #endregion
    }
}
