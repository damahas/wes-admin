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
    public class SysDicDataService : Repository<SysDicDataModel>, ISysDicDataService
    {
        public SysDicDataService(ISqlSugarClient db) : base(db) { }

        public SysDicDataModel GetById(long id)
        {
            return GetFirst(p => p.DictDataId == id && p.IsDel == 0);
        }

        public List<SysDicDataModel> GetByIds(List<long> ids)
        {
            return GetList(p => ids.Contains(p.DictDataId) && p.IsDel == 0);
        }

        public SysDicDataModel GetByDictType(string dictType, string dictValue)
        {

            return GetFirst(p => p.DictType == dictType && p.IsDel == 0 && p.DictValue == dictValue);
        }

        public bool Save(SysDicDataModel dic)
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
            return Context.Updateable<SysDicDataModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.DictDataId)).ExecuteCommand() > 0;
        }

        public List<SysDicDataModel> GetList(ParamData<DicDataParam> param, out int total)
        {
            Expressionable<SysDicDataModel> express = Expressionable.Create<SysDicDataModel>();
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
            var query = Context.Queryable<SysDicDataModel>().OrderBy(p=>p.DictSort).Where(express.ToExpression());
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysDicDataModel> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public List<SysDicDataModel> GetListByDicType(string dicType)
        {
            return GetList(p => p.DictType == dicType && p.Status == "0" && p.IsDel == 0);
        }

        public List<SysDicDataModel> GetAllListByDicType(string dicType)
        {
            return GetList(p => p.DictType == dicType && p.IsDel == 0);
        }
    }
}

