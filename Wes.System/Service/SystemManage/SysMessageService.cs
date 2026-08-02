using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;
using Org.BouncyCastle.Crypto;

namespace Wes.Service
{
    public class SysMessageService : Repository<SysMessageEntity>, ISysMessageService
    {
        public SysMessageService(ISqlSugarClient db) : base(db) { }

        public List<SysMessageEntity> GetList(ParamData<MessageParam> param, out int total)
        {
            Expressionable<SysMessageEntity> express = Expressionable.Create<SysMessageEntity>();
            express.And(p => p.IsDel == 0);
            express.And(p => p.UserId == GlobalContext.CurrentUser.UserId);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.MessageType))
                {
                    express.And(p => p.MessageType == param.Params.MessageType.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.MessageTitle))
                {
                    express.And(p => p.MessageTitle.Contains(param.Params.MessageTitle.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.MessageBody))
                {
                    express.And(p => p.MessageBody == param.Params.MessageBody.Trim());
                }
                if (param.Params.IsRead > -1)
                {
                    express.And(p => p.IsRead == param.Params.IsRead);
                }
            }
            total = 0;
            var query = Context.Queryable<SysMessageEntity>().Where(express.ToExpression())
                .Includes(p => p.SendUser).OrderBy(p => p.CreateTime, OrderByType.Desc);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysMessageEntity> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public SysMessageEntity GetById(long id)
        {
            return Context.Queryable<SysMessageEntity>().Where(p => p.MessageId == id && p.IsDel == 0)
                .Includes(p => p.SendUser).First();
        }

        public bool Save(SysMessageEntity model)
        {
            if (model.MessageId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(model);
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            return InsertReturnSnowflakeId(model) > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<SysMessageEntity>()
                .SetColumns(p => new SysMessageEntity()
                {
                    IsDel = 1,
                    UpdateTime = DateTime.Now,
                    UpdateBy = GlobalContext.CurrentUser.Account
                })
                .Where(p => p.UserId == GlobalContext.CurrentUser.UserId && ids.Contains(p.MessageId))
                .ExecuteCommand() > 0;
        }

        public bool ReadAll()
        {
            return Context.Updateable<SysMessageEntity>()
                .SetColumns(p => new SysMessageEntity()
                {
                    IsRead = 1,
                    ReadTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    UpdateBy = GlobalContext.CurrentUser.Account
                })
                .Where(p => p.UserId == GlobalContext.CurrentUser.UserId && p.IsRead == 0)
                .ExecuteCommand() > 0;
        }

        public bool Read(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<SysMessageEntity>().SetColumns(p => p.IsRead == 1)
                .Where(p => ids.Contains(p.MessageId) && p.UserId == GlobalContext.CurrentUser.UserId).ExecuteCommand() > 0;
        }
    }
}
