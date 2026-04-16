using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service.MonitorManage.Service
{
    public class SysLoginLogService : Repository<SysLoginLogModel>, ISysLoginLogService
    {
        public SysLoginLogService(ISqlSugarClient db) : base(db) { }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysLoginLogModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.LoginId)).ExecuteCommand() > 0;
        }

        public bool DeleteAll()
        {
            return Context.Updateable<SysLoginLogModel>().SetColumns(p => p.IsDel == 1).ExecuteCommand() > 0;
        }

        public List<SysLoginLogModel> GetList(ParamData<LoginLogParam> param, out int total)
        {
            Expressionable<SysLoginLogModel> express = Expressionable.Create<SysLoginLogModel>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.UserName))
                {
                    express.And(p => p.UserName.Contains(param.Params.UserName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Ipaddr))
                {
                    express.And(p => p.Ipaddr.Contains(param.Params.Ipaddr.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Status))
                {
                    express.And(p => p.Status == param.Params.Status);
                }
                if (param.Params.BeginTime != null)
                {
                    express.And(p => p.LoginTime >= param.Params.BeginTime);
                }
                if (param.Params.EndTime != null)
                {
                    express.And(p => p.LoginTime <= param.Params.EndTime);
                }
            }
            total = 0;
            return Context.Queryable<SysLoginLogModel>().Where(express.ToExpression())
                .OrderBy(p => p.LoginTime, OrderByType.Desc).ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public bool Save(SysLoginLogModel model)
        {
            model.IsDel = 0;
            return InsertReturnSnowflakeId(model) > 0;
        }
    }
}

