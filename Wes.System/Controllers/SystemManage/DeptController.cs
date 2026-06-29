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
    [Route("system/dept")]
    public class DeptController : ControllerBase
    {
        private ISysDeptBiz _sysDeptBiz;
        public DeptController(ISysDeptBiz sysDeptBiz)
        {
            this._sysDeptBiz = sysDeptBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] DeptParam param)
        {
            return _sysDeptBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysDeptBiz.GetById(id);
        }

        [HttpGet]
        [Route("list/exclude/{id}")]
        public ReturnData GetExclude(long id)
        {
            return _sysDeptBiz.GetExcludeById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] SysDeptModel dept)
        {
            return _sysDeptBiz.Save(dept);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] SysDeptModel dept)
        {
            return _sysDeptBiz.Save(dept);
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysDeptBiz.Delete(ids);
        }

        [HttpGet]
        [Route("treeselect")]
        public ReturnData TreeSelect()
        {
            var result = _sysDeptBiz.GetRoleDept(0);
            return new ResultData<List<RoleTreeDetailInfo>>(result.Data.RoleTrees);
        }

        /// <summary>
        /// 获取角色绑定数据权限部门
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("roleDeptTree/{roleId}")]
        public ReturnData RoleDeptTreeselect(long roleId)
        {
            return _sysDeptBiz.GetRoleDept(roleId);
        }
    }

}