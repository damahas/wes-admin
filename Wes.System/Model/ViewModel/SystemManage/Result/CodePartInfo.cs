using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    public class CodePartInfo
    {
        public int StartIndex { set; get; }
        public int EndIndex { set; get; }
        public int CurrentIndex { set; get; }

        public string StartChar { set; get; }
        public string EndChar { set; get; }
        public string CurrentChar { set; get; }
    }
}
