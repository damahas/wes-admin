using System;
using System.Linq;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.DbModel;
using Wes.Utils.Model;
using System.Collections.Generic;
using Wes.ViewModel.FlowManage;
using Wes.Utils;
using Newtonsoft.Json;
using SqlSugar;
using Microsoft.VisualBasic;
using NPOI.SS.Formula.PTG;
using Wes.Business.FlowManage.FlowRuntime;

namespace Wes.Business
{
    public class FlowInstanceBiz : IFlowInstanceBiz
    {
        private IFlowInstanceService _flowInstanceService;
        private IFlowInstanceTaskService _flowInstanceTaskService;
        private IFlowInstanceNodeService _flowInstanceNodeService;
        private IFlowProcessVersionService _flowProcessVersionService;
        private IFlowProcessService _flowProcessService;
        private ISugarRepository _sugarRepository;
        private ISysUserService _sysUserService;

        public FlowInstanceBiz(IFlowInstanceService flowInstanceService,
            IFlowInstanceTaskService flowInstanceTaskService,
            IFlowInstanceNodeService flowInstanceNodeService,
            IFlowProcessVersionService flowProcessVersionService,
            IFlowProcessService flowProcessService,
            ISysUserService sysUserService,
            ISugarRepository sugarRepository)
        {
            _flowInstanceService = flowInstanceService;
            _flowInstanceTaskService = flowInstanceTaskService;
            _flowInstanceNodeService = flowInstanceNodeService;
            _flowProcessVersionService = flowProcessVersionService;
            _flowProcessService = flowProcessService;
            _sysUserService = sysUserService;
            _sugarRepository = sugarRepository;
        }

        public ResultData<FlowInstanceModel> GetById(long id)
        {
            return new ResultData<FlowInstanceModel>(_flowInstanceService.GeDetailById(id));
        }

        public ResultData<List<FlowInstanceModel>> GetAll()
        {
            return new ResultData<List<FlowInstanceModel>>(_flowInstanceService.GetAll());
        }

