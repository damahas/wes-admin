using System;
using System.Collections.Generic;
using System.Text;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.SystemManage
{
    public class RoleUserParam
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { set; get; }

        public string? UserName { set; get; }

        public string? Phonenumber { set; get; }

        public string? Account { set; get; }

        public bool IsNoExist { set; get; }
    }

    public class RoleUserSaveParam
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { set; get; }
        public string? userIds { set; get; }
        [JsonConverter(typeof(LongToStringConverter))]
        public long userId { set; get; }
    }
}
