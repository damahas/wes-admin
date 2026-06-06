using System;
using System.Collections.Generic;
using System.Text;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Security;
using Wes.Utils.Extension;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.Utils.Cache;
using Wes.Utils;
using System.Linq;
using Wes.ViewModel.SystemManage;
using Wes.Utils.Hepler;

namespace Wes.Business
{
    public class SysUserBiz : ISysUserBiz
    {
        private ISysUserService _sysUserService;
        private ISysRoleService _sysRoleService;
        private ISysDeptService _sysDeptService;
        private ISysPostService _sysPostService;
        private ISysLoginLogService _sysLoginLogService;
        private ISysDicDataService _sysDicDataService;
        private ISysMenuService _sysMenuService;
        private ISysConfigBiz _sysConfigBiz;

        public SysUserBiz(
            ISysUserService sysUserService,
            ISysRoleService sysRoleService,
            ISysDeptService sysDeptService,
            ISysPostService sysPostService,
            ISysLoginLogService sysLoginLogService,
            ISysDicDataService sysDicDataService,
            ISysMenuService sysMenuService,
            ISysConfigBiz sysConfigBiz
            )
        {
            _sysUserService = sysUserService;
            _sysRoleService = sysRoleService;
            _sysDeptService = sysDeptService;
            _sysPostService = sysPostService;
            _sysLoginLogService = sysLoginLogService;
            _sysDicDataService = sysDicDataService;
            _sysMenuService = sysMenuService;
            _sysConfigBiz = sysConfigBiz;
        }

        #region 登录登出

        public LoginData Login(LoginInfo login)
        {
            if (GlobalContext.LicenseModel == null || GlobalContext.LicenseModel.ExpireTime < DateTime.Now)
            {
                return new LoginData(402, "许可证已过期，为了不影响您的使用，请联系服务专员处理");
            }

            if ("true".Equals(_sysConfigBiz.GetByConfigKey("sys.login.isCaptchaOn")))
            {
                decimal positionX = CacheFactory.Cache.GetCache<decimal>($"{CacheKey.CaptchaVaildImg}{login.code}");

                if (positionX == 0)
                {
                    SaveLoginLog(login.UserName, "1", "验证码已过期");
                    return new LoginData(408, "验证码已过期");
                }
                CacheFactory.Cache.RemoveCache($"{CacheKey.CaptchaVaildImg}{login.code}");
                if (login.positionX < positionX - 0.01m || login.positionX > positionX + 0.01m)
                {
                    SaveLoginLog(login.UserName, "1", "验证码验证失败");
                    return new LoginData(500, "验证码验证失败");
                }
            }

            login.Password = AESUtils.Encrypt(MD5Utils.Encrypt(login.Password));
            var user = _sysUserService.Login(login.UserName, login.Password);
            if (user == null)
            {
                SaveLoginLog(login.UserName, "1", "账号密码错误或找不到用户");
                return new LoginData(500, "账号密码错误或找不到用户");
            }
            if (user.Status == "1")
            {
                SaveLoginLog(login.UserName, "1", "账号已停用");
                return new LoginData(500, "账号已停用");
            }
            var token = JWTUtils.GenerateToken(user.UserId.ToString());
            //var token = Guid.NewGuid().ToString("N");
            var expireTime = DateTime.Now.AddHours(GlobalContext.JwtSettings.Expires);
            var tokenSaveResult = _sysUserService.SaveToken(new SysTokenModel()
            {
                UserId = user.UserId,
                Token = token,
                CreateTime = DateTime.Now,
                Status = 0,
                LoginIp = NetHepler.Ip,
                Os = NetHepler.Os,
                Browser = NetHepler.Browser,
                ExpirationTime = expireTime,
                Source = "web"
            });
            SaveLoginLog(login.UserName, "0", "登录成功");
            CacheFactory.Cache.SetCache($"{CacheKey.AccessToken}{token}", GetUserInfo(token), expireTime);
            return new LoginData(token);
        }

        //public ResultData<object> Login(string userName, string password)
        //{

        //    password = AESUtils.Encrypt(MD5Utils.Encrypt(password));
        //    var user = _sysUserService.Login(userName, password);
        //    if (user == null)
        //    {
        //        SaveLoginLog(userName, "1", "账号密码错误或找不到用户");
        //        return new ResultData<object>(500, "账号密码错误或找不到用户");
        //    }
        //    if (user.Status == "1")
        //    {
        //        SaveLoginLog(userName, "1", "账号已停用");
        //        return new ResultData<object>(500, "账号已停用");
        //    }
        //    var token = Guid.NewGuid().ToString("N");
        //    var expireTime = DateTime.Now.AddDays(7);
        //    var tokenSaveResult = _sysUserService.SaveToken(new SysTokenModel()
        //    {
        //        UserId = user.UserId,
        //        Token = token,
        //        CreateTime = DateTime.Now,
        //        Status = 0,
        //        LoginIp = NetHepler.Ip,
        //        Os = NetHepler.Os,
        //        Browser = NetHepler.Browser,
        //        ExpirationTime = expireTime,
        //        Source = "web"
        //    });
        //    SaveLoginLog(userName, "0", "登录成功");
        //    CacheFactory.Cache.SetCache($"{CacheKey.AccessToken}{token}", GetUserInfo(token), expireTime);
        //    return new ResultData<object>(new
        //    {
        //        user = user,
        //        token = token
        //    });
        //}

