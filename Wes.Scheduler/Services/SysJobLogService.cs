using SqlSugar;
using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Service;
using Wes.Utils.Model;

namespace Wes.Scheduler.Services
{
    public class SysJobLogService : Repository<SysJobLogEntity>, ISysJobLogService
    {
        public SysJobLogService(ISqlSugarClient db) : base(db) { }

        public List<SysJobLogEntity> GetList(ParamData<JobLogParam> param, out int total)
        {
            var exp = Expressionable.Create<SysJobLogEntity>();
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.JobName))
                    exp.And(p => p.JobName.Contains(param.Params.JobName.Trim()));
                if (!string.IsNullOrWhiteSpace(param.Params.JobGroup))
                    exp.And(p => p.JobGroup == param.Params.JobGroup.Trim());
                if (!string.IsNullOrWhiteSpace(param.Params.Status))
                    exp.And(p => p.Status == param.Params.Status.Trim());
            }

            total = 0;
            var query = Context.Queryable<SysJobLogEntity>().Where(exp.ToExpression())
                .OrderBy(p => p.CreateTime, OrderByType.Desc);
            return param.PageSize == 0
                ? query.ToList()
                : query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysJobLogEntity> GetAll()
        {
            return Context.Queryable<SysJobLogEntity>().ToList();
        }

        public SysJobLogEntity? GetById(long id)
        {
            return Context.Queryable<SysJobLogEntity>().Where(p => p.JobLogId == id).First();
        }

        public bool Save(SysJobLogEntity model)
        {
            if (model.JobLogId > 0)
                return Update(model);
            return InsertReturnSnowflakeId(model) > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Deleteable<SysJobLogEntity>().In(ids).ExecuteCommand() > 0;
        }

        public bool Clean()
        {
            return Context.Deleteable<SysJobLogEntity>().ExecuteCommand() > 0;
        }
    }
}
