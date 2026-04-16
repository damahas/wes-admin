using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Wes.Utils.Extension
{
    /// <summary>
    /// 实体复制
    /// </summary>
    public static class EntityCopyExtension
    {
        public static D ToEntityCopy<T, D>(this T t)
        {
            return JsonConvert.DeserializeObject<D>(JsonConvert.SerializeObject(t));
        }
    }
}
