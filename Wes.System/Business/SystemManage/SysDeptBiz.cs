using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysDeptBiz : ISysDeptBiz
    {
        private ISysDeptService _sysDeptService;
        private ISysRoleService _sysRoleService;

        public SysDeptBiz(ISysDeptService sysDeptService, ISysRoleService sysRoleService)
        {
            _sysDeptService = sysDeptService;
            _sysRoleService = sysRoleService;
        }

        #region 部门操作

        public ReturnData Delete(string ids)
        {
            var dicIds = ids.ToLongList();
            if (dicIds == null || dicIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysDeptService.Delete(dicIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public ResultData<SysDeptEntity> GetById(long id)
        {
            return new ResultData<SysDeptEntity>(_sysDeptService.GetById(id));
        }

        public ResultData<List<SysDeptEntity>> GetList(DeptParam param)
        {
            return new ResultData<List<SysDeptEntity>>(_sysDeptService.GetList(param));
        }

        public ResultData<List<SysDeptEntity>> GetAll()
        {
            return new ResultData<List<SysDeptEntity>>(_sysDeptService.GetAll());
        }

        public ReturnData Save(SysDeptEntity dept)
        {
            if (dept.ParentId > 0)
            {
                var parentDept = _sysDeptService.GetById(dept.ParentId);
                if (parentDept != null)
                {
                    dept.Ancestors = $"{parentDept.Ancestors},{parentDept.DeptId}".TrimStart(',');
                }
            }
            else
            {
                dept.Ancestors = "0";
            }
            if (_sysDeptService.Save(dept))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        #endregion


        public ResultData<List<SysDeptEntity>> GetExcludeById(long id)
        {
            return new ResultData<List<SysDeptEntity>>(_sysDeptService.GetExcludeById(id));
        }

        public ResultData<RoleTreeInfo> GetRoleDept(long roleId)
        {
            ResultData<RoleTreeInfo> result = new ResultData<RoleTreeInfo>(new RoleTreeInfo()
            {
                RoleTrees = new List<RoleTreeDetailInfo>(),
                CheckedKeys = _sysRoleService.GetLeafRoleDeptIds(roleId)
            });
            List<RoleTreeDetailInfo> deptList = _sysDeptService.GetList(new DeptParam()).Select(p =>
            {
                return new RoleTreeDetailInfo()
                {
                    Id = p.DeptId,
                    ParentId = p.ParentId,
                    Label = p.DeptName
                };
            }).ToList();
            Dictionary<long, RoleTreeDetailInfo> deptDic = deptList.ToDictionary(p => p.Id, p => p);
            foreach (var dept in deptList)
            {
                if (dept.ParentId == 0)
                {
                    result.Data.RoleTrees.Add(dept);
                }
                else
                {
                    if (deptDic.ContainsKey(dept.ParentId))
                    {
                        if (deptDic[dept.ParentId].children == null)
                        {
                            deptDic[dept.ParentId].children = new List<RoleTreeDetailInfo>();
                        }
                        deptDic[dept.ParentId].children.Add(dept);
                    }
                }
            }
            return result;
        }
    }
}
