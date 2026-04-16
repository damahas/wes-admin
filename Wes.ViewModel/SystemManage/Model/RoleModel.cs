using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;

namespace Wes.ViewModel.SystemManage
{
    public class RoleModel: SysRoleModel
    {
        public List<long> MenuIds { set; get; }
    }
}
