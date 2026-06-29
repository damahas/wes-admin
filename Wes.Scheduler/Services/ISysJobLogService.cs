using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysJobLogService
    {
        List<SysJobLogModel> GetList(ParamData<JobLogParam> param, out int total);
        List<SysJobLogModel> GetAll();
        SysJobLogModel? GetById(long id);
        bool Save(SysJobLogModel model);
        bool Delete(List<long> ids);
        bool Clean();
    }
}
