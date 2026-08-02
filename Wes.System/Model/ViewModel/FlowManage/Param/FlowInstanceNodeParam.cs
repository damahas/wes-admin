using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.FlowManage
{
    public class FlowInstanceNodeParam
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long InstanceId { get; set; }
        public string? NodeId { get; set; }
        public string? NodeName { get; set; }
        public string? PreNodeId { get; set; }
    }
}
