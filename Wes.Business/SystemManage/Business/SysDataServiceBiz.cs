using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysDataServiceBiz : ISysDataServiceBiz
    {
        private ISysDataServiceService _sysDataServiceService;

        public SysDataServiceBiz(ISysDataServiceService sysDataServiceService)
        {
            _sysDataServiceService = sysDataServiceService;
        }

        public RowData<SysDataServiceModel> GetList(ParamData<DataServiceParam> param)
        {
            int total = 0;
            RowData<SysDataServiceModel> result = new RowData<SysDataServiceModel>(_sysDataServiceService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ResultData<SysDataServiceModel> GetById(long id)
        {
            return new ResultData<SysDataServiceModel>(_sysDataServiceService.GetById(id));
        }

        public ReturnData Save(SysDataServiceModel model)
        {
            if (_sysDataServiceService.Save(model))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public ReturnData Delete(string ids)
        {
            var dicIds = ids.ToLongList();
            if (dicIds == null || dicIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysDataServiceService.Delete(dicIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        #region 数据库表信息
        public ResultData<List<DbTableInfo>> GetTables()
        {
            return new ResultData<List<DbTableInfo>>(_sysDataServiceService.GetTables());
        }

        public ResultData<List<DbColumnInfo>> GetTableColumns(string tableName)
        {
            return new ResultData<List<DbColumnInfo>>(_sysDataServiceService.GetTableColumns(tableName));
        }
        #endregion
    }
}
