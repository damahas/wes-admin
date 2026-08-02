using System;
using System.Collections.Generic;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.SystemManage
{
    public class RoleDeptModel
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { set; get; }

        public string DataScope { set; get; }

        public bool DeptCheckStrictly { set; get; }

        public List<long> DeptIds { set; get; }
    }
}

