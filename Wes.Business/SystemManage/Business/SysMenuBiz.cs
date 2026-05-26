using System;
using System.Collections.Generic;
using System.Text;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Security;
using Wes.Utils.Extension;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.Utils.Cache;
using Wes.Utils;
using System.Linq;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysMenuBiz : ISysMenuBiz
    {
        private ISysMenuService _sysMenuService;
        private ISysRoleService _sysRoleService;

        public SysMenuBiz(ISysMenuService sysMenuService, ISysRoleService sysRoleService)
        {
            _sysMenuService = sysMenuService;
            _sysRoleService = sysRoleService;
        }

        #region 菜单操作

        public ResultData<SysMenuModel> GetById(long id)
        {
            return new ResultData<SysMenuModel>(_sysMenuService.GetById(id));
        }

        public ResultData<List<SysMenuModel>> GetList(MenuParam param)
        {
            return new ResultData<List<SysMenuModel>>(_sysMenuService.GetList(param));
        }

        public ReturnData Save(SysMenuModel menu)
        {
            if (_sysMenuService.Save(menu))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public ReturnData Delete(string ids)
        {
            var dicIds = ids.ToLongList();
            if (dicIds == null || dicIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysMenuService.Delete(dicIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        #endregion

        public List<MenuRootInfo> GetUserMenu()
        {
            List<MenuRootInfo> result = new List<MenuRootInfo>();
            // 处理目录
            List<SysMenuModel> queryMenus;
            if (GlobalContext.CurrentUser.IsAdmin)
            {
                queryMenus = _sysMenuService.GetByMenuType("M");
            }
            else
            {
                queryMenus = _sysMenuService.GetByUserId(GlobalContext.CurrentUser.UserId, "M");
            }
            List<MenuRootInfo> menus = queryMenus.Select(p =>
                 {
                     return new MenuRootInfo()
                     {
                         MenuId = p.MenuId,
                         ParentId = p.ParentId,
                         Component = GetComponent(p),
                         Hidden = p.Visible == "1",
                         Name = GetMenuName(p),
                         Path = $"{(p.ParentId == 0 ? "/" : "")}{p.Path}",
                         OrderNum = p.OrderNum ?? 0,
                         Redirect = p.IsFrame == 1 ? "noRedirect" : null,
                         Meta = new MenuMetaInfo()
                         {
                             Title = p.MenuName,
                             NoCache = p.IsCache == 1,
                             Icon = p.Icon,
                             Link = p.IsFrame == 1 ? p.Path : null
                         }
                     };

                 }).ToList();
            Dictionary<long, MenuRootInfo> menuDics = menus.ToDictionary(p => p.MenuId, p => p);
            foreach (var menu in menus)
            {
                if (menu.ParentId == 0)
                {
                    result.Add(menu);
                    continue;
                }
                if (menuDics.ContainsKey(menu.ParentId))
                {
                    if (menuDics[menu.ParentId].Children == null)
                    {
                        menuDics[menu.ParentId].Children = new List<MenuRootInfo>();
                    }
                    menuDics[menu.ParentId].Children.Add(menu);
                }
            }
            // 处理页面
            if (GlobalContext.CurrentUser.IsAdmin)
            {
                queryMenus = _sysMenuService.GetByMenuType("C");
            }
            else
            {
                queryMenus = _sysMenuService.GetByUserId(GlobalContext.CurrentUser.UserId, "C");
            }
            List<MenuRootInfo> pages = queryMenus.Select(p =>
                {
                    return new MenuRootInfo()
                    {
                        MenuId = p.MenuId,
                        ParentId = p.ParentId,
                        Component = GetComponent(p),
                        Hidden = p.Visible == "1",
                        Name = GetMenuName(p),
                        Path = p.Path,
                        OrderNum = p.OrderNum ?? 0,
                        Meta = new MenuMetaInfo()
                        {
                            Title = p.MenuName,
                            NoCache = p.IsCache == 1,
                            Icon = p.Icon,
                            Link = p.IsFrame == 1 ? p.Path : null
                        }
                    };
                }).ToList();
            foreach (var page in pages)
            {
                if (menuDics.ContainsKey(page.ParentId))
                {
                    if (menuDics[page.ParentId].Children == null)
                    {
                        menuDics[page.ParentId].Children = new List<MenuRootInfo>();
                    }
                    menuDics[page.ParentId].Children.Add(page);
                    //TODO 优化排序，这个排序太辣鸡了
                    menuDics[page.ParentId].Children = menuDics[page.ParentId].Children.OrderBy(p => p.OrderNum).ToList();
                }
            }
            return result.OrderBy(p => p.OrderNum).ToList();
        }

        public ResultData<RoleTreeInfo> GetRoleMenu(long roleId)
        {
            ResultData<RoleTreeInfo> result = new ResultData<RoleTreeInfo>(new RoleTreeInfo()
            {
                RoleTrees = new List<RoleTreeDetailInfo>(),
                CheckedKeys = _sysRoleService.GetLeafRoleMenuIds(roleId)
            });
            List<RoleTreeDetailInfo> menuList = _sysMenuService.GetList(new MenuParam()).Select(p =>
             {
                 return new RoleTreeDetailInfo()
                 {
                     Id = p.MenuId,
                     ParentId = p.ParentId,
                     Label = p.MenuName,
                     children = new List<RoleTreeDetailInfo>()
                 };
             }).ToList();
            Dictionary<long, RoleTreeDetailInfo> deptDic = menuList.ToDictionary(p => p.Id, p => p);
            foreach (var menu in menuList)
            {
                if (menu.ParentId == 0)
                {
                    result.Data.RoleTrees.Add(menu);
                }
                else
                {
                    if (deptDic.ContainsKey(menu.ParentId))
                    {
                        deptDic[menu.ParentId].children.Add(menu);
                    }
                }
            }
            return result;
        }

        private string GetComponent(SysMenuModel menu)
        {
            if (!string.IsNullOrWhiteSpace(menu.Component))
                return menu.Component;
            if (menu.ParentId > 0)
                return "ParentView";
            if (menu?.IsFrame == 1 && menu.Path.StartsWith("http"))
                return "InnerLink";
            return "Layout";
        }

        private string GetMenuName(SysMenuModel menu) {
            if (!string.IsNullOrWhiteSpace(menu.Component)) {
                return menu.Component.Replace("/", "_");
            }
            return menu.Path.ToFirstCharUpper();
        }

        public ResultData<List<RoleTreeDetailInfo>> GetMenuTree()
        {
            var result = new List<RoleTreeDetailInfo>();
            List<RoleTreeDetailInfo> menuList = _sysMenuService.GetList(new MenuParam()).Select(p =>
            {
                return new RoleTreeDetailInfo()
                {
                    Id = p.MenuId,
                    ParentId = p.ParentId,
                    Label = p.MenuName,
                    children = new List<RoleTreeDetailInfo>()
                };
            }).ToList();
            Dictionary<long, RoleTreeDetailInfo> deptDic = menuList.ToDictionary(p => p.Id, p => p);
            foreach (var menu in menuList)
            {
                if (menu.ParentId == 0)
                {
                    result.Add(menu);
                }
                else
                {
                    if (deptDic.ContainsKey(menu.ParentId))
                    {
                        deptDic[menu.ParentId].children.Add(menu);
                    }
                }
            }
            return new ResultData<List<RoleTreeDetailInfo>>(result);
        }
    }
}

