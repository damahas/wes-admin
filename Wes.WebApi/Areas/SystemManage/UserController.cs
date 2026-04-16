using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Authorize]
    [Route("system/user")]
    public class UserController : ControllerBase
    {
        private ISysUserBiz _sysUserBiz;

        public UserController(ISysUserBiz sysUserBiz)
        {
            this._sysUserBiz = sysUserBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<UserParam> param)
        {
            return _sysUserBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysUserBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] UserModel dept)
        {
            return _sysUserBiz.Save(dept);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] UserModel dept)
        {
            return _sysUserBiz.Save(dept);
        }

        [HttpPost]
        [Route("export")]
        public FileContentResult Export([FromForm] ParamData<UserParam> param)
        {
            return File(_sysUserBiz.Export(param), "application/ms-excel");
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysUserBiz.Delete(ids);
        }

        [HttpPut]
        [Route("changeStatus")]
        public ReturnData ChangeStatus([FromBody] UserStatusParam userStatusParam)
        {
            return _sysUserBiz.ChangeStatus(userStatusParam.userId, userStatusParam.status);
        }

        [HttpGet]
        [Route("authRole/{id}")]
        public ReturnData GetUserRole(long id)
        {
            return _sysUserBiz.GetUserRole(id);
        }

        [HttpPut]
        [Route("authRole")]
        public ReturnData SaveUserRole(long userId, string roleIds)
        {
            return _sysUserBiz.SaveUserRole(userId, roleIds);
        }

        [HttpPut]
        [Route("resetPwd")]
        public ReturnData ResetUserPwd([FromBody] UserPwdParam userPwdParam)
        {
            return _sysUserBiz.ResetUserPwd(userPwdParam);
        }

        [HttpPut]
        [Route("profile/updatePwd")]
        public ReturnData UpdateCurrentUserPwd(string oldPassword, string newPassword)
        {
            return _sysUserBiz.UpdateCurrentUserPwd(oldPassword, newPassword);
        }

        [HttpGet]
        [Route("profile")]
        public ReturnData GetCurrentUser()
        {
            return new ProfileInfo<UserInfo>()
            {
                Code = 200,
                Data = GlobalContext.CurrentUser
            };
        }

        [HttpPut]
        [Route("profile")]
        public ReturnData SaveCurrentUser([FromBody] UserInfo userInfo)
        {
            return _sysUserBiz.SaveCurrentUser(userInfo);
        }
    }
}