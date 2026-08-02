using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service.SystemManage.Service
{
    public class SysOperLogService : Repository<SysOperLogEntity>, ISysOperLogService
    {
        public SysOperLogService(ISqlSugarClient db) : base(db) { }

        public List<SysOperLogEntity> GetList(ParamData<OperLogParam> param, out int total)
        {
            Expressionable<SysOperLogEntity> express = Expressionable.Create<SysOperLogEntity>();
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.Title))
                {
                    express.And(p => p.Title.Contains(param.Params.Title.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.OperName))
                {
                    express.And(p => p.OperName.Contains(param.Params.OperName.Trim()));
                }
                if (param.Params.BusinessType != null)
                {
                    express.And(p => p.BusinessType == param.Params.BusinessType);
                }
                if (param.Params.Status != null)
                {
                    express.And(p => p.Status == param.Params.Status);
                }
                if (param.Params.BeginTime != null)
                {
                    express.And(p => p.OperTime >= param.Params.BeginTime);
                }
                if (param.Params.EndTime != null)
                {
                    express.And(p => p.OperTime <= param.Params.EndTime);
                }
            }
            total = 0;
            return Context.Queryable<SysOperLogEntity>().Where(express.ToExpression())
                .OrderBy(p => p.OperTime, OrderByType.Desc).ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public bool Save(SysOperLogEntity model)
        {
            return InsertReturnSnowflakeId(model) > 0;
        }
    }
}

