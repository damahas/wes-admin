using System;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.SystemManage
{
    public class OnlineInfo
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long TokenId { set; get; }
        public string UserName { set; get; }
        public string Browser { set; get; }
        public string DeptName { set; get; }
        public string Ipaddr { set; get; }
        public string LoginLocation { set; get; }
        public string Os { set; get; }
        public DateTime LoginTime { set; get; }
        public DateTime ExpirationTime { set; get; }
    }
}

