using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.SystemManage
{
    public class FileParam
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public string TableName { get; set; }
        [JsonConverter(typeof(LongToStringConverter))]
        public long? TableId { get; set; }
    }
}
