using SqlSugar;
using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Service;
using Wes.Utils.Model;

namespace Wes.Scheduler.Services
{
    public class JobLogService : Repository<JobLogEntity>, IJobLogService
    {
        public JobLogService(ISqlSugarClient db) : base(db) { }

        public List<JobLogEntity> GetList(ParamData<JobLogParam> param, out int total)
        {
            var exp = Expressionable.Create<JobLogEntity>();
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
            var query = Context.Queryable<JobLogEntity>().Where(exp.ToExpression())
                .OrderBy(p => p.CreateTime, OrderByType.Desc);
            return param.PageSize == 0
                ? query.ToList()
                : query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<JobLogEntity> GetAll()
        {
            return Context.Queryable<JobLogEntity>().ToList();
        }

        public JobLogEntity? GetById(long id)
        {
            return Context.Queryable<JobLogEntity>().Where(p => p.JobLogId == id).First();
        }

        public bool Save(JobLogEntity model)
        {
            if (model.JobLogId > 0)
                return Update(model);
            return InsertReturnSnowflakeId(model) > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Deleteable<JobLogEntity>().In(ids).ExecuteCommand() > 0;
        }

        public bool Clean()
        {
            return Context.Deleteable<JobLogEntity>().ExecuteCommand() > 0;
        }
    }
}
