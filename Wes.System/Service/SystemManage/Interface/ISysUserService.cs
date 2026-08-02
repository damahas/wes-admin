using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysUserService
    {
        public SysUserEntity Login(string userName, string password);

        public List<SysUserEntity> GetListByRoleId(ParamData<RoleUserParam> param, out int total);

        #region 用户操作

        public SysUserEntity GetById(long userId);

        public SysUserEntity GetByAccout(string account);

        public List<SysUserEntity> GetByRoleId(long roleId);

        public List<SysUserEntity> GetByDeptId(long deptId);

        public long GetLeaderIdByUserId(long userId);

        public long GetLeaderIdByAccount(string account);

        public SysUserEntity GetByUserName(string userName);

        public List<SysUserEntity> GetList(ParamData<UserParam> param, out int total);

        public List<SysUserEntity> GetList(List<long> userIds);

        public bool Save(SysUserEntity user);

        public bool Delete(List<long> ids);

        #endregion

        #region 用户角色操作

        public List<SysUserRoleEntity> GetUserRole(long userId);

        public bool SaveUserRole(List<SysUserRoleEntity> userRoles);

        public bool DeleteUserRole(long userId, List<long> roleIds);

        #endregion

        #region 用户岗位操作

        public List<SysUserPostEntity> GetUserPost(long userId);

        public bool SaveUserPost(List<SysUserPostEntity> userPosts);

        public bool DeleteUserPost(long userId, List<long> postIds);

        #endregion

        #region Token操作

        public SysUserEntity GetByToken(string token);

        public SysTokenEntity GetToken(string token);

        public SysTokenEntity SaveToken(SysTokenEntity tokenModel);

        public bool InvalidToken(string token);

        public SysTokenEntity InvalidToken(long tokenId);

        public List<OnlineInfo> GetOnlineList(ParamData<OnlineParam> param, out int total);

        #endregion

    }
}
