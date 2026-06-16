using System;
using System.Collections.Generic;

namespace Wes.ViewModel.SystemManage
{
    public class RoleDeptModel
    {
        public long RoleId { set; get; }

        public string DataScope { set; get; }

        public bool DeptCheckStrictly { set; get; }

        public List<long> DeptIds { set; get; }
    }
}

