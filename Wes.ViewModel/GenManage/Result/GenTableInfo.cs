using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;

namespace Wes.ViewModel.GenManage
{
    public class GenTableInfo
    {
        public GenTableModel Info { set; get; }

        public List<GenTableColumnModel> Rows { set; get; }

        public List<GenTableModel> Tables { set; get; }
    }
}
