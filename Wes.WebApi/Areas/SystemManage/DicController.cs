using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Utils.Model;
using Wes.DbModel;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/dict")]
    public class DicController : ControllerBase
    {
        private ISysDicDataBiz _sysDicDataBiz;
        private ISysDicTypeBiz _sysDicTypeBiz;

        public DicController(ISysDicDataBiz sysDicDataBiz, ISysDicTypeBiz sysDicTypeBiz)
        {
            this._sysDicDataBiz = sysDicDataBiz;
            this._sysDicTypeBiz = sysDicTypeBiz;
        }

        #region type

        [HttpGet]
        [Route("type/list")]
        public ReturnData GetDic([FromQuery] ParamData<DicTypeParam> param)
        {
            return _sysDicTypeBiz.GetList(param);
        }

        [HttpGet]
        [Route("type/{id}")]
        public ReturnData GetDicById(long id)
        {
            return _sysDicTypeBiz.GetById(id);
        }

        [HttpPut]
        [Route("type")]
        public ReturnData UpdateDic([FromBody] SysDicTypeModel dic)
        {
            return _sysDicTypeBiz.Save(dic);
        }

        [HttpPost]
        [Route("type")]
        public ReturnData InsertDic([FromBody] SysDicTypeModel dic)
        {
            return _sysDicTypeBiz.Save(dic);
        }

        [HttpDelete]
        [Route("type/{ids}")]
        public ReturnData DeleteDic(string ids)
        {
            return _sysDicTypeBiz.Delete(ids);
        }

        [HttpPost]
        [Route("type/export")]
        public FileContentResult ExportDic([FromForm] ParamData<DicTypeParam> param)
        {
            return File(_sysDicTypeBiz.Export(param), "application/ms-excel");
        }

        [HttpDelete]
        [Route("type/refreshCache")]
        public ReturnData RefreshDicCache()
        {
            return _sysDicTypeBiz.Refresh();
        }

        [HttpGet]
        [Route("type/optionselect")]
        public ReturnData GetAllDic()
        {
            return _sysDicTypeBiz.GetAll();
        }

        #endregion

        #region data

        [HttpGet]
        [Route("data/list")]
        public ReturnData GetDicData([FromQuery] ParamData<DicDataParam> param)
        {
            return _sysDicDataBiz.GetList(param);
        }

        [HttpGet]
        [Route("data/{id}")]
        public ReturnData GetDicDataById(long id)
        {
            return _sysDicDataBiz.GetById(id);
        }

        [HttpPut]
        [Route("data")]
        public ReturnData UpdateDicData([FromBody] SysDicDataModel dic)
        {
            return _sysDicDataBiz.Save(dic);
        }

        [HttpPost]
        [Route("data")]
        public ReturnData InsertDicData([FromBody] SysDicDataModel dic)
        {
            return _sysDicDataBiz.Save(dic);
        }

        [HttpDelete]
        [Route("data/{ids}")]
        public ReturnData DeleteDicData(string ids)
        {
            return _sysDicDataBiz.Delete(ids);
        }

        [HttpGet]
        [Route("data/type/{dicType}")]
        public ReturnData GetDicDataListByDicType(string dicType)
        {
            return new ResultData<List<SysDicDataModel>>(_sysDicDataBiz.GetListByDicType(dicType));
        }

        [HttpPost]
        [Route("data/export")]
        public FileContentResult ExportDicData([FromForm] ParamData<DicDataParam> param)
        {
            return File(_sysDicDataBiz.Export(param), "application/ms-excel");
        }

        #endregion
    }
}

