using Wes.DbModel;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysJobLogBiz : ISysJobLogBiz
    {
        private readonly ISysJobLogService _sysJobLogService;

        public SysJobLogBiz(ISysJobLogService sysJobLogService)
        {
            _sysJobLogService = sysJobLogService;
        }

        public RowData<SysJobLogModel> GetList(ParamData<JobLogParam> param)
        {
            int total = 0;
            var result = new RowData<SysJobLogModel>(_sysJobLogService.GetList(param, out total))
            {
                total = total
            };
            return result;
        }

        public ResultData<SysJobLogModel> GetById(long id)
        {
            return new ResultData<SysJobLogModel>(_sysJobLogService.GetById(id));
        }

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
                return new ReturnData(500, "参数有误！");

            return _sysJobLogService.Delete(delIds)
                ? new ReturnData()
                : new ReturnData(500, "删除失败！");
        }

        public ReturnData Clean()
        {
            return _sysJobLogService.Clean()
                ? new ReturnData()
                : new ReturnData(500, "清空失败！");
        }
    }
}
