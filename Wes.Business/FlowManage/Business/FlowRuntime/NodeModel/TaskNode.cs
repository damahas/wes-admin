using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.ViewModel.FlowManage;

namespace Wes.Business.FlowManage.FlowRuntime
{
    public class TaskNode : IBaseNode
    {
        private ISqlSugarClient db { set; get; }
        private IFlowInstanceService instanceService { set; get; }
        private IFlowInstanceNodeService nodeService { set; get; }
        private IFlowInstanceTaskService taskService { set; get; }
        private ISysUserService userService { set; get; }
        private FlowInstanceModel flowInstanceModel { set; get; }
        private FlowVersionModel flow { set; get; }

        private FlowRunModel flowRunModel { set; get; }

        public TaskNode(ISqlSugarClient sqlSugarClient, FlowInstanceModel flowInstanceModel, FlowVersionModel flowVersionModel, FlowRunModel flowRunModel)
        {
            this.db = sqlSugarClient;
            this.flowInstanceModel = flowInstanceModel;
            this.flow = flowVersionModel;
            instanceService = GlobalContext.ServiceProvider.GetService<IFlowInstanceService>();
            nodeService = GlobalContext.ServiceProvider.GetService<IFlowInstanceNodeService>();
            taskService = GlobalContext.ServiceProvider.GetService<IFlowInstanceTaskService>();
            userService = GlobalContext.ServiceProvider.GetService<ISysUserService>();
            this.flowRunModel = flowRunModel;
        }

        public FlowRunResultEnum Exec(out string errMsg)
        {
            errMsg = null;
            var task = taskService.GetById(flowRunModel.InstanceTaskId);
            var node = nodeService.GetByTaskId(flowRunModel.InstanceTaskId);
            flowInstanceModel.CurrentNodeId = node.InstanceNodeId;
            instanceService.Save(flowInstanceModel, db);
            if (task == null)
            {
                errMsg = "找不到任务节点";
                return FlowRunResultEnum.error;
            }
            if (task.ActualUserId != GlobalContext.CurrentUser.UserId)
            {
                errMsg = "当前任务节点处理人非当前登录人，请检查";
                return FlowRunResultEnum.error;
            }
            switch (flowRunModel.TaskResult)
            {
                case FlowStatusEnum.pass:
                    task.TaskResult = FlowStatusEnum.pass;
                    task.Comments = flowRunModel.Comments;
                    taskService.Save(task, db);
                    var flowNode = flow.nodes.Find(p => p.id == node.NodeId);
                    if (flowNode == null)
                    {
                        errMsg = $"找不到“{node.NodeName}”节点，请检查流程图";
                        return FlowRunResultEnum.error;
                    }
                    // 处理方式
                    switch (flowNode.meta.handleRule)
                    {
                        case NodeHandleTypeEnum.one:
                        case NodeHandleTypeEnum.select:
                            node.NodeResult = FlowStatusEnum.pass;
                            nodeService.Save(node, db);
                            // 自动处理任务
                            taskService.AutoHandleNodeTask(node.InstanceNodeId, task.InstanceTaskId, db);
                            return FlowRunResultEnum.next;
                        case NodeHandleTypeEnum.all:
                            // 所有人都得处理
                            var tasks = taskService.GetByNodeId(task.InstanceNodeId).Where(p => p.InstanceTaskId != task.InstanceTaskId).ToList();
                            if (!tasks.Exists(p => p.TaskResult == FlowStatusEnum.start || p.TaskResult == FlowStatusEnum.doing))
                            {
                                node.NodeResult = FlowStatusEnum.pass;
                                nodeService.Save(node, db);
                                return FlowRunResultEnum.next;
                            }
                            return FlowRunResultEnum.success;
                        default:
                            errMsg = $"“{node.NodeName} ”节点处理方式有误，请检查流程";
                            return FlowRunResultEnum.error;
                    }
                case FlowStatusEnum.unpass:
                    // 更新任务状态
                    task.TaskResult = FlowStatusEnum.unpass;
                    task.Comments = flowRunModel.Comments;
                    taskService.Save(task, db);
                    // 更新任务其他节点
                    taskService.AutoHandleNodeTask(node.InstanceNodeId, task.InstanceTaskId, db);
                    // 更新节点状态
                    node.NodeResult = FlowStatusEnum.unpass;
                    nodeService.Save(node, db);
                    // 更新实例状态
                    flowInstanceModel.InstanceStatus = FlowStatusEnum.unpass;
                    instanceService.Save(flowInstanceModel, db);
                    return FlowRunResultEnum.success;
                case FlowStatusEnum.delegation:
                    if (!flowRunModel.SelectedUser.ContainsKey("delegation"))
                    {
                        errMsg = "请选择委托人";
                        return FlowRunResultEnum.error;
                    }
                    task.TaskResult = FlowStatusEnum.delegation;
                    task.Comments = flowRunModel.Comments;
                    taskService.Save(task, db);
                    var delegationTask = task.ToEntityCopy<FlowInstanceTaskModel, FlowInstanceTaskModel>();
                    delegationTask.InstanceTaskId = 0;
                    delegationTask.TaskUserId = flowRunModel.SelectedUser["delegation"];
                    delegationTask.ActualUserId = flowRunModel.SelectedUser["delegation"];
                    delegationTask.TaskResult = FlowStatusEnum.start;
                    delegationTask.RecallTaskId = task.InstanceTaskId;
                    delegationTask.IsRecall = flowRunModel.IsRecall;
                    taskService.Save(delegationTask, db);
                    return FlowRunResultEnum.success;
                default: break;
            }
            return FlowRunResultEnum.success;
        }

