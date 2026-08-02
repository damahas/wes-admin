using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysPostService : Repository<SysPostEntity>, ISysPostService
    {
        public SysPostService(ISqlSugarClient db) : base(db) { }

        public List<SysPostEntity> GetByUserId(long userId)
        {
            return this.Context.Queryable<SysPostEntity>()
                .LeftJoin<SysUserPostEntity>((o, p) => o.PostId == p.PostId)
                .Where((o, p) => o.IsDel == 0 && p.UserId == userId && o.Status == "0").ToList();
        }

        #region 角色基本操作

        public SysPostEntity GetById(long postId)
        {
            return GetFirst(p => p.PostId == postId && p.IsDel == 0);
        }

        public SysPostEntity GetByPostCode(string postCode)
        {
            return GetFirst(p => p.PostCode == postCode && p.IsDel == 0);
        }

        public List<SysPostEntity> GetList(ParamData<PostParam> param, out int total)
        {
            Expressionable<SysPostEntity> express = Expressionable.Create<SysPostEntity>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.PostCode))
                {
                    express.And(p => p.PostCode.Contains(param.Params.PostCode.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.PostName))
                {
                    express.And(p => p.PostName.Contains(param.Params.PostName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Status))
                {
                    express.And(p => p.Status == param.Params.Status);
                }
            }
            total = 0;
            var query = Context.Queryable<SysPostEntity>().Where(express.ToExpression()).OrderBy(p => p.PostSort);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysPostEntity> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public bool Save(SysPostEntity post)
        {
            if (post.PostId > 0)
            {
                post.UpdateTime = DateTime.Now;
                post.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(post);
            }
            post.IsDel = 0;
            post.CreateTime = DateTime.Now;
            post.CreateBy = GlobalContext.CurrentUser.Account;
            return InsertReturnSnowflakeId(post) > 0;
        }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysPostEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.PostId)).ExecuteCommand() > 0;
        }

        #endregion

    }
}
