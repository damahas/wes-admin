using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wes.Entity;

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
        public List<SysUserEntity> Users { set; get; }
    }
}
