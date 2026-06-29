using Dm.util;
using Jint;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

        public ResultData<SysDataServiceModel> GetByCode(string serviceCode)
        {
            return new ResultData<SysDataServiceModel>(_sysDataServiceService.GetByCode(serviceCode));
        }

        public ReturnData Save(SysDataServiceModel model)
        {
            var oldModel = _sysDataServiceService.GetByCode(model.ServiceCode);
            if (oldModel != null && (model.DsId == 0 || oldModel.DsId != model.DsId))
            {
                return new ReturnData(500, "数据服务编码重复，需要保证唯一！");
            }
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

        public ResultData<Dictionary<string, object>> Exec(string serviceCode, Dictionary<string, object> param)
        {
            var serviceModel = _sysDataServiceService.GetByCode(serviceCode);
            if (serviceModel == null)
            {
                return new ResultData<Dictionary<string, object>>(404, "数据服务不存在，请检查参数！");
            }
            if (serviceModel.Status != "0")
            {
                return new ResultData<Dictionary<string, object>>(500, "数据服务未启用，无法执行！");
            }
            Dictionary<string, object> result = new Dictionary<string, object>();
            // 初始化js引擎，并将参数注入到js引擎中
            var jsEngine = new Engine();
            foreach (var item in param)
            {
                jsEngine.SetValue(item.Key, item.Value.ToRawObject());
            }
            foreach (var node in serviceModel?.Nodes)
            {
                switch (node.PartType)
                {
                    case DataServiceNodeTypeEnum.SQL:
                        if (string.IsNullOrWhiteSpace(node.PartConfig))
                        {
                            result.Add(node.VarName, null);
                            continue;
                        }
                        string sql = node.PartConfig;
                        Dictionary<string, object> sqlParam = new Dictionary<string, object>();
                        MatchCollection matches = Regex.Matches(sql, @"#{([^}]*?)}");
                        foreach (Match match in matches)
                        {
                            string key = $"@{match.Groups[1].Value}";
                            if (sqlParam.ContainsKey(key))
                            {
                                continue;
                            }
                            sql = sql.Replace(match.Value, key);
                            sqlParam.Add(key, jsEngine.Evaluate(match.Groups[1].Value).toString());
                        }
                        switch (node.VarType)
                        {
                            case DataServiceNodeReturnEnum.OBJECT:
                                result.Add(node.VarName, _sysDataServiceService.GetDataSingle(sql, sqlParam));
                                break;
                            case DataServiceNodeReturnEnum.FIRST_CELL:
                                result.Add(node.VarName, _sysDataServiceService.GetDataFirstCell(sql, sqlParam));
                                break;
                            case DataServiceNodeReturnEnum.COMMAND:
                                result.Add(node.VarName, _sysDataServiceService.ExecCommand(sql, sqlParam));
                                break;
                            default:
                                result.Add(node.VarName, _sysDataServiceService.GetDataList(sql, sqlParam));
                                break;
                        }
                        break;
                    case DataServiceNodeTypeEnum.JAVASCRIPT:
                        if (string.IsNullOrWhiteSpace(node.PartConfig))
                        {
                            continue;
                        }
                        result = jsEngine.Execute(node.PartConfig).Invoke("handle", result).ToObject() as Dictionary<string, object>;
                        break;
                    default:
                        break;
                }
            }
            return new ResultData<Dictionary<string, object>>(result);
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
