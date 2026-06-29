using SqlSugar;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysUserService : Repository<SysUserModel>, ISysUserService
    {
        public SysUserService(ISqlSugarClient db) : base(db) { }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SysUserModel Login(string userName, string password)
        {
            return this.GetFirst(p => p.Account == userName && p.Password == password && p.IsDel == 0 && p.Status == "0");
        }

        public List<SysUserModel> GetListByRoleId(ParamData<RoleUserParam> param, out int total)
        {
            Expressionable<SysUserModel> express = Expressionable.Create<SysUserModel>();
            express.And(u => u.IsDel == 0);
            if (param.Params != null)
            {
                if (param.Params.RoleId > 0)
                {
                    if (param.Params.IsNoExist)
                    {
                        express.And(u => SqlFunc.Subqueryable<SysUserRoleModel>().Where(s => s.RoleId == param.Params.RoleId && s.UserId == u.UserId).NotAny());
                    }
                    else
                    {
                        express.And(u => SqlFunc.Subqueryable<SysUserRoleModel>().Where(s => s.RoleId == param.Params.RoleId && s.UserId == u.UserId).Any());
                    }
                }
                if (!string.IsNullOrWhiteSpace(param.Params.UserName))
                {
                    express.And(u => u.UserName.Contains(param.Params.UserName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Phonenumber))
                {
                    express.And(u => u.Phonenumber.Contains(param.Params.Phonenumber.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Account))
                {
                    express.And(u => u.Account.Contains(param.Params.Account.Trim()));
                }
            }
            total = 0;
            var query = Context.Queryable<SysUserModel>().OrderBy(p => p.UserId, OrderByType.Desc).Where(express.ToExpression());
            if (param.PageSize == 0)
            {
                return query.ToList();
            }
            return query.ToPageList(param.PageNum, param.PageSize, ref total);

        }

        #region 用户操作

        /// <summary>
        /// 通过userId获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SysUserModel GetById(long userId)
        {
            return this.GetFirst(p => p.UserId == userId && p.IsDel == 0);
        }

        public SysUserModel GetByAccout(string account)
        {
            return this.GetFirst(p => p.Account == account && p.IsDel == 0);
        }

        public List<SysUserModel> GetByRoleId(long roleId)
        {
            return Context.Queryable<SysUserModel>()
                .InnerJoin<SysUserRoleModel>((u, r) => u.UserId == r.UserId)
                .Where((u, r) => r.RoleId == roleId).ToList();
        }

        public List<SysUserModel> GetByDeptId(long deptId)
        {
            return Context.Queryable<SysUserModel>()
                .InnerJoin<SysDeptModel>((u, d) => u.DeptId == d.DeptId)
                .Where((u, d) => d.Ancestors.Contains($",{deptId},") || d.ParentId == deptId || d.DeptId == deptId).ToList();
        }

        public long GetLeaderIdByUserId(long userId)
        {
            var user = GetFirst(p => p.UserId == userId);
            if (user == null || user.DeptId == 0)
            {
                return 0;
            }
            var depts = Context.Queryable<SysDeptModel>().Where(p => p.LeaderUserId > 0
            && (p.DeptId == user.DeptId || p.ParentId == user.DeptId || p.Ancestors.Contains($",{user.DeptId},"))
            ).ToList();
            return depts.OrderBy(p => p.Ancestors.Length).FirstOrDefault()?.LeaderUserId ?? 0;
        }

        public long GetLeaderIdByAccount(string account)
        {
            var user = GetFirst(p => p.Account == account);
            if (user == null || user.DeptId == 0)
            {
                return 0;
            }
            var depts = Context.Queryable<SysDeptModel>().Where(p => p.LeaderUserId > 0
            && (p.DeptId == user.DeptId || p.ParentId == user.DeptId || p.Ancestors.Contains($",{user.DeptId},"))
            ).ToList();
            return depts.OrderBy(p => p.Ancestors.Length).FirstOrDefault()?.LeaderUserId ?? 0;
        }

        public SysUserModel GetByUserName(string userName)
        {
            return this.GetFirst(p => p.UserName == userName && p.IsDel == 0);
        }

        public List<SysUserModel> GetList(ParamData<UserParam> param, out int total)
        {
            //TODO 查询树记得查询子的
            Expressionable<SysUserModel> express = Expressionable.Create<SysUserModel>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.Account))
                {
                    express.And(p => p.Account.Contains(param.Params.Account.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.UserName))
                {
                    express.And(p => p.UserName.Contains(param.Params.UserName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Phonenumber))
                {
                    express.And(p => p.Phonenumber == param.Params.Phonenumber);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Status))
                {
                    express.And(p => p.Status == param.Params.Status);
                }
                if (param.Params.DeptId > 0)
                {
                    express.And(p => p.DeptId == param.Params.DeptId
                    || p.Dept.ParentId == param.Params.DeptId
                    || p.Dept.Ancestors.Contains($"{param.Params.DeptId},"));
                }
                if (param.Params.BeginTime != null)
                {
                    express.And(p => p.CreateTime >= param.Params.BeginTime);
                }
                if (param.Params.EndTime != null)
                {
                    express.And(p => p.CreateTime <= param.Params.EndTime);
                }
            }
            total = 0;
            var query = Context.Queryable<SysUserModel>().Includes(p => p.Dept).Where(express.ToExpression());
            if (param.PageNum == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysUserModel> GetList(List<long> userIds)
        {
            return Context.Queryable<SysUserModel>().Where(p => userIds.Contains(p.UserId)).Includes(p => p.Dept).ToList();
        }

        public bool Save(SysUserModel user)
        {
            if (user.UserId > 0)
            {
                user.UpdateTime = DateTime.Now;
                user.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(user);
            }
            user.IsDel = 0;
            user.CreateTime = DateTime.Now;
            user.CreateBy = GlobalContext.CurrentUser.Account;
            return InsertReturnSnowflakeId(user) > 0;
        }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysUserModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.UserId)).ExecuteCommand() > 0;
        }

        #endregion

        #region 用户角色操作

        public List<SysUserRoleModel> GetUserRole(long userId)
        {
            return Context.Queryable<SysUserRoleModel>().Where(p => p.UserId == userId).ToList();
        }

        public bool SaveUserRole(List<SysUserRoleModel> userRoles)
        {
            return this.Context.Insertable(userRoles).ExecuteReturnSnowflakeIdList().Count > 0;
        }

        public bool DeleteUserRole(long userId, List<long> roleIds)
        {
            Expressionable<SysUserRoleModel> express = Expressionable.Create<SysUserRoleModel>();
            express.And(p => p.UserId == userId);
            if (roleIds != null && roleIds.Count > 0)
            {
                express.And(p => !roleIds.Contains(p.RoleId));
            }
            return Context.Deleteable<SysUserRoleModel>().Where(express.ToExpression()).ExecuteCommand() > 0;
        }

        #endregion

        #region 用户岗位操作

        public List<SysUserPostModel> GetUserPost(long userId)
        {
            return Context.Queryable<SysUserPostModel>().Where(p => p.UserId == userId).ToList();
        }

        public bool SaveUserPost(List<SysUserPostModel> userPosts)
        {
            return this.Context.Insertable(userPosts).ExecuteReturnSnowflakeIdList().Count > 0;
        }

        public bool DeleteUserPost(long userId, List<long> postIds)
        {
            Expressionable<SysUserPostModel> express = Expressionable.Create<SysUserPostModel>();
            express.And(p => p.UserId == userId);
            if (postIds != null && postIds.Count > 0)
            {
                express.And(p => !postIds.Contains(p.PostId));
            }
            return Context.Deleteable<SysUserPostModel>().Where(express.ToExpression()).ExecuteCommand() > 0;
        }


        #endregion

        #region Token操作

        /// <summary>
        /// 通过token获取用户
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public SysUserModel GetByToken(string token)
        {
            return this.Context.Queryable<SysUserModel>()
                .LeftJoin<SysTokenModel>((o, p) => o.UserId == p.UserId)
                .Where((o, p) => p.Token == token && p.IsDel == 0 && p.Status == 0 && p.ExpirationTime > DateTime.Now).First();
        }

        /// <summary>
        /// 保存token
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public SysTokenModel SaveToken(SysTokenModel tokenModel)
        {
            if (tokenModel.TokenId == 0)
            {
                tokenModel.CreateTime = DateTime.Now;
                tokenModel.IsDel = 0;
                tokenModel.TokenId = this.Context.Insertable(tokenModel).ExecuteReturnSnowflakeId();
            }
            else
            {
                this.Context.Updateable(tokenModel).ExecuteCommand();
            }
            return tokenModel;
        }

        /// <summary>
        /// 通过token获取token对象
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public SysTokenModel GetToken(string token)
        {
            return this.Context.Queryable<SysTokenModel>().Where(p => p.Token == token && p.IsDel == 0 && p.Status == 0).First();
        }

        public bool InvalidToken(string token)
        {
            return Context.Updateable<SysTokenModel>().SetColumns(p => p.Status == 1)
                    .Where(p => p.Token == token).ExecuteCommand() > 0;
        }

        public SysTokenModel InvalidToken(long tokenId)
        {
            Context.Updateable<SysTokenModel>().SetColumns(p => p.Status == 1)
                    .Where(p => p.TokenId == tokenId).ExecuteCommand();
            return Context.Queryable<SysTokenModel>().Where(p => p.TokenId == tokenId).First();
        }

        public List<OnlineInfo> GetOnlineList(ParamData<OnlineParam> param, out int total)
        {
            Expressionable<SysTokenModel, SysUserModel, SysDeptModel> express = Expressionable.Create<SysTokenModel, SysUserModel, SysDeptModel>();
            express.And((t, u, d) => t.Status == 0);
            express.And((t, u, d) => t.ExpirationTime <= DateTime.Now);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.LoginLocation))
                {
                    express.And((t, u, d) => t.LoginLocation.Contains(param.Params.LoginLocation));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.UserName))
                {
                    express.And((t, u, d) => u.UserName.Contains(param.Params.UserName));
                }
            }

            total = 0;
            return Context.Queryable<SysTokenModel, SysUserModel, SysDeptModel>((t, u, d) =>
                new JoinQueryInfos(
                        JoinType.Left, t.UserId == u.UserId,
                        JoinType.Left, u.DeptId == d.DeptId
                )
                ).Where(express.ToExpression())
                .OrderBy((t, u, d) => t.CreateTime, OrderByType.Desc)
                .Select((t, u, d) => new OnlineInfo
                {
                    TokenId = t.TokenId,
                    UserName = u.Account,
                    Browser = t.Browser,
                    LoginLocation = t.LoginLocation,
                    Ipaddr = t.LoginIp,
                    DeptName = d.DeptName,
                    Os = t.Os,
                    LoginTime = t.CreateTime.Value,
                    ExpirationTime = t.ExpirationTime

                })
                .ToPageList(param.PageNum, param.PageSize, ref total);
        }

        #endregion

    }
}
