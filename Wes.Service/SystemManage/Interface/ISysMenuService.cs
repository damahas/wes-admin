using System;
using System.Collections.Generic;
using System.Text;
using Wes.DbModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysMenuService
    {
        public SysMenuModel GetById(long id);

        public List<SysMenuModel> GetList(MenuParam param);

        public bool Delete(List<long> ids);

        public bool Save(SysMenuModel menu);

        public List<SysMenuModel> GetByUserId(long userId, string menuType);

        public List<SysMenuModel> GetByMenuType(string menuType);

        public List<string> GetPermissionsByUserId(long userId);
    }
}

