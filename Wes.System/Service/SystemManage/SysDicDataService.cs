using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysDicDataService : Repository<SysDicDataEntity>, ISysDicDataService
    {
        public SysDicDataService(ISqlSugarClient db) : base(db) { }

        public SysDicDataEntity GetById(long id)
        {
            return GetFirst(p => p.DictDataId == id && p.IsDel == 0);
        }

        public List<SysDicDataEntity> GetByIds(List<long> ids)
        {
            return GetList(p => ids.Contains(p.DictDataId) && p.IsDel == 0);
        }

        public SysDicDataEntity GetByDictType(string dictType, string dictValue)
        {

            return GetFirst(p => p.DictType == dictType && p.IsDel == 0 && p.DictValue == dictValue);
        }

        public bool Save(SysDicDataEntity dic)
        {
            if (dic.DictDataId > 0)
            {
                dic.UpdateTime = DateTime.Now;
                dic.UpdateBy = GlobalContext.CurrentUser.Account;
                return Update(dic);
            }
            dic.IsDel = 0;
            dic.IsDefault = "N";
            dic.CreateTime = DateTime.Now;
            dic.CreateBy = GlobalContext.CurrentUser.Account;
            return InsertReturnSnowflakeId(dic) > 0;
        }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysDicDataEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.DictDataId)).ExecuteCommand() > 0;
        }

        public List<SysDicDataEntity> GetList(ParamData<DicDataParam> param, out int total)
        {
            Expressionable<SysDicDataEntity> express = Expressionable.Create<SysDicDataEntity>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.DictLabel))
                {
                    express.And(p => p.DictLabel.Contains(param.Params.DictLabel.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.DictType))
                {
                    express.And(p => p.DictType == param.Params.DictType);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Status))
                {
                    express.And(p => p.Status == param.Params.Status);
                }
            }
            total = 0;
            var query = Context.Queryable<SysDicDataEntity>().OrderBy(p=>p.DictSort).Where(express.ToExpression());
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysDicDataEntity> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public List<SysDicDataEntity> GetListByDicType(string dicType)
        {
            return GetList(p => p.DictType == dicType && p.Status == "0" && p.IsDel == 0);
        }

        public List<SysDicDataEntity> GetAllListByDicType(string dicType)
        {
            return GetList(p => p.DictType == dicType && p.IsDel == 0);
        }
    }
}

