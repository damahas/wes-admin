using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;

namespace Wes.ViewModel.SystemManage
{
    public class RoleTreeInfo
    {
        public List<long> CheckedKeys { set; get; }

        public List<RoleTreeDetailInfo> RoleTrees { set; get; }
    }

    public class RoleTreeDetailInfo
    {
        public long Id { set; get; }

        [JsonIgnore]
        public long ParentId { set; get; }

        public string Label { set; get; }

        public List<RoleTreeDetailInfo> children { set; get; }
    }
}

