using System;
using System.Collections.Generic;
using System.Text;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.FlowManage
{
    public class FlowProcessParam
    {
        public string? ProcessCode { get; set; }

        public string? ProcessName { get; set; }

        [JsonConverter(typeof(LongToStringConverter))]
        public long ParentId { get; set; }

        [JsonConverter(typeof(LongToStringConverter))]
        public long CurVersionId { get; set; }

        public string? BusinessField { get; set; }

        public string? FormUrl { get; set; }

        public string? BackUrl { get; set; }
    }
}
