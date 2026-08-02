using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Model;

namespace Wes.Scheduler.Business
{
    public interface ISysJobBiz
    {
        RowData<SysJobEntity> GetList(ParamData<JobParam> param);
        ResultData<SysJobEntity> GetById(long id);
        ReturnData Save(SysJobEntity model);
        ReturnData Delete(string ids);
        ReturnData ChangeStatus(SysJobEntity model);
        ReturnData Run(long id);
    }
}
