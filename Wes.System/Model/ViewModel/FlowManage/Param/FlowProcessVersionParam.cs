using System;
using System.Collections.Generic;
using System.Text;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.FlowManage
{
    public class FlowProcessVersionParam
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long ProcessId { get; set; }

        public string? Version { get; set; }
    }
}
