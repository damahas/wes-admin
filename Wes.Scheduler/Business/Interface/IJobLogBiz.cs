using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Model;

namespace Wes.Scheduler.Business
{
    public interface IJobLogBiz
    {
        RowData<JobLogEntity> GetList(ParamData<JobLogParam> param);
        ResultData<JobLogEntity> GetById(long id);
        ReturnData Delete(string ids);
        ReturnData Clean();
    }
}
