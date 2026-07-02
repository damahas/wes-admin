using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.Utils.Integration;
using Wes.ViewModel.SystemManage;
using Wes.Utils.Cache;
using Wes.Utils;

namespace Wes.Business
{
    public class SysConfigBiz : ISysConfigBiz
    {
        private ISysConfigService _sysConfigService;
        private ISysDicDataService _sysDicDataService;
        private ISyncDataSaveService _syncDataSaveService;

        public SysConfigBiz(ISysConfigService sysConfigService, ISysDicDataService sysDicDataService, ISyncDataSaveService syncDataSaveService)
        {
            _sysConfigService = sysConfigService;
            _sysDicDataService = sysDicDataService;
            _syncDataSaveService = syncDataSaveService;
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

        public RowData<SysConfigModel> GetAll()
        {
            return new RowData<SysConfigModel>(_sysConfigService.GetAll());
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

        public ReturnData SaveSort(List<long> configIds)
        {
            _sysConfigService.UpdateSort(configIds);
            return new ReturnData();
        }

        #region 调用三方接口

        /// <summary>
        /// 同步指定三方平台的组织架构（部门 + 用户）
        /// </summary>
        /// <param name="provider">平台标识：dingtalk / feishu / wecom</param>
        public async Task<ReturnData> SyncThirdPartyAsync(string provider)
        {
            var configKey = $"sys.integration.{provider.ToLower()}";
            var configJson = GetByConfigKey(configKey);
            if (string.IsNullOrEmpty(configJson))
                throw new Exception($"未找到平台配置，Key: {configKey}，请先在参数配置中新增 {configKey}");

            var integration = CreateIntegration(provider, configJson);

            var deptResult = await _syncDataSaveService.SaveDeptsAsync(integration.Provider, await integration.GetDepartments());
            var userResult = await _syncDataSaveService.SaveUsersAsync(integration.Provider, await integration.GetUsers());

            var success = deptResult.Success && userResult.Success;
            var msg = $"创建{deptResult.CreatedCount + userResult.CreatedCount}条，更新{deptResult.UpdatedCount + userResult.UpdatedCount}条，失败{deptResult.FailedCount + userResult.FailedCount}条";
            return success ? new ReturnData() : new ReturnData(500, msg);
        }

        /// <summary>
        /// 从配置 JSON 创建三方集成实例
        /// </summary>
        private IThirdPartyIntegration CreateIntegration(string provider, string configJson)
        {
            var jObj = JObject.Parse(configJson);
            var factory = new IntegrationFactory();

            switch (provider.ToLower())
            {
                case "dingtalk":
                    var dingConfig = new DingTalkConfig
                    {
                        BaseUrl = jObj["dingPath"]?.Value<string>() ?? "https://oapi.dingtalk.com",
                        AppId = jObj["appId"]?.Value<string>(),
                        ClientId = jObj["clientId"]?.Value<string>(),
                        ClientSecret = jObj["clientSecret"]?.Value<string>(),
                        CorpId = jObj["corpId"]?.Value<string>(),
                        Enabled = true,
                    };
                    return factory.Create(dingConfig);

                case "feishu":
                    var feishuConfig = new FeishuConfig
                    {
                        BaseUrl = jObj["baseUrl"]?.Value<string>() ?? "https://open.feishu.cn",
                        AppId = jObj["appId"]?.Value<string>(),
                        AppSecret = jObj["appSecret"]?.Value<string>(),
                        Enabled = true,
                    };
                    return factory.Create(feishuConfig);

                case "wecom":
                    var wecomConfig = new WeComConfig
                    {
                        BaseUrl = jObj["baseUrl"]?.Value<string>() ?? "https://qyapi.weixin.qq.com",
                        CorpId = jObj["corpId"]?.Value<string>(),
                        CorpSecret = jObj["corpSecret"]?.Value<string>(),
                        AgentId = jObj["agentId"]?.Value<long?>(),
                        Enabled = true,
                    };
                    return factory.Create(wecomConfig);

                default:
                    throw new NotSupportedException($"不支持的三方平台: {provider}");
            }
        }

        #endregion
    }
}
