using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using System.Linq;

namespace Wes.ViewModel.SystemManage
{
    public class ProfileInfo<T> : ReturnData
    {
        public UserInfo Data { set; get; }

        public string PostGroup
        {
            get
            {
                if (Data != null && Data.Posts != null && Data.Posts.Any())
                {
                    return Data.Posts.First().PostName;
                }
                return "";
            }
        }

        public string RoleGroup
        {
            get
            {
                if (Data != null && Data.Roles != null && Data.Roles.Any())
                {
                    return Data.Roles.First().RoleName;
                }
                return "";
            }
        }
    }
}
