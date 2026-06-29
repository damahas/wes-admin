using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.DbModel;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    /// <summary>
    /// 定时任务管理
    /// </summary>
    [ApiController]
    [Route("monitor/job")]
    public class JobController : ControllerBase
    {
        private readonly ISysJobBiz _sysJobBiz;
        private readonly ISysJobLogBiz _sysJobLogBiz;

        public JobController(ISysJobBiz sysJobBiz, ISysJobLogBiz sysJobLogBiz)
        {
            _sysJobBiz = sysJobBiz;
            _sysJobLogBiz = sysJobLogBiz;
        }

        /// <summary>任务列表</summary>
        [HttpGet("list")]
        public ReturnData GetJobList([FromQuery] ParamData<JobParam> param)
        {
            return _sysJobBiz.GetList(param);
        }

        /// <summary>任务详情</summary>
        [HttpGet("{id}")]
        public ReturnData GetJobById(long id)
        {
            return _sysJobBiz.GetById(id);
        }

        /// <summary>新增任务</summary>
        [HttpPost]
        public ReturnData AddJob([FromBody] SysJobModel model)
        {
            model.JobId = 0; // 强制走新增
            return _sysJobBiz.Save(model);
        }

        /// <summary>编辑任务</summary>
        [HttpPut]
        public ReturnData UpdateJob([FromBody] SysJobModel model)
        {
            return _sysJobBiz.Save(model);
        }

        /// <summary>删除任务</summary>
        [HttpDelete("{ids}")]
        public ReturnData DeleteJob(string ids)
        {
            return _sysJobBiz.Delete(ids);
        }

        /// <summary>修改任务状态</summary>
        [HttpPut("changeStatus")]
        public ReturnData ChangeStatus([FromBody] SysJobModel model)
        {
            return _sysJobBiz.ChangeStatus(model);
        }

        /// <summary>执行一次</summary>
        [HttpPost("run/{id}")]
        public ReturnData RunJob(long id)
        {
            return _sysJobBiz.Run(id);
        }

        // ==================== 日志 ====================

        /// <summary>日志列表</summary>
        [HttpGet("log/list")]
        public ReturnData GetLogList([FromQuery] ParamData<JobLogParam> param)
        {
            return _sysJobLogBiz.GetList(param);
        }

        /// <summary>日志详情</summary>
        [HttpGet("log/{id}")]
        public ReturnData GetLogById(long id)
        {
            return _sysJobLogBiz.GetById(id);
        }

        /// <summary>删除日志</summary>
        [HttpDelete("log/{ids}")]
        public ReturnData DeleteLog(string ids)
        {
            return _sysJobLogBiz.Delete(ids);
        }

        /// <summary>清空日志</summary>
        [HttpDelete("log/clean")]
        public ReturnData CleanLog()
        {
            return _sysJobLogBiz.Clean();
        }
    }
}
