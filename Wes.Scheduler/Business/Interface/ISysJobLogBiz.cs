using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Model;

namespace Wes.Scheduler.Business
{
    public interface ISysJobLogBiz
    {
        RowData<SysJobLogEntity> GetList(ParamData<JobLogParam> param);
        ResultData<SysJobLogEntity> GetById(long id);
        ReturnData Delete(string ids);
        ReturnData Clean();
    }
}
