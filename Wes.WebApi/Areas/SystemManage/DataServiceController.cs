using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/dataService")]
    public class DataServiceController : ControllerBase
    {
        private ISysDataServiceBiz _sysDataServiceBiz;

        public DataServiceController(ISysDataServiceBiz sysDataServiceBiz)
        {
            _sysDataServiceBiz = sysDataServiceBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery]ParamData<DataServiceParam> param)
        {
            return _sysDataServiceBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysDataServiceBiz.GetById(id);
        }

        [HttpPost]
        public ReturnData Save([FromBody] SysDataServiceModel model)
        {
            return _sysDataServiceBiz.Save(model);
        }

        [HttpPut]
        public ReturnData Update([FromBody] SysDataServiceModel model)
        {
            return _sysDataServiceBiz.Save(model);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysDataServiceBiz.Delete(ids);
        }

        [HttpGet]
        [Route("table/list")]
        public ReturnData GetTableList()
        {
            return _sysDataServiceBiz.GetTables();
        }

        [HttpGet]
        [Route("table/{tableName}/column")]
        public ReturnData GetTableColumn(string tableName)
        {
            return _sysDataServiceBiz.GetTableColumns(tableName);
        }
    }
}
