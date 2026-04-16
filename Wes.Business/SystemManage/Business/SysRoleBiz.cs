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

namespace Wes.Business
{
    public class SysRoleBiz : ISysRoleBiz
    {
        private ISysRoleService _sysRoleService;
        private ISysDicDataService _sysDicDataService;

        public SysRoleBiz(ISysRoleService sysRoleService, ISysDicDataService sysDicDataService)
        {
            _sysRoleService = sysRoleService;
            _sysDicDataService = sysDicDataService;
        }

        #region 权限操作

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysRoleService.Delete(delIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public ResultData<SysRoleModel> GetById(long id)
        {
            return new ResultData<SysRoleModel>(_sysRoleService.GetById(id));
        }

        public RowData<SysRoleModel> GetList(ParamData<RoleParam> param)
        {
            int total = 0;
            RowData<SysRoleModel> result = new RowData<SysRoleModel>(_sysRoleService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Save(RoleModel role)
        {
            var exist = _sysRoleService.GetByRoleKey(role.RoleKey);
            if (exist != null && exist.RoleId != role.RoleId)
            {
                return new ReturnData(500, "已存在该权限字符，请勿重复添加！");
            }
            _sysRoleService.Save(role);
            // TODO 处理角色所属菜单
            var menus = _sysRoleService.GetRoleMenuIds(role.RoleId);
            var newDepts = role.MenuIds.Except(menus);
            if (newDepts.Any())
            {
                _sysRoleService.SaveRoleMenu(newDepts.Select(p =>
                {
                    return new SysRoleMenuModel()
                    {
                        MenuId = p,
                        RoleId = role.RoleId
                    };
                }).ToList());
            }
            _sysRoleService.DeleteRoleMenu(role.RoleId, role.MenuIds);
            return new ReturnData();
        }

        public byte[] Export(ParamData<RoleParam> param)
        {
            int total = 0;
            param.PageSize = 0;
            return NPOIHepler.ExportExcel(_sysRoleService.GetList(param, out total), _sysDicDataService);
        }

        public ReturnData ChangeStatus(long roleId, string status)
        {
            var role = _sysRoleService.GetById(roleId);
            role.Status = status;
            if (_sysRoleService.Save(role))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "状态保存失败！");
        }

        #endregion

        #region 权限关联人员

        public ReturnData SaveRoleUser(long roleId, string userIds)
        {
            var saveIds = userIds.ToLongList();
            if (roleId == 0 || saveIds == null || saveIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysRoleService.SaveRoleUser(roleId, saveIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public ReturnData DeleteRoleUser(long roleId, string userIds)
        {
            var saveIds = userIds.ToLongList();
            if (roleId == 0 || saveIds == null || saveIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysRoleService.DeleteRoleUser(roleId, saveIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        #endregion

        #region 权限关联数据权限

        public ReturnData SaveRoleDept(RoleDeptModel roleDeptModel)
        {
            var role = _sysRoleService.GetById(roleDeptModel.RoleId);
            if (role == null) return new ReturnData(500, "找不到角色");
            // 更新角色
            role.DataScope = roleDeptModel.DataScope;
            role.DeptCheckStrictly = roleDeptModel.DeptCheckStrictly;
            _sysRoleService.Save(role);
            // 更新角色数据权限部门
            var depts = _sysRoleService.GetRoleDeptIds(roleDeptModel.RoleId);
            var newDepts = roleDeptModel.DeptIds.Except(depts);
            if (newDepts.Any())
            {
                _sysRoleService.SaveRoleDept(newDepts.Select(p =>
                {
                    return new SysRoleDeptModel()
                    {
                        DeptId = p,
                        RoleId = roleDeptModel.RoleId
                    };
                }).ToList());
            }
            _sysRoleService.DeleteRoleDept(roleDeptModel.RoleId, roleDeptModel.DeptIds);
            return new ReturnData();
        }

        #endregion
    }
}

