using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Model;

namespace Wes.Scheduler.Business
{
    public interface IJobBiz
    {
        RowData<JobEntity> GetList(ParamData<JobParam> param);
        ResultData<JobEntity> GetById(long id);
        ReturnData Save(JobEntity model);
        ReturnData Delete(string ids);
        ReturnData ChangeStatus(JobEntity model);
        ReturnData Run(long id);
    }
}
