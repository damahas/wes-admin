using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysJobBiz
    {
        RowData<SysJobModel> GetList(ParamData<JobParam> param);
        ResultData<SysJobModel> GetById(long id);
        ReturnData Save(SysJobModel model);
        ReturnData Delete(string ids);
        ReturnData ChangeStatus(SysJobModel model);
        ReturnData Run(long id);
    }
}
