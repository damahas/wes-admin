using Microsoft.Extensions.DependencyInjection;
using NetTaste;
using NPOI.HSSF.Record.Chart;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.FlowManage;

namespace Wes.Business.FlowManage.FlowRuntime
{
    public class FlowEngine
    {
        /// <summary>
        /// 流程实例
        /// </summary>
        public FlowVersionModel Flow { set; get; }

        public FlowInstanceModel FlowInstance { set; get; }

        /// <summary>
        /// taskId 如果为0 则是发起
        /// </summary>
        public FlowRunModel FlowRunModel { set; get; }

        /// <summary>
        /// 深度 防止自动处理进入死循环容错
        /// </summary>
        public int depth { set; get; }

        public ISqlSugarClient Db { set; get; }
        public IFlowInstanceNodeService nodeService { set; get; }

        public FlowEngine()
        {
            nodeService = GlobalContext.ServiceProvider.GetService<IFlowInstanceNodeService>();
            depth = 0;
        }

        public FlowEngine(FlowVersionModel flow, FlowRunModel flowRunModel, ISqlSugarClient sugarClient) : this()
        {
            Flow = flow;
            FlowRunModel = flowRunModel;
            Db = sugarClient;
        }

        public FlowEngine(FlowVersionModel flow, ISqlSugarClient sugarClient) : this(flow, null, sugarClient)
        {
        }

        public FlowRunResultEnum Run(out string errorMsg, out List<long> userIds)
        {
            errorMsg = "";
            userIds = new List<long>();
            long InstanceTaskId = FlowRunModel?.InstanceTaskId ?? 0;
            var nodeInfo = InstanceTaskId > 0 ? nodeService.GetByTaskId(InstanceTaskId)
                : new FlowInstanceNodeModel()
                {
                    NodeId = Flow.nodes.Find(p => p.meta?.type == FlowNodeTypeEnum.start)?.id,
                    NodeType = FlowNodeTypeEnum.start
                };
            var baseNode = GetNode(nodeInfo?.NodeType);
            if (baseNode == null)
            {
                errorMsg = "获取节点错误，请检查当前节点类型";
                return FlowRunResultEnum.error;
            }
            // 执行
            var execResult = baseNode.Exec(out errorMsg);
            switch (execResult)
            {
                case FlowRunResultEnum.success:
                    return FlowRunResultEnum.success;
                case FlowRunResultEnum.error:
                    return FlowRunResultEnum.error;
                case FlowRunResultEnum.next:
                default:
                    break;
            }
            // 流转
            var node = Flow.nodes.Find(p => p.id == nodeInfo.NodeId);
            if (node == null)
            {
                errorMsg = "当前节点不存在，流程图发起后发生变更";
                return FlowRunResultEnum.error;
            }
            depth = 0;
            return ToNext(node, out errorMsg, out userIds);
        }

        // 找下一个节点
        public FlowRunResultEnum ToNext(FlowVersionNodeModel nodeInfo, out string errorMsg, out List<long> userIds)
        {
            errorMsg = "";
            userIds = new List<long>();
            // 防止死循环
            if (depth > 16)
            {
                errorMsg = "已自动处理十六个节点，流程可能进入死循环，请检查流程";
                return FlowRunResultEnum.error;
            }
            depth++;
            var nodeIds = Flow.lines.Where(p => p.startId == nodeInfo.id).Select(p => p.endId);
            var nodes = Flow.nodes.FindAll(p => nodeIds.Contains(p.id));
            if (nodes == null || nodes.Count == 0)
            {
                // 通知节点后面可以没有下一节点
                if (nodeInfo.meta.type == FlowNodeTypeEnum.notice)
                {
                    return FlowRunResultEnum.success;
                }
                errorMsg = $"找不到 [{nodeInfo.meta?.name}] 下一个节点，请检查";
                return FlowRunResultEnum.error;
            }
            foreach (var node in nodes)
            {
                var baseNode = GetNode(node.meta?.type);
                if (baseNode == null)
                {
                    errorMsg = $"获取节点错误，请检查[{node.meta?.name}]节点类型";
                    return FlowRunResultEnum.error;
                }
                var runResult = baseNode.Next(node, nodeInfo, out errorMsg, out userIds);
                if (runResult == FlowRunResultEnum.next)
                {
                    return ToNext(node, out errorMsg, out userIds);
                }
                return runResult;
            }
            return FlowRunResultEnum.success;
        }

        public IBaseNode GetNode(FlowNodeTypeEnum? nodeType)
        {
            if (nodeType == null)
            {
                return null;
            }
            switch (nodeType)
            {
                case FlowNodeTypeEnum.start: return new StartNode(Db, FlowInstance, Flow);
                case FlowNodeTypeEnum.end: return new EndNode(Db, FlowInstance);
                case FlowNodeTypeEnum.task: return new TaskNode(Db, FlowInstance, Flow, FlowRunModel);
                //case FlowNodeTypeEnum.notice: return new NoticeNode();
                default: return null;
            }
        }
    }
}
