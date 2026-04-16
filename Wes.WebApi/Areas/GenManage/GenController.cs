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
    [Route("tool/gen")]
    public class GenController : ControllerBase
    {
        private IGenTableBiz _genTableBiz;

        public GenController(IGenTableBiz genTableBiz)
        {
            _genTableBiz = genTableBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<DbTableParam> param)
        {
            return _genTableBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetDbTable(long id)
        {
            return _genTableBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] GenTableModel table)
        {
            return _genTableBiz.Save(table);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _genTableBiz.Delete(ids);
        }

        [HttpPost]
        [Route("importTable")]
        public ReturnData ImportTable(string tables)
        {
            return _genTableBiz.ImportTable(tables);
        }

        [HttpGet]
        [Route("db/list")]
        public ReturnData GetDbTableList([FromQuery] ParamData<DbTableParam> param)
        {
            return _genTableBiz.GetDbTableList(param);
        }

        [HttpGet]
        [Route("synchDb/{tableName}")]
        public ReturnData RefreshDbTable(string tableName)
        {
            return _genTableBiz.RefreshDbTable(tableName);
        }

        [HttpGet]
        [Route("preview/{id}")]
        public ReturnData Preview(long id)
        {
            return _genTableBiz.Preview(id);
        }

        [HttpGet]
        [Route("genCode/{tableName}")]
        public ReturnData GenDbTable(string tableName)
        {
            return _genTableBiz.GenDbTable(tableName);
        }
    }
}