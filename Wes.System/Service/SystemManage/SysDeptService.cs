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
    public class SysDeptService : Repository<SysDeptEntity>, ISysDeptService
    {
        public SysDeptService(ISqlSugarClient db) : base(db) { }

        public SysDeptEntity GetById(long deptId)
        {
            return Context.Queryable<SysDeptEntity>().Where(p => p.DeptId == deptId && p.IsDel == 0).Includes(p => p.LeaderUser).First();
        }

        public List<SysDeptEntity> GetList(DeptParam param)
        {
            Expressionable<SysDeptEntity> express = Expressionable.Create<SysDeptEntity>();
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
            return Context.Queryable<SysDeptEntity>().Includes(p => p.LeaderUser).Where(express.ToExpression()).ToList();
        }

        public List<SysDeptEntity> GetAll()
        {
            return GetList();
        }

        public bool Save(SysDeptEntity dic)
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
            return Context.Updateable<SysDeptEntity>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.DeptId)).ExecuteCommand() > 0;
        }

        public List<SysDeptEntity> GetExcludeById(long id)
        {
            return GetList(p => !p.Ancestors.Contains($"{id},") && p.DeptId != id && p.ParentId != id);
        }
    }
}

