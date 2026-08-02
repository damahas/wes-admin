using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.FlowManage
{
    public class FlowInstanceTaskParam
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long InstanceId { get; set; }
        [JsonConverter(typeof(LongToStringConverter))]
        public long TaskUserId { get; set; }
        [JsonConverter(typeof(LongToStringConverter))]
        public long ActualUserId { get; set; }
        public string? Comments { get; set; }
        public System.DateTime? HandleTime { get; set; }
        public int IsRecall { get; set; }
    }
}
