using SqlSugar;
using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Scheduler.Services
{
    public class JobService : Repository<JobEntity>, IJobService
    {
        public JobService(ISqlSugarClient db) : base(db) { }

        public List<JobEntity> GetList(ParamData<JobParam> param, out int total)
        {
            var exp = Expressionable.Create<JobEntity>();
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
            var query = Context.Queryable<JobEntity>().Where(exp.ToExpression())
                .OrderBy(p => p.JobId, OrderByType.Desc);
            return param.PageSize == 0
                ? query.ToList()
                : query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<JobEntity> GetAll()
        {
            return Context.Queryable<JobEntity>().ToList();
        }

        public JobEntity? GetById(long id)
        {
            return Context.Queryable<JobEntity>().Where(p => p.JobId == id).First();
        }

        public bool Save(JobEntity model)
        {
            if (model.JobId > 0)
            {
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = GlobalContext.CurrentUser?.Account;
                return Update(model);
            }
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser?.Account;
            return InsertReturnSnowflakeId(model) > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Deleteable<JobEntity>().In(ids).ExecuteCommand() > 0;
        }

        public bool ChangeStatus(JobEntity model)
        {
            var updateBy = GlobalContext.CurrentUser?.Account;
            return Context.Updateable<JobEntity>()
                .SetColumns(p => new JobEntity { Status = model.Status, UpdateTime = DateTime.Now, UpdateBy = updateBy })
                .Where(p => p.JobId == model.JobId)
                .ExecuteCommand() > 0;
        }
    }
}
