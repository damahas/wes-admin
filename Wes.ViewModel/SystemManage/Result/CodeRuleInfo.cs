using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;

namespace Wes.ViewModel.SystemManage
{
    public class CodeRuleInfo : SysCodeRuleModel
    {
        public List<SysCodeRulePartModel> Parts { set; get; }
    }
}
