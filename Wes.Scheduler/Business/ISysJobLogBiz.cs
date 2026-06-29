using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysJobLogBiz
    {
        RowData<SysJobLogModel> GetList(ParamData<JobLogParam> param);
        ResultData<SysJobLogModel> GetById(long id);
        ReturnData Delete(string ids);
        ReturnData Clean();
    }
}
