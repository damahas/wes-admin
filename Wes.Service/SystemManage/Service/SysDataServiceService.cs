using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service.SystemManage.Service
{
    public class SysDataServiceService : Repository<SysDataServiceModel>, ISysDataServiceService
    {
        public SysDataServiceService(ISqlSugarClient db) : base(db) { }

        public List<SysDataServiceModel> GetList(ParamData<DataServiceParam> param, out int total)
        {
            Expressionable<SysDataServiceModel> express = Expressionable.Create<SysDataServiceModel>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.ServiceCode))
                {
                    express.And(p => p.ServiceCode.Contains(param.Params.ServiceCode.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.ServiceName))
                {
                    express.And(p => p.ServiceName.Contains(param.Params.ServiceName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Params.ServiceType))
                {
                    express.And(p => p.ServiceType == param.Params.ServiceType);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.Category))
                {
                    express.And(p => p.Category == param.Params.Category);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.IsEnabled))
                {
                    express.And(p => p.IsEnabled == param.Params.IsEnabled);
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
            var query = Context.Queryable<SysDataServiceModel>().Where(express.ToExpression()).OrderBy(p => p.DsId, OrderByType.Desc);
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysDataServiceModel> GetAll()
        {
            return GetList();
        }

        public SysDataServiceModel GetById(long id)
        {
            return Context.Queryable<SysDataServiceModel>()
                .Where(p => p.DsId == id && p.IsDel == 0)
                .Includes(p => p.Nodes.OrderBy(t=>t.SortBy).ToList()).First();
        }

        public List<SysDataServiceModel> GetByIds(List<long> ids)
        {
            return GetList(p => p.IsDel == 0 && ids.Contains(p.DsId));
        }

        public bool Save(SysDataServiceModel model)
        {
            if (model.DsId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser.Account;
                return Context.UpdateNav(model).Include(p => p.Nodes).ExecuteCommand();
            }
            model.DsId = SnowFlakeSingle.Instance.NextId();
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            return Context.InsertNav(model).Include(p => p.Nodes).ExecuteCommand();
        }

        public bool Delete(List<long> ids)
        {
            return Context.Updateable<SysDataServiceModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.DsId)).ExecuteCommand() > 0;
        }

        #region 数据库表信息
        public List<DbTableInfo> GetTables() { 
            return Context.DbMaintenance.GetTableInfoList();
        }
        public List<DbColumnInfo> GetTableColumns(string tableName)
        {
            return Context.DbMaintenance.GetColumnInfosByTableName(tableName);
        }
        #endregion
    }
}
