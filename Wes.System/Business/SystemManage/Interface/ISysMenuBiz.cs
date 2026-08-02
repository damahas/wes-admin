using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
	public interface ISysMenuBiz
	{
		#region 菜单操作

		public ResultData<SysMenuEntity> GetById(long id);

		public ResultData<List<SysMenuEntity>> GetList(MenuParam param);

		public ReturnData Save(SysMenuEntity menu);

		public ReturnData Delete(string ids);

		#endregion

		public List<MenuRootInfo> GetUserMenu();

		public ResultData<RoleTreeInfo> GetRoleMenu(long roleId);

		public ResultData<List<RoleTreeDetailInfo>> GetMenuTree();
	}
}

