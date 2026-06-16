using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils
{
    /// <summary>
    /// excel导出字段特性
    /// </summary>
    public class ExportFieldAttr : Attribute
    {
        public int SortBy = 10;

        public string FieldName { set; get; }

        public string DicName { set; get; }
    }
}
