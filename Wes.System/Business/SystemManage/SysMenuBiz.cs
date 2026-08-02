using System;
using System.Collections.Generic;
using System.Text;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Security;
using Wes.Utils.Extension;
using Wes.Entity;
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
        private ISysI18nBiz _sysI18nBiz;

        public SysMenuBiz(ISysMenuService sysMenuService, ISysRoleService sysRoleService, ISysI18nBiz sysI18nBiz)
        {
            _sysMenuService = sysMenuService;
            _sysRoleService = sysRoleService;
            _sysI18nBiz = sysI18nBiz;
        }

        #region 菜单操作

        public ResultData<SysMenuEntity> GetById(long id)
        {
            return new ResultData<SysMenuEntity>(_sysMenuService.GetById(id));
        }

        public ResultData<List<SysMenuEntity>> GetList(MenuParam param)
        {
            return new ResultData<List<SysMenuEntity>>(_sysMenuService.GetList(param));
        }

        public ReturnData Save(SysMenuEntity menu)
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
            var isAdmin = GlobalContext.CurrentUser.IsAdmin;
            var userId = GlobalContext.CurrentUser.UserId;
            
            var dirMenus  = isAdmin ? _sysMenuService.GetByMenuType("M") : _sysMenuService.GetByUserId(userId, "M");
            var pageMenus = isAdmin ? _sysMenuService.GetByMenuType("C") : _sysMenuService.GetByUserId(userId, "C");

            var result   = new List<MenuRootInfo>();
            var menuMap  = new Dictionary<long, MenuRootInfo>();

            AttachToTree(dirMenus,  result, menuMap);
            AttachToTree(pageMenus, result, menuMap);

            // 子菜单排序
            foreach (var item in menuMap.Values)
            {
                if (item.Children?.Count > 0)
                    item.Children = item.Children.OrderBy(p => p.OrderNum).ToList();
            }

            return result.OrderBy(p => p.OrderNum).ToList();
        }

        /// <summary>将菜单列表挂到树结构中</summary>
        private void AttachToTree(List<SysMenuEntity> menus, List<MenuRootInfo> result, Dictionary<long, MenuRootInfo> menuMap)
        {
            foreach (var m in menus)
            {
                var info = BuildMenuRootInfo(m);
                menuMap[info.MenuId] = info;

                if (info.ParentId == 0)
                {
                    result.Add(info);
                }
                else if (menuMap.TryGetValue(info.ParentId, out var parent))
                {
                    parent.Children ??= new List<MenuRootInfo>();
                    parent.Children.Add(info);
                }
            }
        }

        /// <summary>构建单个菜单节点</summary>
        private MenuRootInfo BuildMenuRootInfo(SysMenuEntity menu)
        {
            return new MenuRootInfo
            {
                MenuId    = menu.MenuId,
                ParentId  = menu.ParentId,
                Component = GetComponent(menu),
                Hidden    = menu.Visible == "1",
                Name      = GetMenuName(menu),
                Path      = menu.ParentId == 0 ? $"/{menu.Path}" : menu.Path,
                OrderNum  = menu.OrderNum ?? 0,
                Redirect  = menu.IsFrame == 1 ? "noRedirect" : null,
                Meta = new MenuMetaInfo
                {
                    Title   = GetMenuTitle(menu),
                    NoCache = menu.IsCache == 1,
                    Icon    = menu.Icon,
                    Link    = menu.IsFrame == 1 ? menu.Path : null
                }
            };
        }

        /// <summary>获取菜单标题（支持国际化）</summary>
        private string GetMenuTitle(SysMenuEntity menu)
        {
            var lang = GlobalContext.Lang.Value;
            if (string.IsNullOrWhiteSpace(lang) || lang == "zh-CN")
                return menu.MenuName;

            var translated = _sysI18nBiz.GetTranslation($"sys.menu.{menu.MenuId}", lang);
            return !string.IsNullOrWhiteSpace(translated) ? translated : menu.MenuName;
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

        private string GetComponent(SysMenuEntity menu)
        {
            if (!string.IsNullOrWhiteSpace(menu.Component))
                return menu.Component;
            if (menu.ParentId > 0)
                return "ParentView";
            if (menu?.IsFrame == 1 && menu.Path.StartsWith("http"))
                return "InnerLink";
            return "Layout";
        }

        private string GetMenuName(SysMenuEntity menu) {
            if (!string.IsNullOrWhiteSpace(menu.RouteName))
            {
                return menu.RouteName;
            }
            if (!string.IsNullOrWhiteSpace(menu.Component))
            {
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

