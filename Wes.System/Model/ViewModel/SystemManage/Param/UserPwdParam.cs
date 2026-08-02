using System;
using Wes.Utils.Converter;
using System.Text.Json.Serialization;
namespace Wes.ViewModel.SystemManage
{
	public class UserPwdParam
	{
		[JsonConverter(typeof(LongToStringConverter))]
		public long userId { set; get; }

		public string Password { set; get; }
	}
}

