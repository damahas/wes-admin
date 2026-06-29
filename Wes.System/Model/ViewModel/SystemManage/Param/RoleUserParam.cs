using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    public class RoleUserParam
    {
        public long RoleId { set; get; }

        public string? UserName { set; get; }

        public string? Phonenumber { set; get; }

        public string? Account { set; get; }

        public bool IsNoExist { set; get; }
    }

    public class RoleUserSaveParam
    {
        public long RoleId { set; get; }
        public string? userIds { set; get; }
        public long userId { set; get; }
    }
}
