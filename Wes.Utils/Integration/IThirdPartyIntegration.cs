using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wes.Utils.Integration
{
    /// <summary>
    /// 三方集成统一接口。
    /// 扫码登录在此处理；部门/用户数据仅拉取并返回统一格式，
    /// 持久化交由外部（Wes.Service 层）的 SyncDataSaveService 处理。
    /// 统一模型见 Models/SyncModels.cs 和 Models/ThirdPartyModels.cs。
    /// </summary>
    public interface IThirdPartyIntegration
    {
        /// <summary>集成类型标识（如 "DingTalk", "WeCom", "Feishu"）</summary>
        string Provider { get; }

        #region 扫码登录

        /// <summary>获取扫码登录地址</summary>
        Task<string> GetQrCodeLoginUrl(string state);

        /// <summary>通过 authCode 获取三方用户信息</summary>
        Task<ThirdPartyUserInfo> GetUserInfoByAuthCode(string authCode);

        #endregion

        #region 部门数据拉取

        /// <summary>从三方平台拉取全部部门</summary>
        Task<List<SyncDeptData>> GetDepartments();

        /// <summary>根据三方部门 ID 拉取单个部门</summary>
        Task<SyncDeptData> GetDepartmentById(string thirdPartyDeptId);

        #endregion

        #region 用户数据拉取

        /// <summary>从三方平台拉取全部用户</summary>
        Task<List<SyncUserData>> GetUsers();

        /// <summary>根据三方用户 ID 拉取单个用户</summary>
        Task<SyncUserData> GetUserById(string thirdPartyUserId);

        #endregion
    }
}
