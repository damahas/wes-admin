using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Wes.Utils.Extension
{
    public static class StringExtension
    {
        #region 字符串操作

        /// <summary>
        /// 字符串首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToFirstCharUpper(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return str.First().ToString().ToUpper() + str.Substring(1);
        }

        /// <summary>
        /// 字符串首字母小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToFirstCharLower(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return str.First().ToString().ToLower() + str.Substring(1);
        }

        /// <summary>
        /// 字符串单词首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        public static string ToWordFirstCharUpper(this string str, char splitChar)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return string.Join("", str.Split(splitChar).Select(p => p.First().ToString().ToUpper() + p.Substring(1)));
        }

        #endregion

        #region 数据类型转换

        /// <summary>
        /// 字符串转成long
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ToLong(this string str)
        {
            try
            {
                return Convert.ToInt64(str);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// “,”隔开字符串 => long数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<long> ToLongList(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            return str.Split(",").Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.ToLong()).ToList();
        }

        /// <summary>
        /// “,”隔开字符串 => string数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> ToStringList(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            return str.Split(",").Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
        }

        #endregion

        #region 数据库名转换

        /// <summary>
        /// 获取mysql连接字符串中数据库名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToDbName(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            return Regex.Match(str, @"(?<=DataBase=)[^;]*").Value;
        }

        #endregion

    }
}
