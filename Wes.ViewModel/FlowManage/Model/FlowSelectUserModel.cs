using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wes.DbModel;

namespace Wes.ViewModel.FlowManage
{
    public class FlowSelectUserModel
    {
        /// <summary>
        /// 选择人员节点
        /// </summary>
        public string NodeId { set; get; }

        /// <summary>
        /// 待选择人员
        /// </summary>
        public List<SysUserModel> Users { set; get; }
    }
}
