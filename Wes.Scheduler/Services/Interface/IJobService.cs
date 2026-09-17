using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Model;

namespace Wes.Scheduler.Services
{
    public interface IJobService
    {
        List<JobEntity> GetList(ParamData<JobParam> param, out int total);
        List<JobEntity> GetAll();
        JobEntity? GetById(long id);
        bool Save(JobEntity model);
        bool Delete(List<long> ids);
        bool ChangeStatus(JobEntity model);
    }
}
