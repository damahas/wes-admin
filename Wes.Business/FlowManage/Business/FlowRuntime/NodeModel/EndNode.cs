using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.PTG;
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
using Wes.ViewModel.FlowManage;

namespace Wes.Business.FlowManage.FlowRuntime
{
    public class EndNode : IBaseNode
    {
        private ISqlSugarClient db { set; get; }
        private IFlowInstanceNodeService nodeService { set; get; }
        private IFlowInstanceTaskService taskService { set; get; }
        private FlowInstanceModel flowInstanceModel { set; get; }

        public EndNode(ISqlSugarClient sqlSugarClient, FlowInstanceModel flowInstanceModel)
        {
            this.db = sqlSugarClient;
            this.flowInstanceModel = flowInstanceModel;
            nodeService = GlobalContext.ServiceProvider.GetService<IFlowInstanceNodeService>();
            taskService = GlobalContext.ServiceProvider.GetService<IFlowInstanceTaskService>();
        }

        public FlowRunResultEnum Exec(out string errMsg)
        {
            throw new Exception("结束节点有误，请检查流程结束节点");
        }

        public FlowRunResultEnum Next(FlowVersionNodeModel node, FlowVersionNodeModel preNode, out string errorMsg, out List<long> userIds)
        {
            errorMsg = "";
            userIds = new List<long>();
            var endNode = new FlowInstanceNodeModel()
            {
                InstanceId = flowInstanceModel.InstanceId,
                NodeId = node.id,
                NodeName = node.meta?.name,
                NodeType = FlowNodeTypeEnum.end,
                PreNodeId = preNode.id,
                NodeResult = FlowStatusEnum.pass,
            };
            nodeService.Save(endNode, db);
            //taskService.Save(new FlowInstanceTaskModel()
            //{
            //    InstanceId = flowInstanceModel.InstanceId,
            //    InstanceNodeId = endNode.InstanceNodeId,
            //    TaskUserId = GlobalContext.CurrentUser.UserId,
            //    ActualUserId = GlobalContext.CurrentUser.UserId,
            //    TaskResult = FlowStatusEnum.pass,
            //    HandleTime = DateTime.Now
            //}, db);
            flowInstanceModel.InstanceStatus = FlowStatusEnum.pass;
            flowInstanceModel.CurrentNodeId = endNode.InstanceNodeId;
            db.Updateable(flowInstanceModel).ExecuteCommand();
            return FlowRunResultEnum.success;
        }
    }
}
