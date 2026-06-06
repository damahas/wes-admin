using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysUserService
    {
        public SysUserModel Login(string userName, string password);

        public List<SysUserModel> GetListByRoleId(ParamData<RoleUserParam> param, out int total);

        #region 用户操作

        public SysUserModel GetById(long userId);

        public SysUserModel GetByAccout(string account);

        public List<SysUserModel> GetByRoleId(long roleId);

        public List<SysUserModel> GetByDeptId(long deptId);

        public long GetLeaderIdByUserId(long userId);

        public long GetLeaderIdByAccount(string account);

        public SysUserModel GetByUserName(string userName);

        public List<SysUserModel> GetList(ParamData<UserParam> param, out int total);

        public List<SysUserModel> GetList(List<long> userIds);

        public bool Save(SysUserModel user);

        public bool Delete(List<long> ids);

        #endregion

        #region 用户角色操作

        public List<SysUserRoleModel> GetUserRole(long userId);

        public bool SaveUserRole(List<SysUserRoleModel> userRoles);

        public bool DeleteUserRole(long userId, List<long> roleIds);

        #endregion

        #region 用户岗位操作

        public List<SysUserPostModel> GetUserPost(long userId);

        public bool SaveUserPost(List<SysUserPostModel> userPosts);

        public bool DeleteUserPost(long userId, List<long> postIds);

        #endregion

        #region Token操作

        public SysUserModel GetByToken(string token);

        public SysTokenModel GetToken(string token);

        public SysTokenModel SaveToken(SysTokenModel tokenModel);

        public bool InvalidToken(string token);

        public SysTokenModel InvalidToken(long tokenId);

        public List<OnlineInfo> GetOnlineList(ParamData<OnlineParam> param, out int total);

        #endregion

    }
}
