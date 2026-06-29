using System;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
	public interface ISysDicTypeBiz
	{
		public RowData<SysDicTypeModel> GetList(ParamData<DicTypeParam> param);

		public ResultData<List<SysDicTypeModel>> GetAll();

		public ResultData<SysDicTypeModel> GetById(long id);

		public ReturnData Save(SysDicTypeModel dic);

		public ReturnData Delete(string ids);

		public byte[] Export(ParamData<DicTypeParam> param);

		public ReturnData Refresh();
	}
}

