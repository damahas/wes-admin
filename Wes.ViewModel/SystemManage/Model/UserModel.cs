using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;

namespace Wes.ViewModel.SystemManage
{
    public class UserModel : SysUserModel
    {
        public List<long> PostIds { set; get; }

        public List<long> RoleIds { set; get; }
    }
}

