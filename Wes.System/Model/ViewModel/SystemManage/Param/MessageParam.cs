using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    public class MessageParam
    {
        public string? MessageType { set; get; }

        public string? MessageTitle { set; get; }

        public string? MessageBody { set; get; }

        public int IsRead { set; get; } = -1;
    }
}