        public ReturnData Logout(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new ReturnData();
            }
            _sysUserService.InvalidToken(token);
            SaveLoginLog(CacheFactory.Cache.GetCache<UserInfo>($"{CacheKey.AccessToken}{token}").UserName, "0", "退出成功");
            CacheFactory.Cache.RemoveCache($"{CacheKey.AccessToken}{token}");
            return new ReturnData();
        }

        #endregion

        public bool TryGetUser(string token, out UserInfo userInfo)
        {
            userInfo = CacheFactory.Cache.GetCache<UserInfo>($"{CacheKey.AccessToken}{token}");
            if (userInfo != null)
            {
                return true;
            }
            userInfo = GetUserInfo(token);
            if (userInfo != null)
            {
                CacheFactory.Cache.SetCache($"{CacheKey.AccessToken}{token}", userInfo, userInfo.ExpirationTime);
                return true;
            }
            return false;
        }

        #region 用户操作

        public RowData<SysUserModel> GetList(ParamData<UserParam> param)
        {
            int total = 0;
            RowData<SysUserModel> result = new RowData<SysUserModel>(_sysUserService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public UserReturnInfo GetById(long id)
        {
            UserReturnInfo reslut = new UserReturnInfo() { };
            if (id > 0)
            {
                reslut.Data = _sysUserService.GetById(id).ToEntityCopy<SysUserModel, UserModel>();
                //if (reslut.Data == null)
                //{
                //    return new UserReturnInfo(500, "找不到用户！");
                //}
                reslut.Data.RoleIds = _sysUserService.GetUserRole(id)?.Select(p => p.RoleId).ToList() ?? new List<long>();
                reslut.Data.PostIds = _sysUserService.GetUserPost(id)?.Select(p => p.PostId).ToList() ?? new List<long>();
            }

            reslut.Roles = _sysRoleService.GetAll();
            reslut.Posts = _sysPostService.GetAll();
            return reslut;
        }

        public ReturnData Save(UserModel user)
        {
            var exist = _sysUserService.GetByUserName(user.Account);
            if (exist != null && exist.UserId != user.UserId)
            {
                return new ReturnData(500, $"已存在 {user.Account}，用户账户不能重复！");
            }
            exist = _sysUserService.GetById(user.UserId);
            if (exist != null)
            {
                user.Password = exist.Password;
            }
            else
            {
                user.Password = AESUtils.Encrypt(MD5Utils.Encrypt(user.Password));
            }
            _sysUserService.Save(user);
            // 处理角色保存
            var roles = _sysUserService.GetUserRole(user.UserId)?.Select(p => p.RoleId).ToList() ?? new List<long>();
            var newRoles = user.RoleIds.Except(roles);
            if (newRoles.Any())
            {
                _sysUserService.SaveUserRole(newRoles.Select(p =>
                {
                    return new SysUserRoleModel()
                    {
                        UserId = user.UserId,
                        RoleId = p
                    };
                }).ToList());
            }
            _sysUserService.DeleteUserRole(user.UserId, user.RoleIds);
            // 处理岗位保存
            var posts = _sysUserService.GetUserPost(user.UserId)?.Select(p => p.PostId).ToList() ?? new List<long>();
            var newPosts = user.PostIds.Except(posts);
            if (newPosts.Any())
            {
                _sysUserService.SaveUserPost(newPosts.Select(p =>
                {
                    return new SysUserPostModel()
                    {
                        UserId = user.UserId,
                        PostId = p
                    };
                }).ToList());
            }
            _sysUserService.DeleteUserPost(user.UserId, user.PostIds);
            return new ReturnData();
        }

        public ReturnData Delete(string ids)
        {
            var userIds = ids.ToLongList();
            if (userIds == null || userIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysUserService.Delete(userIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public byte[] Export(ParamData<UserParam> param)
        {
            int total = 0;
            param.PageSize = 0;
            return NPOIHepler.ExportExcel(_sysUserService.GetList(param, out total), _sysDicDataService);
        }

        public ReturnData ChangeStatus(long userId, string status)
        {
            var user = _sysUserService.GetById(userId);
            user.Status = status;
            if (_sysUserService.Save(user))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "状态保存失败！");
        }

        #endregion

        #region 角色用户

        public RowData<SysUserModel> GetRoleUserList(ParamData<RoleUserParam> param)
        {
            int total = 0;
            if (param.Params == null) param.Params = new RoleUserParam();
            param.Params.IsNoExist = false;
            RowData<SysUserModel> result = new RowData<SysUserModel>(_sysUserService.GetListByRoleId(param, out total));
            result.total = total;
            return result;
        }

        public RowData<SysUserModel> GetRoleNoExistUserList(ParamData<RoleUserParam> param)
        {
            int total = 0;
            if (param.Params == null) param.Params = new RoleUserParam();
            param.Params.IsNoExist = true;
            RowData<SysUserModel> result = new RowData<SysUserModel>(_sysUserService.GetListByRoleId(param, out total));
            result.total = total;
            return result;
        }

        public ResultData<object> GetUserRole(long id)
        {
            var user = _sysUserService.GetById(id).ToEntityCopy<SysUserModel, UserModel>();
            user.RoleIds = _sysUserService.GetUserRole(id)?.Select(p => p.RoleId).ToList() ?? new List<long>();
            return new ResultData<object>(new
            {
                Roles = _sysRoleService.GetAll(),
                User = user
            });
        }

        public ReturnData SaveUserRole(long userId, string roleIds)
        {
            if (userId == 0) return new ReturnData(500, "用户id有误！");
            List<long> roleSaveIds = new List<long>();
            if (!string.IsNullOrWhiteSpace(roleIds))
            {
                roleSaveIds = roleIds.ToLongList();
            }
            var roles = _sysUserService.GetUserRole(userId)?.Select(p => p.RoleId).ToList() ?? new List<long>();
            var newRoles = roleSaveIds.Except(roles);
            if (newRoles.Any())
            {
                _sysUserService.SaveUserRole(newRoles.Select(p =>
                {
                    return new SysUserRoleModel()
                    {
                        UserId = userId,
                        RoleId = p
                    };
                }).ToList());
            }
            _sysUserService.DeleteUserRole(userId, roleSaveIds);
            return new ReturnData();
        }

        #endregion

        #region 密码

        public ReturnData ResetUserPwd(UserPwdParam userPwdParam)
        {
            //TODO 重置密码需要验证
            var user = _sysUserService.GetById(userPwdParam.userId);
            if (user == null)
            {
                return new ReturnData(500, "找不到用户！");
            }
            user.Password = AESUtils.Encrypt(MD5Utils.Encrypt(userPwdParam.Password));
            _sysUserService.Save(user);
            return new ReturnData();
        }

        public ReturnData UpdateCurrentUserPwd(string oldPassword, string newPassword)
        {
            var user = _sysUserService.GetById(GlobalContext.CurrentUser.UserId);
            if (user == null)
            {
                return new ReturnData(500, "找不到用户！");
            }
            var oldPwd = AESUtils.Encrypt(MD5Utils.Encrypt(oldPassword));
            if (user.Password != oldPwd)
            {
                return new ReturnData(500, "旧密码输入错误！");
            }
            user.Password = AESUtils.Encrypt(MD5Utils.Encrypt(newPassword));
            _sysUserService.Save(user);
            return new ReturnData();
        }

        #endregion

        #region 个人中心

        public ReturnData SaveCurrentUser(UserInfo userInfo)
        {
            var user = _sysUserService.GetById(GlobalContext.CurrentUser.UserId);
            if (user == null)
            {
                return new ReturnData(500, "找不到当前用户！");
            }
            user.Sex = userInfo.Sex;
            user.UserName = userInfo.UserName;
            user.Phonenumber = userInfo.Phonenumber;
            _sysUserService.Save(user);
            var currentUser = GetUserInfo(GlobalContext.Token.Value);
            CacheFactory.Cache.SetCache($"{CacheKey.AccessToken}{GlobalContext.Token.Value}", currentUser, currentUser.ExpirationTime);
            return new ReturnData();
        }

        #endregion

        #region 私有方法

        private UserInfo GetUserInfo(string token)
        {
            var user = _sysUserService.GetByToken(token);
            if (user == null)
            {
                return null;
            }
            UserInfo userInfo = user.ToEntityCopy<SysUserModel, UserInfo>();
            userInfo.Roles = _sysRoleService.GetByUserId(userInfo.UserId).Select(p => p.ToEntityCopy<SysRoleModel, RoleInfo>()).ToList();
            userInfo.Dept = _sysDeptService.GetById(userInfo.DeptId)?.ToEntityCopy<SysDeptModel, DeptInfo>() ?? null;
            userInfo.Posts = _sysPostService.GetByUserId(userInfo.UserId).Select(p => p.ToEntityCopy<SysPostModel, PostInfo>()).ToList();
            userInfo.Permissions = _sysMenuService.GetPermissionsByUserId(userInfo.UserId);
            return userInfo;
        }

        private bool SaveLoginLog(string userName, string status, string msg)
        {
            return _sysLoginLogService.Save(new SysLoginLogModel()
            {
                UserName = userName,
                Ipaddr = NetHepler.Ip,
                Browser = NetHepler.Browser,
                Os = NetHepler.Os,
                Status = status,
                Msg = msg,
                LoginTime = DateTime.Now
            }); ;
        }

        #endregion
    }
}
