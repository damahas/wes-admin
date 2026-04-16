using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils;
using Wes.ViewModel.FlowManage;

namespace Wes.Business.FlowManage.FlowRuntime
{
    public class StartNode : IBaseNode
    {
        private ISqlSugarClient db { set; get; }
        private IFlowInstanceService instanceService { set; get; }
        private IFlowInstanceNodeService nodeService { set; get; }
        private IFlowInstanceTaskService taskService { set; get; }
        private FlowInstanceModel flowInstanceModel { set; get; }
        private FlowVersionModel flow { set; get; }

        public StartNode(ISqlSugarClient sqlSugarClient, FlowInstanceModel flowInstanceModel, FlowVersionModel flowVersionModel)
        {
            this.db = sqlSugarClient;
            this.flowInstanceModel = flowInstanceModel;
            this.flow = flowVersionModel;
            instanceService = GlobalContext.ServiceProvider.GetService<IFlowInstanceService>();
            nodeService = GlobalContext.ServiceProvider.GetService<IFlowInstanceNodeService>();
            taskService = GlobalContext.ServiceProvider.GetService<IFlowInstanceTaskService>();
        }

        // 初始化 发起流程
        public FlowRunResultEnum Exec(out string errMsg)
        {
            errMsg = "";
            var startNode = flow.nodes.Find(p => p?.meta?.type == FlowNodeTypeEnum.start);
            if (startNode == null)
            {
                errMsg = "找不到流程开始节点";
                return FlowRunResultEnum.error;
            }
            flowInstanceModel.InstanceStatus = FlowStatusEnum.doing;
            instanceService.Save(flowInstanceModel);
            var node = new FlowInstanceNodeModel()
            {
                InstanceId = flowInstanceModel.InstanceId,
                NodeId = startNode.id,
                NodeName = startNode.meta?.name,
                NodeType = FlowNodeTypeEnum.start,
                NodeResult = FlowStatusEnum.pass,
            };
            nodeService.Save(node, db);
            taskService.Save(new FlowInstanceTaskModel()
            {
                InstanceId = flowInstanceModel.InstanceId,
                InstanceNodeId = node.InstanceNodeId,
                TaskUserId = GlobalContext.CurrentUser.UserId,
                ActualUserId = GlobalContext.CurrentUser.UserId,
                TaskResult = FlowStatusEnum.pass,
                HandleTime = DateTime.Now
            }, db);
            return FlowRunResultEnum.next;
        }

        public FlowRunResultEnum Next(FlowVersionNodeModel node, FlowVersionNodeModel preNode, out string errorMsg, out List<long> userIds)
        {
            throw new Exception("开始节点有误，请检查流程开始节点");
        }
    }
}
