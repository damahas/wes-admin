using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysDicTypeService : Repository<SysDicTypeModel>, ISysDicTypeService
    {
        public SysDicTypeService(ISqlSugarClient db) : base(db) { }

        public List<SysDicTypeModel> GetList(ParamData<DicTypeParam> param, out int total)
        {
            Expressionable<SysDicTypeModel> express = Expressionable.Create<SysDicTypeModel>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.DictName))
                {
                    express.And(p => p.DictName.Contains(param.Params.DictName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.DictType))
                {
                    express.And(p => p.DictType.Contains(param.Params.DictType.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Status))
                {
                    express.And(p => p.Status == param.Params.Status);
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
            var query = Context.Queryable<SysDicTypeModel>().Where(express.ToExpression()).OrderBy(p => p.DictId, OrderByType.Desc);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysDicTypeModel> GetAll()
        {
            return GetList();
        }

        public SysDicTypeModel GetById(long id)
        {
            return GetFirst(p => p.DictId == id && p.IsDel == 0);
        }

        public List<SysDicTypeModel> GetByIds(List<long> ids)
        {
            return GetList(p => p.IsDel == 0 && ids.Contains(p.DictId));
        }

        public SysDicTypeModel GetByDictType(string dictType)
        {
            return GetFirst(p => p.DictType == dictType && p.IsDel == 0);
        }

        public bool Save(SysDicTypeModel dic)
        {
            if (dic.DictId > 0)
            {
                dic.UpdateTime = DateTime.Now;
                dic.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(dic);
            }
            dic.IsDel = 0;
            dic.CreateTime = DateTime.Now;
            dic.CreateBy = GlobalContext.CurrentUser.Account;
            return InsertReturnSnowflakeId(dic) > 0;
        }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysDicTypeModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.DictId)).ExecuteCommand() > 0;
        }
    }
}

