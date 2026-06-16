using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wes.Business.FlowManage.FlowRuntime;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.ViewModel.FlowManage;

namespace Wes.Business.FlowManage.Business.FlowRuntime
{
    public class NoticeNode : IBaseNode
    {
        private ISqlSugarClient db { set; get; }
        private IFlowInstanceService instanceService { set; get; }
        private IFlowInstanceNodeService nodeService { set; get; }
        private IFlowInstanceTaskService taskService { set; get; }
        private ISysUserService userService { set; get; }
        private FlowInstanceModel flowInstanceModel { set; get; }
        private FlowVersionModel flow { set; get; }
        private FlowRunModel flowRunModel { set; get; }

        public NoticeNode(ISqlSugarClient sqlSugarClient, FlowInstanceModel flowInstanceModel, FlowRunModel flowRunModel)
        {
            this.db = sqlSugarClient;
            this.flowInstanceModel = flowInstanceModel;
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
            if (task == null)
            {
                errMsg = "找不到任务节点";
                return FlowRunResultEnum.error;
            }
            task.TaskResult = FlowStatusEnum.pass;
            taskService.Save(task);
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
                NodeType = FlowNodeTypeEnum.notice,
                PreNodeId = preNode.id,
                NodeResult = FlowStatusEnum.pass,
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
                        result.Add(flowInstanceModel.CreateBy.ToLong());
                        break;
                    case NodeHandleByEnum.leader:
                        result.Add(userService.GetLeaderIdByUserId(flowInstanceModel.CreateBy.ToLong()));
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
            return FlowRunResultEnum.next;
        }
    }
}
