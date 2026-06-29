using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.FlowManage
{
    public class FlowProcessParam
    {
        public string? ProcessCode { get; set; }

        public string? ProcessName { get; set; }

        public long ParentId { get; set; }

        public long CurVersionId { get; set; }

        public string? BusinessField { get; set; }

        public string? FormUrl { get; set; }

        public string? BackUrl { get; set; }
    }
}
