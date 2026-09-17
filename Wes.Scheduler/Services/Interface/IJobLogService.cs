using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Model;

namespace Wes.Scheduler.Services
{
    public interface IJobLogService
    {
        List<JobLogEntity> GetList(ParamData<JobLogParam> param, out int total);
        List<JobLogEntity> GetAll();
        JobLogEntity? GetById(long id);
        bool Save(JobLogEntity model);
        bool Delete(List<long> ids);
        bool Clean();
    }
}
