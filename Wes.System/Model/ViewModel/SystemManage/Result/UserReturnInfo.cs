using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
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

        public List<SysPostEntity> Posts { set; get; }

        public List<long> RoleIds
        {
            get
            {
                return Data?.RoleIds ?? new List<long>();
            }
        }

        public List<SysRoleEntity> Roles { set; get; }
    }
}
