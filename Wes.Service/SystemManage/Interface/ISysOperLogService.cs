using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
	public interface ISysOperLogService
	{
		public List<SysOperLogModel> GetList(ParamData<OperLogParam> param, out int total);

		public bool Save(SysOperLogModel model);
	}
}

