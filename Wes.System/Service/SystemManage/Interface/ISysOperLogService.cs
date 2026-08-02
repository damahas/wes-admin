using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
	public interface ISysOperLogService
	{
		public List<SysOperLogEntity> GetList(ParamData<OperLogParam> param, out int total);

		public bool Save(SysOperLogEntity model);
	}
}