        public RowData<FlowInstanceModel> GetList(ParamData<FlowInstanceParam> param)
        {
            int total = 0;
            RowData<FlowInstanceModel> result = new RowData<FlowInstanceModel>(_flowInstanceService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Save(FlowInstanceModel model)
        {
            if (_flowInstanceService.Save(model))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            var result = _flowInstanceService.Delete(delIds);
            _flowInstanceNodeService.DeleteByInstanceId(delIds);
            _flowInstanceTaskService.DeleteByInstanceId(delIds);
            if (result)
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        #region 流程流转

        public ResultData<FlowRunModel> GetRunIni(string processCode, long businessId)
        {
            FlowRunModel result = new FlowRunModel()
            {
                ProcessCode = processCode,
                BusinessId = businessId,
                InstanceStatus = FlowStatusEnum.start,
            };
            var instance = _flowInstanceService.GetByBusinessId(processCode, businessId);
            if (instance == null)
            {
                var process = _flowProcessService.GetByCode(processCode);
                if (process == null)
                {
                    return new ResultData<FlowRunModel>(500, "流程有误，请检查流程编码");
                }
                return new ResultData<FlowRunModel>(result);
            }
            result = new FlowRunModel()
            {
                ProcessCode = processCode,
                BusinessId = instance.BusinessId,
                BusinessCode = instance.BusinessCode,
                IsUrgent = instance.IsUrgent,
                InstanceStatus = instance.InstanceStatus,
                ExtendInfo = instance.ExtendInfo,
            };
            var task = _flowInstanceTaskService.GetByUserId(instance.InstanceId, GlobalContext.CurrentUser.UserId);
            if (task != null)
            {
                result.InstanceTaskId = task.InstanceTaskId;
                result.Node = _flowInstanceNodeService.GetById(task.InstanceNodeId);
            }
            return new ResultData<FlowRunModel>(result);
        }

        public ResultData<FlowRunModel> Exec(FlowRunModel flowRunModel)
        {
            var db = _sugarRepository.Context;
            FlowInstanceModel flowInstance = _flowInstanceService.GetByBusinessId(flowRunModel.ProcessCode, flowRunModel.BusinessId);
            FlowEngine flowEngine;
            try
            {
                db.Ado.BeginTran();
                // 发起流程
                if (flowRunModel.InstanceStatus == FlowStatusEnum.start)
                {
                    if (flowInstance != null)
                    {
                        return GetErrorReturn("流程已发起，请勿重复发起", db);
                    }
                    flowEngine = new FlowEngine(GetVersion(flowRunModel.ProcessCode, db), db);
                    if (flowEngine.Flow == null)
                    {
                        return GetErrorReturn("找不到流程版本", db);
                    }
                    flowInstance = new FlowInstanceModel()
                    {
                        ProcessId = flowEngine.Flow.processId,
                        VersionId = flowEngine.Flow.versionId,
                        BusinessId = flowRunModel.BusinessId,
                        BusinessCode = flowRunModel.BusinessCode,
                        IsUrgent = flowRunModel.IsUrgent,
                        InstanceStatus = FlowStatusEnum.start,
                        ExtendInfo = flowRunModel.ExtendInfo
                    };
                    _flowInstanceService.Save(flowInstance, db);
                }
                else
                {
                    if (flowInstance == null)
                    {
                        return GetErrorReturn("找不到流程实例，请先发起流程", db);
                    }
                    flowEngine = new FlowEngine(GetVersion(flowInstance.VersionId, db), flowRunModel, db);
                }
                // 设置流程实例
                flowEngine.FlowInstance = flowInstance;
                // 流程执行
                string errMsg = "";
                List<long> userIds = new List<long>();
                var runResult = flowEngine.Run(out errMsg, out userIds);
                switch (runResult)
                {
                    case FlowRunResultEnum.error:
                        return GetErrorReturn(errMsg, db);
                    case FlowRunResultEnum.select:
                        //flowRunModel.SelectUsers = _sysUserService.GetList(userIds);
                        db.Ado.RollbackTran();
                        return new ResultData<FlowRunModel>()
                        {
                            Code = 100,
                            Data = flowRunModel
                        };
                    case FlowRunResultEnum.success:
                    default:
                        break;
                }
                db.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                db.Ado.RollbackTran();
                return new ResultData<FlowRunModel>(500, ex.Message);
            }
            return new ResultData<FlowRunModel>();
        }

        public ReturnData TaskDelegate(long taskId, long userId)
        {
            var task = _flowInstanceTaskService.GetById(taskId);
            var user = _sysUserService.GetById(userId);
            if (task == null || task.IsDel == 1)
            {
                return new ReturnData(500, "找不到任务节点");
            }
            if (user == null)
            {
                return new ReturnData(500, "找不到委托人");
            }
            if (task.TaskResult != FlowStatusEnum.start)
            {
                return new ReturnData(500, "任务节点已处理，请勿重复处理");
            }
            var newTask = task.ToEntityCopy<FlowInstanceTaskModel, FlowInstanceTaskModel>();
            task.TaskResult = FlowStatusEnum.delegation;
            task.Comments = $"{GlobalContext.CurrentUser.UserName} 代理委托给 {user.UserName}";
            _flowInstanceTaskService.Save(task);
            newTask.InstanceTaskId = 0;
            newTask.ActualUserId = userId;
            newTask.Comments = null;
            _flowInstanceTaskService.Save(newTask);
            return new ReturnData();
        }

        public ReturnData NodeReset(long nodeId)
        {
            var node = _flowInstanceNodeService.GetById(nodeId);
            if (node == null || node.IsDel == 1)
            {
                return new ReturnData(500, "找不到节点");
            }
            var nodes = _flowInstanceNodeService.GetResetNodes(node.InstanceId, nodeId);
            _flowInstanceNodeService.DeleteByNodeId(nodes.Select(p => p.InstanceNodeId).ToList());
            return new ReturnData();
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <param name="processCode"></param>
        /// <returns></returns>
        private FlowVersionModel GetVersion(string processCode, ISqlSugarClient sqlSugarClient)
        {
            var version = _flowProcessVersionService.GetByProcessCode(processCode);
            if (version == null)
            {
                return null;
            }
            if (version.IsLock == 0)
            {
                version.IsLock = 1;
                _flowProcessVersionService.Save(version, sqlSugarClient);
            }
            FlowVersionModel result = JsonConvert.DeserializeObject<FlowVersionModel>(version.Content);
            result.versionId = version.VersionId;
            result.processId = version.ProcessId;
            return result;
        }

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        private FlowVersionModel GetVersion(long versionId, ISqlSugarClient sqlSugarClient)
        {
            var version = _flowProcessVersionService.GetById(versionId);
            if (version == null)
            {
                return null;
            }
            if (version.IsLock == 0)
            {
                version.IsLock = 1;
                _flowProcessVersionService.Save(version, sqlSugarClient);
            }
            FlowVersionModel result = JsonConvert.DeserializeObject<FlowVersionModel>(version.Content);
            result.versionId = version.VersionId;
            result.processId = version.ProcessId;
            return result;
        }

        /// <summary>
        /// 错误返回
        /// </summary>
        /// <param name="errMsg"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private ResultData<FlowRunModel> GetErrorReturn(string errMsg, ISqlSugarClient db)
        {
            db.Ado.RollbackTran();
            return new ResultData<FlowRunModel>(500, errMsg);
        }
        #endregion
    }
}
