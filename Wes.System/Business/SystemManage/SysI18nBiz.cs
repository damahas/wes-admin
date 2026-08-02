using System;
using System.Collections.Generic;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Extension;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.Utils;
using System.Linq;
using Wes.Utils.Cache;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysI18nBiz : ISysI18nBiz
    {
        private ISysI18nService _sysI18nService;

        public SysI18nBiz(ISysI18nService sysI18nService)
        {
            _sysI18nService = sysI18nService;
        }

        public RowData<SysI18nEntity> GetList(ParamData<I18nParam> param)
        {
            int total = 0;
            RowData<SysI18nEntity> result = new RowData<SysI18nEntity>(_sysI18nService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ResultData<SysI18nEntity> GetById(long id)
        {
            return new ResultData<SysI18nEntity>(_sysI18nService.GetById(id));
        }

        public ReturnData Save(SysI18nEntity model)
        {
            // 校验 I18nKey + Lang 唯一性（编辑时排除自身）
            var exists = _sysI18nService.GetByKeyAndLang(model.I18nKey, model.Lang);
            if (exists != null && exists.I18nId != model.I18nId)
                return new ReturnData(500, "同一语言下翻译键名已存在！");

            if (_sysI18nService.Save(model))
            {
                // 清除对应语言的缓存
                if (!string.IsNullOrWhiteSpace(model.Lang))
                {
                    CacheFactory.Cache.RemoveCache($"{CacheKey.I18n}{model.Lang}");
                }
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public ReturnData Delete(string ids)
        {
            var idList = ids.ToLongList();
            if (idList == null || idList.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            // 获取被删除的记录以清除缓存
            var delItems = idList.Select(id => _sysI18nService.GetById(id)).Where(p => p != null).ToList();
            foreach (var item in delItems)
            {
                if (!string.IsNullOrWhiteSpace(item.Lang))
                {
                    CacheFactory.Cache.RemoveCache($"{CacheKey.I18n}{item.Lang}");
                }
            }
            if (_sysI18nService.Delete(idList))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public ReturnData RefreshCache()
        {
            foreach (var lang in new[] { "zh-CN", "en-US" })
            {
                CacheFactory.Cache.RemoveCache($"{CacheKey.I18n}{lang}");
            }
            return new ReturnData();
        }

        public ReturnData Seed()
        {
            return new ReturnData(200, "Seed 功能待实现");
        }

        public string GetTranslation(string key, string lang)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(lang))
                return null;
            return _sysI18nService.GetTranslation(key, lang);
        }

        public ResultData<Dictionary<string, string>> GetFrontendTranslations(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
            {
                lang = "zh-CN";
            }
            var cacheKey = $"{CacheKey.I18n}{lang}";
            var cached = CacheFactory.Cache.GetCache<Dictionary<string, string>>(cacheKey);
            if (cached != null)
            {
                return new ResultData<Dictionary<string, string>>(cached);
            }

            var translations = _sysI18nService.GetAllFrontendTranslations(lang);
            CacheFactory.Cache.SetCache(cacheKey, translations, DateTime.Now.AddHours(24));
            return new ResultData<Dictionary<string, string>>(translations);
        }
    }
}
