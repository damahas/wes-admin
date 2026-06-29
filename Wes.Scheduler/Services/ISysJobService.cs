using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysJobService
    {
        List<SysJobModel> GetList(ParamData<JobParam> param, out int total);
        List<SysJobModel> GetAll();
        SysJobModel? GetById(long id);
        bool Save(SysJobModel model);
        bool Delete(List<long> ids);
        bool ChangeStatus(SysJobModel model);
    }
}
