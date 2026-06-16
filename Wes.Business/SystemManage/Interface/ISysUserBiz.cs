using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysUserBiz
    {
        #region 登录登出

        public LoginData Login(LoginInfo login);

        public ReturnData Logout(string token);

        #endregion

        public bool TryGetUser(string token, out UserInfo userInfo);

        #region 用户操作

        public RowData<SysUserModel> GetList(ParamData<UserParam> param);

        public UserReturnInfo GetById(long id);

        public ReturnData Save(UserModel user);

        public ReturnData Delete(string ids);

        public byte[] Export(ParamData<UserParam> param);

        public ReturnData ChangeStatus(long userId, string status);

        #endregion

        #region 角色用户

        public RowData<SysUserModel> GetRoleUserList(ParamData<RoleUserParam> param);

        public RowData<SysUserModel> GetRoleNoExistUserList(ParamData<RoleUserParam> param);

        public ResultData<object> GetUserRole(long id);

        public ReturnData SaveUserRole(long userId, string roleIds);

        #endregion

        #region 密码

        public ReturnData ResetUserPwd(UserPwdParam userPwdParam);

        public ReturnData UpdateCurrentUserPwd(string oldPassword, string newPassword);

        #endregion

        #region 个人中心

        public ReturnData SaveCurrentUser(UserInfo userInfo);

        #endregion
    }
}
