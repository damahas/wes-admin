using System.Collections.Generic;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysI18nBiz
    {
        public RowData<SysI18nEntity> GetList(ParamData<I18nParam> param);

        public ResultData<SysI18nEntity> GetById(long id);

        public ReturnData Save(SysI18nEntity model);

        public ReturnData Delete(string ids);

        public ReturnData RefreshCache();

        public ReturnData Seed();

        public ResultData<Dictionary<string, string>> GetFrontendTranslations(string lang);

        /// <summary>获取单条翻译文本</summary>
        public string GetTranslation(string key, string lang);
    }
}
