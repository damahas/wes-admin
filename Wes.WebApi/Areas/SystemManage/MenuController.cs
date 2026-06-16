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
    [Route("system/menu")]
    public class MenuController : ControllerBase
    {
        private ISysMenuBiz _sysMenuBiz;

        public MenuController(ISysMenuBiz sysMenuBiz)
        {
            this._sysMenuBiz = sysMenuBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery]MenuParam param)
        {
            return _sysMenuBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysMenuBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] SysMenuModel dept)
        {
            return _sysMenuBiz.Save(dept);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] SysMenuModel dept)
        {
            return _sysMenuBiz.Save(dept);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysMenuBiz.Delete(ids);
        }

        [HttpGet]
        [Route("treeselect")]
        public ReturnData GetMenuTree()
        {
            return _sysMenuBiz.GetMenuTree();
        }

        [HttpGet]
        [Route("roleMenuTreeselect/{roleId}")]
        public ReturnData GetRoleMenu(long roleId)
        {
            return _sysMenuBiz.GetRoleMenu(roleId);
        }
    }
}