using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Wes.Utils.Converter;

namespace Wes.ViewModel.SystemManage
{
    public class RoleTreeInfo
    {
        public List<long> CheckedKeys { set; get; }

        public List<RoleTreeDetailInfo> RoleTrees { set; get; }
    }

    public class RoleTreeDetailInfo
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long Id { set; get; }

        [JsonIgnore]
        public long ParentId { set; get; }

        public string Label { set; get; }

        public List<RoleTreeDetailInfo> children { set; get; }
    }
}

