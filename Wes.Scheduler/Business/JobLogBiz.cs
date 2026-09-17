using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Scheduler.Services;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Scheduler.Business
{
    public class JobLogBiz : IJobLogBiz
    {
        private readonly IJobLogService _jobLogService;

        public JobLogBiz(IJobLogService jobLogService)
        {
            _jobLogService = jobLogService;
        }

        public RowData<JobLogEntity> GetList(ParamData<JobLogParam> param)
        {
            int total = 0;
            var result = new RowData<JobLogEntity>(_jobLogService.GetList(param, out total))
            {
                total = total
            };
            return result;
        }

        public ResultData<JobLogEntity> GetById(long id)
        {
            return new ResultData<JobLogEntity>(_jobLogService.GetById(id));
        }

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
                return new ReturnData(500, "参数有误！");

            return _jobLogService.Delete(delIds)
                ? new ReturnData()
                : new ReturnData(500, "删除失败！");
        }

        public ReturnData Clean()
        {
            return _jobLogService.Clean()
                ? new ReturnData()
                : new ReturnData(500, "清空失败！");
        }
    }
}
