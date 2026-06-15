using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 三方集成骨架基类，封装 Provider 属性和子组件组合模式。
    /// 子类只需实现扫码登录的两个方法，部门/用户拉取委托给 *_Sync 组件。
    /// </summary>
    public abstract class BaseIntegration : IThirdPartyIntegration, IDisposable
    {
        public abstract string Provider { get; }

        #region 扫码登录（子类实现）

        public abstract Task<string> GetQrCodeLoginUrl(string state);
        public abstract Task<ThirdPartyUserInfo> GetUserInfoByAuthCode(string authCode);

        #endregion

        #region 部门 / 用户拉取（子类委托 *Sync 组件）

        public abstract Task<List<SyncDeptData>> GetDepartments();
        public abstract Task<SyncDeptData> GetDepartmentById(string thirdPartyDeptId);
        public abstract Task<List<SyncUserData>> GetUsers();
        public abstract Task<SyncUserData> GetUserById(string thirdPartyUserId);

        #endregion

        public abstract void Dispose();
    }
}
