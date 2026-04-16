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
    public class SysDicDataBiz : ISysDicDataBiz
    {
        private ISysDicDataService _sysDicDataService;

        public SysDicDataBiz(ISysDicDataService sysDicDataService)
        {
            _sysDicDataService = sysDicDataService;
        }

        public RowData<SysDicDataModel> GetList(ParamData<DicDataParam> param)
        {
            int total = 0;
            RowData<SysDicDataModel> result = new RowData<SysDicDataModel>(_sysDicDataService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ResultData<SysDicDataModel> GetById(long id)
        {
            return new ResultData<SysDicDataModel>(_sysDicDataService.GetById(id));
        }

        public ReturnData Save(SysDicDataModel dic)
        {
            var existDict = _sysDicDataService.GetByDictType(dic.DictType, dic.DictValue);
            if (existDict != null && existDict.DictDataId != dic.DictDataId)
            {   
                return new ReturnData(500, "已存在该字典值，请勿重复添加！");
            }
            if (_sysDicDataService.Save(dic))
            {
                Refresh(dic.DictType);
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
            var dicDatas = _sysDicDataService.GetByIds(dicIds);
            if (_sysDicDataService.Delete(dicIds))
            {
                if (dicDatas.Any())
                {
                    Refresh(dicDatas.First().DictType);
                }
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public List<SysDicDataModel> GetListByDicType(string dicType)
        {
            var dics = CacheFactory.Cache.GetCache<List<SysDicDataModel>>($"{CacheKey.Dic}{dicType}");
            if (dics != null)
            {
                return dics;
            }
            return Refresh(dicType);
        }

        public byte[] Export(ParamData<DicDataParam> param)
        {
            int total = 0;
            param.PageSize = 0;
            return NPOIHepler.ExportExcel(_sysDicDataService.GetList(param, out total), _sysDicDataService);
        }

        private List<SysDicDataModel> Refresh(string dicType)
        {
            var dics = _sysDicDataService.GetListByDicType(dicType);
            CacheFactory.Cache.SetCache($"{CacheKey.Dic}{dicType}", dics, DateTime.Now.AddHours(8));
            return dics;
        }
    }
}

