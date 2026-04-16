using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;
using Wes.Utils.Cache;
using Wes.Utils;

namespace Wes.Business
{
    public class SysConfigBiz : ISysConfigBiz
    {
        private ISysConfigService _sysConfigService;
        private ISysDicDataService _sysDicDataService;

        public SysConfigBiz(ISysConfigService sysConfigService, ISysDicDataService sysDicDataService)
        {
            _sysConfigService = sysConfigService;
            _sysDicDataService = sysDicDataService;
        }

        #region 配置操作

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            var delConfigs = _sysConfigService.GetByIds(delIds);
            foreach (var item in delConfigs)
            {
                CacheFactory.Cache.RemoveCache($"{CacheKey.Config}{item.ConfigKey}");
            }
            if (_sysConfigService.Delete(delIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public ResultData<SysConfigModel> GetById(long id)
        {
            return new ResultData<SysConfigModel>(_sysConfigService.GetById(id));
        }

        public RowData<SysConfigModel> GetList(ParamData<ConfigParam> param)
        {
            int total = 0;
            RowData<SysConfigModel> result = new RowData<SysConfigModel>(_sysConfigService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Save(SysConfigModel config)
        {
            var exist = _sysConfigService.GetByConfigKey(config.ConfigKey);
            if (exist != null && exist.ConfigId != config.ConfigId)
            {
                return new ReturnData(500, "已存在该参数键名，请勿重复添加！");
            }
            if (_sysConfigService.Save(config))
            {
                CacheFactory.Cache.SetCache($"{CacheKey.Config}{config.ConfigKey}", config.ConfigValue, DateTime.Now.AddHours(8));
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public byte[] Export(ParamData<ConfigParam> param)
        {
            int total = 0;
            param.PageSize = 0;
            return NPOIHepler.ExportExcel(_sysConfigService.GetList(param, out total), _sysDicDataService);
        }
        #endregion

        public string GetByConfigKey(string configKey)
        {
            string ConfigValue = CacheFactory.Cache.GetCache<string>($"{CacheKey.Config}{configKey}");
            if (ConfigValue != null)
                return ConfigValue;
            var config = _sysConfigService.GetByConfigKey(configKey);
            if (config == null)
            {
                return "";
            }
            CacheFactory.Cache.SetCache($"{CacheKey.Config}{configKey}", config.ConfigValue, DateTime.Now.AddHours(8));
            return config.ConfigValue;
        }

        public ReturnData Refresh()
        {
            var configs = _sysConfigService.GetAll();
            foreach (var item in configs)
            {
                CacheFactory.Cache.SetCache($"{CacheKey.Config}{item.ConfigKey}", item.ConfigValue, DateTime.Now.AddHours(8));
            }
            return new ReturnData();
        }
    }
}
