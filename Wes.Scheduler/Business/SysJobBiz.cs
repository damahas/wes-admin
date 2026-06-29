using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Wes.DbModel;
using Wes.Service;
using Wes.Scheduler;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysJobBiz : ISysJobBiz
    {
        private readonly ISysJobService _sysJobService;

        public SysJobBiz(ISysJobService sysJobService)
        {
            _sysJobService = sysJobService;
        }

        public RowData<SysJobModel> GetList(ParamData<JobParam> param)
        {
            int total = 0;
            var result = new RowData<SysJobModel>(_sysJobService.GetList(param, out total))
            {
                total = total
            };
            return result;
        }

        public ResultData<SysJobModel> GetById(long id)
        {
            return new ResultData<SysJobModel>(_sysJobService.GetById(id));
        }

        public ReturnData Save(SysJobModel model)
        {
            // 启用状态（0）需要 cron 校验；暂停状态（1）不校验，直接落库
            if (model.Status == "0")
            {
                var cronErr = ValidateCron(model.CronExpression);
                if (cronErr != null)
                    return new ReturnData(500, $"Cron 表达式无效：{cronErr}");
            }

            if (!_sysJobService.Save(model))
                return new ReturnData(500, "保存失败！");

            // 启用状态 → 同步到 Quartz；暂停状态 → 只存数据库，不调 Quartz
            if (model.Status == "0")
                _ = SyncToSchedulerAsync(model);

            return new ReturnData();
        }

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
                return new ReturnData(500, "参数有误！");

            // 先移除 Quartz 调度
            foreach (var id in delIds)
            {
                var job = _sysJobService.GetById(id);
                if (job != null)
                {
                    _ = RemoveFromSchedulerAsync(job);
                }
            }

            return _sysJobService.Delete(delIds)
                ? new ReturnData()
                : new ReturnData(500, "删除失败！");
        }

        public ReturnData ChangeStatus(SysJobModel model)
        {
            var job = _sysJobService.GetById(model.JobId);
            if (job == null)
                return new ReturnData(500, "任务不存在！");

            // 启用时重新校验 cron
            if (model.Status == "0")
            {
                var cronErr = ValidateCron(job.CronExpression);
                if (cronErr != null)
                    return new ReturnData(500, $"Cron 表达式无效，无法启用：{cronErr}");
            }

            if (!_sysJobService.ChangeStatus(model))
                return new ReturnData(500, "状态修改失败！");

            // 启用 → 同步到 Quartz；暂停 → 从 Quartz 移除
            _ = SyncToSchedulerAsync(job);

            return new ReturnData();
        }

        public ReturnData Run(long id)
        {
            var job = _sysJobService.GetById(id);
            if (job == null)
                return new ReturnData(500, "任务不存在！");

            var scheduler = GlobalContext.ServiceProvider!.GetRequiredService<JobSchedulerService>();
            _ = scheduler.TriggerJobNowAsync(job);
            return new ReturnData();
        }

        // ============ Quartz 同步 ============

        private static async Task SyncToSchedulerAsync(SysJobModel job)
        {
            try
            {
                var scheduler = GlobalContext.ServiceProvider!.GetRequiredService<JobSchedulerService>();
                // 已存在则先删再建，确保 cron 和状态一致
                if (await scheduler.ExistsAsync(job))
                    await scheduler.DeleteJobAsync(job);

                if (job.Status == "0")
                    await scheduler.AddJobAsync(job);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JobBiz] 调度器同步失败 [{job.JobName}]: {ex.Message}");
            }
        }

        private static async Task RemoveFromSchedulerAsync(SysJobModel job)
        {
            try
            {
                var scheduler = GlobalContext.ServiceProvider!.GetRequiredService<JobSchedulerService>();
                if (await scheduler.ExistsAsync(job))
                    await scheduler.DeleteJobAsync(job);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JobBiz] 调度器移除失败 [{job.JobName}]: {ex.Message}");
            }
        }

        /// <summary>
        /// 校验 Cron 表达式，返回 null 表示有效，否则返回错误信息
        /// </summary>
        private static string? ValidateCron(string? cronExpression)
        {
            if (string.IsNullOrWhiteSpace(cronExpression))
                return "Cron 表达式不能为空";
            if (!CronExpression.IsValidExpression(cronExpression))
                return $"表达式格式无效：{cronExpression}";
            return null;
        }
    }
}
