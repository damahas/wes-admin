using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.ViewModel.SystemManage;

namespace Wes.Service
{
    public interface ISysMenuService
    {
        public SysMenuEntity GetById(long id);

        public List<SysMenuEntity> GetList(MenuParam param);

        public bool Delete(List<long> ids);

        public bool Save(SysMenuEntity menu);

        public List<SysMenuEntity> GetByUserId(long userId, string menuType);

        public List<SysMenuEntity> GetByMenuType(string menuType);

        public List<string> GetPermissionsByUserId(long userId);
    }
}

