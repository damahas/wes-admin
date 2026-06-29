using SqlSugar;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public class SysJobService : Repository<SysJobModel>, ISysJobService
    {
        public SysJobService(ISqlSugarClient db) : base(db) { }

        public List<SysJobModel> GetList(ParamData<JobParam> param, out int total)
        {
            var exp = Expressionable.Create<SysJobModel>();
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
            var query = Context.Queryable<SysJobModel>().Where(exp.ToExpression())
                .OrderBy(p => p.JobId, OrderByType.Desc);
            return param.PageSize == 0
                ? query.ToList()
                : query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysJobModel> GetAll()
        {
            return Context.Queryable<SysJobModel>().ToList();
        }

        public SysJobModel? GetById(long id)
        {
            return Context.Queryable<SysJobModel>().Where(p => p.JobId == id).First();
        }

        public bool Save(SysJobModel model)
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
            return Context.Deleteable<SysJobModel>().In(ids).ExecuteCommand() > 0;
        }

        public bool ChangeStatus(SysJobModel model)
        {
            var updateBy = GlobalContext.CurrentUser?.Account;
            return Context.Updateable<SysJobModel>()
                .SetColumns(p => new SysJobModel { Status = model.Status, UpdateTime = DateTime.Now, UpdateBy = updateBy })
                .Where(p => p.JobId == model.JobId)
                .ExecuteCommand() > 0;
        }
    }
}
