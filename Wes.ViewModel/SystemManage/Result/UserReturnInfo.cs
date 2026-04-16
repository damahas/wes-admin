using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;

namespace Wes.ViewModel.SystemManage
{
    public class UserReturnInfo : ReturnData
    {
        public UserReturnInfo()
        {
            this.Code = 200;
        }

        public UserReturnInfo(long code, string msg) : base(code, msg)
        {
        }

        public UserModel Data { set; get; }

        public List<long> PostIds
        {
            get
            {
                return Data?.PostIds ?? new List<long>();
            }
        }

        public List<SysPostModel> Posts { set; get; }

        public List<long> RoleIds
        {
            get
            {
                return Data?.RoleIds ?? new List<long>();
            }
        }

        public List<SysRoleModel> Roles { set; get; }
    }
}
