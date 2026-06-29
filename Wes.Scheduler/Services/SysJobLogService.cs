using SqlSugar;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysJobLogService : Repository<SysJobLogModel>, ISysJobLogService
    {
        public SysJobLogService(ISqlSugarClient db) : base(db) { }

        public List<SysJobLogModel> GetList(ParamData<JobLogParam> param, out int total)
        {
            var exp = Expressionable.Create<SysJobLogModel>();
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
            var query = Context.Queryable<SysJobLogModel>().Where(exp.ToExpression())
                .OrderBy(p => p.CreateTime, OrderByType.Desc);
            return param.PageSize == 0
                ? query.ToList()
                : query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysJobLogModel> GetAll()
        {
            return Context.Queryable<SysJobLogModel>().ToList();
        }

        public SysJobLogModel? GetById(long id)
        {
            return Context.Queryable<SysJobLogModel>().Where(p => p.JobLogId == id).First();
        }

        public bool Save(SysJobLogModel model)
        {
            if (model.JobLogId > 0)
                return Update(model);
            return InsertReturnSnowflakeId(model) > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Deleteable<SysJobLogModel>().In(ids).ExecuteCommand() > 0;
        }

        public bool Clean()
        {
            return Context.Deleteable<SysJobLogModel>().ExecuteCommand() > 0;
        }
    }
}
