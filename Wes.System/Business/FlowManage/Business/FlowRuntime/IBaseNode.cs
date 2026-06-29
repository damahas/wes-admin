using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wes.DbModel;
using Wes.ViewModel.FlowManage;

namespace Wes.Business.FlowManage.FlowRuntime
{
    public interface IBaseNode
    {
        /// <summary>
        /// 负责处理节点审批
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public FlowRunResultEnum Exec(out string errMsg);
        // 处理下一个节点找人
        public FlowRunResultEnum Next(FlowVersionNodeModel node, FlowVersionNodeModel preNode, out string errorMsg, out List<long> userIds);

        //public void Hook(FlowVersionNodeModel node, FlowVersionNodeModel preNode);
    }
}
