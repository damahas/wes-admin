using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    public class DbTableParam
    {
        public string TableName { set; get; }

        public string TableComment { set; get; }

        public DateTime? BeginTime { set; get; }

        public DateTime? EndTime { set; get; }
    }
}
