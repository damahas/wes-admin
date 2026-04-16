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
    public class SysDeptService : Repository<SysDeptModel>, ISysDeptService
    {
        public SysDeptService(ISqlSugarClient db) : base(db) { }

        public SysDeptModel GetById(long deptId)
        {
            return Context.Queryable<SysDeptModel>().Where(p => p.DeptId == deptId && p.IsDel == 0).Includes(p => p.LeaderUser).First();
        }

        public List<SysDeptModel> GetList(DeptParam param)
        {
            Expressionable<SysDeptModel> express = Expressionable.Create<SysDeptModel>();
            express.And(p => p.IsDel == 0);
            if (param != null)
            {
                if (!string.IsNullOrWhiteSpace(param.DeptName))
                {
                    express.And(p => p.DeptName.Contains(param.DeptName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(param.Status))
                {
                    express.And(p => p.Status == param.Status);
                }
            }
            return Context.Queryable<SysDeptModel>().Includes(p => p.LeaderUser).Where(express.ToExpression()).ToList();
        }

        public List<SysDeptModel> GetAll()
        {
            return GetList();
        }

        public bool Save(SysDeptModel dic)
        {
            if (dic.DeptId > 0)
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
            return Context.Updateable<SysDeptModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.DeptId)).ExecuteCommand() > 0;
        }

        public List<SysDeptModel> GetExcludeById(long id)
        {
            return GetList(p => !p.Ancestors.Contains($"{id},") && p.DeptId != id && p.ParentId != id);
        }
    }
}

