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
    [Route("system/role")]
    public class RoleController : ControllerBase
    {
        private ISysRoleBiz _sysRoleBiz;
        private ISysUserBiz _sysUserBiz;

        public RoleController(ISysRoleBiz sysRoleBiz, ISysUserBiz sysUserBiz)
        {
            this._sysRoleBiz = sysRoleBiz;
            this._sysUserBiz = sysUserBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<RoleParam> param)
        {
            return _sysRoleBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetDicById(long id)
        {
            return _sysRoleBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] RoleModel role)
        {
            return _sysRoleBiz.Save(role);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] RoleModel role)
        {
            return _sysRoleBiz.Save(role);
        }


        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysRoleBiz.Delete(ids);
        }

        [HttpPost]
        [Route("export")]
        public FileContentResult Export([FromForm] ParamData<RoleParam> param)
        {
            return File(_sysRoleBiz.Export(param), "application/ms-excel", $"角色导出{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }

        [HttpPut]
        [Route("changeStatus")]
        public ReturnData ChangeStatus(long roleId, string status)
        {
            return _sysRoleBiz.ChangeStatus(roleId, status);
        }

        [HttpGet]
        [Route("authUser/allocatedList")]
        public ReturnData GetRoleUserList([FromQuery] ParamData<RoleUserParam> param)
        {
            return this._sysUserBiz.GetRoleUserList(param);
        }

        [HttpGet]
        [Route("authUser/unallocatedList")]
        public ReturnData GetRoleNoExistUserList([FromQuery] ParamData<RoleUserParam> param)
        {
            return this._sysUserBiz.GetRoleNoExistUserList(param);
        }

        [HttpPut]
        [Route("authUser/selectAll")]
        public ReturnData SaveRoleUser([FromBody] RoleUserSaveParam roleUserSaveParam)
        {
            return _sysRoleBiz.SaveRoleUser(roleUserSaveParam.RoleId, roleUserSaveParam.userIds);
        }

        [HttpPut]
        [Route("authUser/cancelAll")]
        public ReturnData DeleteRoleUsers([FromBody] RoleUserSaveParam roleUserSaveParam)
        {
            return _sysRoleBiz.DeleteRoleUser(roleUserSaveParam.RoleId, roleUserSaveParam.userIds);
        }

        [HttpPut]
        [Route("authUser/cancel")]
        public ReturnData DeleteRoleUser([FromBody] RoleUserSaveParam roleUserSaveParam)
        {
            return _sysRoleBiz.DeleteRoleUser(roleUserSaveParam.RoleId, roleUserSaveParam.userId.ToString());
        }

        [HttpPut]
        [Route("dataScope")]
        public ReturnData SaveRoleDept([FromBody] RoleDeptModel roleDeptModel)
        {
            return _sysRoleBiz.SaveRoleDept(roleDeptModel);
        }
    }
}

