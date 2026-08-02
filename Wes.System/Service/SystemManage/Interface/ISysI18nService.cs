using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysI18nService
    {
        public List<SysI18nEntity> GetList(ParamData<I18nParam> param, out int total);

        public SysI18nEntity GetById(long id);

        public bool Save(SysI18nEntity model);

        public bool Delete(List<long> ids);

        public string GetTranslation(string key, string lang);

        public Dictionary<string, string> GetTranslationsByPrefix(string prefix, string lang);

        public Dictionary<string, string> GetAllFrontendTranslations(string lang);

        public SysI18nEntity GetByKeyAndLang(string i18nKey, string lang);
    }
}
