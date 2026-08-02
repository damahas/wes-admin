using System;
using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
	public interface ISysDicTypeBiz
	{
		public RowData<SysDicTypeEntity> GetList(ParamData<DicTypeParam> param);

		public ResultData<List<SysDicTypeEntity>> GetAll();

		public ResultData<SysDicTypeEntity> GetById(long id);

		public ReturnData Save(SysDicTypeEntity dic);

		public ReturnData Delete(string ids);

		public byte[] Export(ParamData<DicTypeParam> param);

		public ReturnData Refresh();
	}
}

