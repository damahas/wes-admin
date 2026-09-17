using Microsoft.AspNetCore.Mvc;
using Wes.Scheduler.Business;
using Wes.Scheduler.Model.Entity;
using Wes.Scheduler.Model.ViewModel;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Scheduler.Controllers
{
    /// <summary>
    /// 定时任务管理
    /// </summary>
    [ApiController]
    [Route("scheduler/job")]
    public class JobController : ControllerBase
    {
        private readonly IJobBiz _jobBiz;
        private readonly IJobLogBiz _jobLogBiz;

        public JobController(IJobBiz jobBiz, IJobLogBiz jobLogBiz)
        {
            _jobBiz = jobBiz;
            _jobLogBiz = jobLogBiz;
        }

        /// <summary>任务列表</summary>
        [HttpGet("list")]
        public ReturnData GetJobList([FromQuery] ParamData<JobParam> param)
        {
            return _jobBiz.GetList(param);
        }

        /// <summary>任务详情</summary>
        [HttpGet("{id}")]
        public ReturnData GetJobById(long id)
        {
            return _jobBiz.GetById(id);
        }

        /// <summary>新增任务</summary>
        [HttpPost]
        public ReturnData AddJob([FromBody] JobEntity model)
        {
            model.JobId = 0; // 强制走新增
            return _jobBiz.Save(model);
        }

        /// <summary>编辑任务</summary>
        [HttpPut]
        public ReturnData UpdateJob([FromBody] JobEntity model)
        {
            return _jobBiz.Save(model);
        }

        /// <summary>删除任务</summary>
        [HttpDelete("{ids}")]
        public ReturnData DeleteJob(string ids)
        {
            return _jobBiz.Delete(ids);
        }

        /// <summary>修改任务状态</summary>
        [HttpPut("changeStatus")]
        public ReturnData ChangeStatus([FromBody] JobEntity model)
        {
            return _jobBiz.ChangeStatus(model);
        }

        /// <summary>执行一次</summary>
        [HttpPost("run/{id}")]
        public ReturnData RunJob(long id)
        {
            return _jobBiz.Run(id);
        }

        // ==================== 日志 ====================

        /// <summary>日志列表</summary>
        [HttpGet("log/list")]
        public ReturnData GetLogList([FromQuery] ParamData<JobLogParam> param)
        {
            return _jobLogBiz.GetList(param);
        }

        /// <summary>日志详情</summary>
        [HttpGet("log/{id}")]
        public ReturnData GetLogById(long id)
        {
            return _jobLogBiz.GetById(id);
        }

        /// <summary>删除日志</summary>
        [HttpDelete("log/{ids}")]
        public ReturnData DeleteLog(string ids)
        {
            return _jobLogBiz.Delete(ids);
        }

        /// <summary>清空日志</summary>
        [HttpDelete("log/clean")]
        public ReturnData CleanLog()
        {
            return _jobLogBiz.Clean();
        }
    }
}
