using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wes.Utils.Extension;

namespace Wes.Utils.Converter
{
    /// <summary>
    /// 将 long 序列化为字符串，避免前端 JavaScript（Number 仅 53 位精度）接收雪花 ID 时精度丢失。
    /// 用于 entity / model 的 Id 字段上标注 [JsonConverter(typeof(LongToStringConverter))]。
    /// </summary>
    public class LongToStringConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString().ToLong();
            }
            return reader.GetInt64();
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
