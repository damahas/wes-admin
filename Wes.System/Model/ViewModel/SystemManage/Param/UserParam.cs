using System;
using System.Collections.Generic;
using System.Text;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.SystemManage
{
    public class UserParam
    {
        public string? Account { set; get; }

        public string? UserName { set; get; }

        public string? Phonenumber { set; get; }

        public string? Status { set; get; }

        [JsonConverter(typeof(LongToStringConverter))]
        public long DeptId { set; get; }

        public DateTime? BeginTime { set; get; }

        public DateTime? EndTime { set; get; }
    }
}
