using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Wes.Utils.Extension
{
    public static class JsonElementExtensions
    {
        /// <summary>
        /// 将 JsonElement 转换为原始的 CLR 类型值。
        /// 支持：null、bool、数字（int/long/ulong/decimal/double）、string、对象（转为 Dictionary）、数组（转为 List）。
        /// </summary>
        public static object? ToRawObject(this object element)
        {
            if (element is JsonElement elem && elem.ValueKind == JsonValueKind.Object)
                return elem.ToObjectIfNeeded();
            return element;
        }

        /// <summary>
        /// 仅当 JsonElement 为 Object 类型时，转换为 Dictionary&lt;string, object?&gt;；
        /// 否则直接返回 JsonElement 本身（或可以返回 null，根据业务决定）。
        /// </summary>
        public static object? ToObjectIfNeeded(this JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var prop in element.EnumerateObject())
                {
                    // 注意：这里不对属性值做递归转换（保持为 JsonElement 或简单值）
                    // 若需要深层处理，可递归调用 ToObjectIfNeeded
                    dict[prop.Name] = prop.Value.ValueKind == JsonValueKind.Object
                        ? prop.Value.ToObjectIfNeeded()  // 递归处理嵌套对象
                        : prop.Value;                     // 非对象直接存 JsonElement
                }
                return dict;
            }
            // 不是 Object，可以按需返回 element 本身，或 null，或抛出异常
            return element;
        }
    }
}
