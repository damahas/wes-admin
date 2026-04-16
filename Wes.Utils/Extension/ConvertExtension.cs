using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils.Extension
{
    public static class ConvertExtension
    {
        public static decimal ToDecimal(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            try
            {
                return Convert.ToDecimal(str);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
