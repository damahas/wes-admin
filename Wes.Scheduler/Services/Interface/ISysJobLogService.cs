using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Model;

namespace Wes.Scheduler.Services
{
    public interface ISysJobLogService
    {
        List<SysJobLogEntity> GetList(ParamData<JobLogParam> param, out int total);
        List<SysJobLogEntity> GetAll();
        SysJobLogEntity? GetById(long id);
        bool Save(SysJobLogEntity model);
        bool Delete(List<long> ids);
        bool Clean();
    }
}
