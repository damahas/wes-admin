using System;
namespace Wes.Utils.Model
{
    /// <summary>
    /// 分页类
    /// </summary>
    public class ParamData<T>
    {
        public T Params { set; get; }

        public int PageNum { set; get; }

        public int PageSize { set; get; }

        public string OrderByColumn { set; get; }

        public bool IsAsc { set; get; }

        public bool IsOr { set; get; }
    }
}

