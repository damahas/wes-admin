using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Model;

namespace Wes.Scheduler.Services
{
    public interface ISysJobService
    {
        List<SysJobEntity> GetList(ParamData<JobParam> param, out int total);
        List<SysJobEntity> GetAll();
        SysJobEntity? GetById(long id);
        bool Save(SysJobEntity model);
        bool Delete(List<long> ids);
        bool ChangeStatus(SysJobEntity model);
    }
}
