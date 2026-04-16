using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    public class UserParam
    {
        public string Account { set; get; }

        public string UserName { set; get; }

        public string Phonenumber { set; get; }

        public string Status { set; get; }

        public long DeptId { set; get; }

        public DateTime? BeginTime { set; get; }

        public DateTime? EndTime { set; get; }
    }
}
