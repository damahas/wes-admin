using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysPostService
    {
        public List<SysPostEntity> GetByUserId(long userId);

        #region 角色基本操作

        public SysPostEntity GetById(long postId);

        public SysPostEntity GetByPostCode(string postCode);

        public List<SysPostEntity> GetList(ParamData<PostParam> param, out int total);

        public List<SysPostEntity> GetAll();

        public bool Save(SysPostEntity post);

        public bool Delete(List<long> ids);

        #endregion
    }
}
