using System;
using System.Collections.Generic;
using System.Text;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Extension;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.Utils;
using System.Linq;
using Wes.ViewModel.SystemManage;
using Wes.Utils.Cache;

namespace Wes.Business
{
    public class SysDicTypeBiz : ISysDicTypeBiz
    {
        private ISysDicTypeService _sysDicTypeService;
        private ISysDicDataService _sysDicDataService;

        public SysDicTypeBiz(ISysDicTypeService sysDicTypeService, ISysDicDataService sysDicDataService)
        {
            _sysDicTypeService = sysDicTypeService;
            _sysDicDataService = sysDicDataService;
        }

        public RowData<SysDicTypeModel> GetList(ParamData<DicTypeParam> param)
        {
            int total = 0;
            RowData<SysDicTypeModel> result = new RowData<SysDicTypeModel>(_sysDicTypeService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ResultData<List<SysDicTypeModel>> GetAll()
        {
            return new ResultData<List<SysDicTypeModel>>(_sysDicTypeService.GetAll());
        }

        public ResultData<SysDicTypeModel> GetById(long id)
        {
            return new ResultData<SysDicTypeModel>(_sysDicTypeService.GetById(id));
        }

        public ReturnData Save(SysDicTypeModel dic)
        {
            var existDict = _sysDicTypeService.GetByDictType(dic.DictType);
            if (existDict != null && existDict.DictId != dic.DictId)
            {
                return new ReturnData(500, "已存在该字典类型，请勿重复添加！");
            }
            if (_sysDicTypeService.Save(dic))
            {
                // 刷新缓存
                if (existDict != null && existDict.DictType != dic.DictType)
                {
                    CacheFactory.Cache.RemoveCache($"{CacheKey.Dic}{existDict.DictType}");
                }
                if (dic.Status == "1")
                {
                    CacheFactory.Cache.RemoveCache($"{CacheKey.Dic}{dic.DictType}");
                }
                else
                {
                    CacheFactory.Cache.SetCache($"{CacheKey.Dic}{dic.DictType}", _sysDicDataService.GetListByDicType(dic.DictType), DateTime.Now.AddHours(8));
                }
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
            var delTypes = _sysDicTypeService.GetByIds(dicIds);
            foreach (var item in delTypes)
            {
                CacheFactory.Cache.RemoveCache($"{CacheKey.Dic}{item.DictType}");
            }
            if (_sysDicTypeService.Delete(dicIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public byte[] Export(ParamData<DicTypeParam> param)
        {
            int total = 0;
            param.PageSize = 0;
            return NPOIHepler.ExportExcel(_sysDicTypeService.GetList(param, out total), _sysDicDataService);
        }

        public ReturnData Refresh()
        {
            var dicTypes = _sysDicDataService.GetAll().GroupBy(p => p.DictType).ToDictionary(p => p.Key, p => p.ToList());
            foreach (var item in dicTypes)
            {
                CacheFactory.Cache.SetCache($"{CacheKey.Dic}{item.Key}", item.Value, DateTime.Now.AddHours(8));
            }
            return new ReturnData();
        }
    }
}