        public FlowRunResultEnum Next(FlowVersionNodeModel node, FlowVersionNodeModel preNode, out string errorMsg, out List<long> userIds)
        {
            errorMsg = "";
            userIds = new List<long>();
            var taskNode = new FlowInstanceNodeModel()
            {
                InstanceId = flowInstanceModel.InstanceId,
                NodeId = node.id,
                NodeName = node.meta?.name,
                NodeType = FlowNodeTypeEnum.task,
                PreNodeId = preNode.id,
                NodeResult = FlowStatusEnum.start,
            };
            nodeService.Save(taskNode, db);
            // 如果没有处理人直接跳过
            if ((node?.meta?.handleBy?.Count ?? 0) == 0)
            {
                taskNode.NodeResult = FlowStatusEnum.pass;
                nodeService.Save(taskNode, db);
                return FlowRunResultEnum.next;
            }
            // 获取人员列表
            List<long> result = new List<long>();
            foreach (var handle in node.meta.handleBy)
            {
                switch (handle.type)
                {
                    case NodeHandleByEnum.author:
                        result.Add(userService.GetByAccout(flowInstanceModel.CreateBy)?.UserId ?? 0);
                        break;
                    case NodeHandleByEnum.leader:
                        result.Add(userService.GetLeaderIdByAccount(flowInstanceModel.CreateBy));
                        break;
                    case NodeHandleByEnum.role:
                        var roleUsers = userService.GetByRoleId(handle.handleId);
                        result.AddRange(roleUsers?.Select(p => p.UserId) ?? new List<long>());
                        break;
                    case NodeHandleByEnum.dept:
                        var deptUsers = userService.GetByDeptId(handle.handleId);
                        result.AddRange(deptUsers?.Select(p => p.UserId) ?? new List<long>());
                        break;
                    case NodeHandleByEnum.user:
                        result.Add(handle.handleId);
                        break;
                    default: break;
                }
            }
            userIds = result;
            // 根据不同处理类型选择
            switch (node?.meta?.handleRule)
            {
                case NodeHandleTypeEnum.select:
                    if (flowRunModel.SelectedUser.ContainsKey(node.id))
                    {
                        taskService.Save(new FlowInstanceTaskModel()
                        {
                            InstanceId = flowInstanceModel.InstanceId,
                            InstanceNodeId = taskNode.InstanceNodeId,
                            TaskUserId = flowRunModel.SelectedUser[node.id],
                            ActualUserId = flowRunModel.SelectedUser[node.id],
                            TaskResult = FlowStatusEnum.start,
                            HandleTime = DateTime.Now
                        });
                        return FlowRunResultEnum.success;
                    }
                    return FlowRunResultEnum.select;
                case NodeHandleTypeEnum.one:
                case NodeHandleTypeEnum.all:
                default:
                    if (result.Count == 0)
                    {
                        taskNode.NodeResult = FlowStatusEnum.pass;
                        nodeService.Save(taskNode, db);
                        return FlowRunResultEnum.next;
                    }
                    foreach (var userId in result)
                    {
                        taskService.Save(new FlowInstanceTaskModel()
                        {
                            InstanceId = flowInstanceModel.InstanceId,
                            InstanceNodeId = taskNode.InstanceNodeId,
                            TaskUserId = userId,
                            ActualUserId = userId,
                            TaskResult = FlowStatusEnum.start,
                            HandleTime = DateTime.Now
                        });
                    }
                    break;
            }
            return FlowRunResultEnum.success;
        }
    }
}
