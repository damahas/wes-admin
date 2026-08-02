using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.SystemManage
{
    public class UserStatusParam
    {
        [JsonConverter(typeof(LongToStringConverter))]
        public long userId { set; get; }
        public string status { set; get; }
    }
}
