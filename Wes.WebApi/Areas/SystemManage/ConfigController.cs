using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wes.Business;
using Wes.DbModel;
using Wes.Utils.Hepler;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/config")]
    public class ConfigController : ControllerBase
    {
        private ISysConfigBiz _sysConfigBiz;

        public ConfigController(ISysConfigBiz sysConfigBiz)
        {
            _sysConfigBiz = sysConfigBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<ConfigParam> param)
        {
            return _sysConfigBiz.GetList(param);
        }

        [HttpGet]
        [Route("all")]
        public ReturnData GetAll()
        {
            return new RowData<SysConfigModel>(_sysConfigBiz.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysConfigBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] SysConfigModel config)
        {
            return _sysConfigBiz.Save(config);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] SysConfigModel config)
        {
            return _sysConfigBiz.Save(config);
        }

        [HttpPost]
        [Route("sort")]
        public ReturnData SaveSort([FromBody] List<long> configIds)
        {
            return _sysConfigBiz.SaveSort(configIds);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysConfigBiz.Delete(ids);
        }

        [HttpPost]
        [Route("export")]
        public FileContentResult Export([FromForm] ParamData<ConfigParam> param)
        {
            return File(_sysConfigBiz.Export(param), "application/ms-excel");
        }

        [HttpGet]
        [Route("configKey/{configKey}")]
        public ReturnData GetByConfigKey(string configKey)
        {
            return new ReturnData(200, _sysConfigBiz.GetByConfigKey(configKey));
        }

        [HttpDelete]
        [Route("refreshCache")]
        public ReturnData RefreshCache()
        {
            return _sysConfigBiz.Refresh();
        }

        [HttpPost]
        [Route("sync/{provider}")]
        public async Task<ReturnData> SyncThirdParty(string provider)
        {
            return await _sysConfigBiz.SyncThirdPartyAsync(provider);
        }

        [HttpPost]
        [Route("test/mail")]
        public ReturnData TestMail([FromBody] MailModel mailModel)
        {
            if (string.IsNullOrWhiteSpace(mailModel.MailHost))
                return new ReturnData(400, "邮箱服务器不能为空");
            if (string.IsNullOrWhiteSpace(mailModel.MailPort))
                return new ReturnData(400, "邮箱端口不能为空");
            if (string.IsNullOrWhiteSpace(mailModel.MailAccount))
                return new ReturnData(400, "邮箱账号不能为空");
            if (string.IsNullOrWhiteSpace(mailModel.TestMail))
                return new ReturnData(400, "测试邮箱不能为空");

            var success = MailHelper.Send(mailModel, mailModel.TestMail, "测试邮件", "您收到一条测试邮件，请注意查收", null, out var errorMsg);
            if (success)
                return new ReturnData(200, "发送成功");
            else
                return new ReturnData(500, $"发送失败：{errorMsg}");
        }
    }
}