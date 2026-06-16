using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    public class ConfigParam
    {
        public string ConfigName { set; get; }

        public string ConfigKey { set; get; }

        public string ConfigType { set; get; }

        public DateTime? BeginTime { set; get; }

        public DateTime? EndTime { set; get; }
    }
}
