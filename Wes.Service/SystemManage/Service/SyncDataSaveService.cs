using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Integration;

namespace Wes.Service
{
    /// <summary>
    /// 三方集成数据保存服务。
    /// 接收 Utils 层统一格式数据，负责建立本地实体与三方映射关系并持久化到数据库。
    /// 同时实现 ISyncDataAccess 供登录流程中查询三方映射使用。
    /// </summary>
    public class SyncDataSaveService : ISyncDataSaveService, ISyncDataAccess
    {
        private readonly ISqlSugarClient _db;

        public SyncDataSaveService(ISqlSugarClient db)
        {
            _db = db;
        }

        #region ISyncDataSaveService 实现 —— 批量保存部门/用户

        public async Task<SyncSaveResult> SaveDeptsAsync(string provider, List<SyncDeptData> depts)
        {
            var result = new SyncSaveResult { Success = true };
            if (depts == null || depts.Count == 0)
                return result;

            // 构建已保存的映射字典（用于父部门关联解析）
            var mappingDict = new Dictionary<string, long>();

            foreach (var dd in depts)
            {
                try
                {
                    var dept = await GetOrCreateDept(provider, dd);

                    dept.DeptName = dd.DeptName;
                    dept.OrderNum = dd.OrderNum;
                    dept.UpdateTime = DateTime.Now;

                    // 解析父部门
                    ResolveDeptParent(dept, dd.ThirdPartyParentId, mappingDict);

                    // 保存
                    var isNew = dept.DeptId == 0;
                    if (isNew)
                    {
                        dept.CreateTime = DateTime.Now;
                        dept.CreateBy = "system";
                        dept.IsDel = 0;
                        dept.Status = "0";
                        dept.DeptId = _db.Insertable(dept).ExecuteReturnSnowflakeId();
                        result.CreatedCount++;
                    }
                    else
                    {
                        _db.Updateable(dept).ExecuteCommand();
                        result.UpdatedCount++;
                    }

                    // 建立/更新三方映射
                    await SaveMapping(provider, "Dept", dd.ThirdPartyDeptId, dept.DeptId.ToString(), dd.DeptName);
                    mappingDict[dd.ThirdPartyDeptId] = dept.DeptId;
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"保存部门 [{dd.DeptName}] 失败: {ex.Message}");
                }
            }

            // 二次修正：用完整映射字典修复 Ancenstors 中的占位符
            await FixDeptAncestors(provider, depts, mappingDict);

            return result;
        }

        public async Task<SyncSaveResult> SaveUsersAsync(string provider, List<SyncUserData> users)
        {
            var result = new SyncSaveResult { Success = true };
            if (users == null || users.Count == 0)
                return result;

            foreach (var du in users)
            {
                try
                {
                    var user = await GetOrCreateUser(provider, du);

                    user.UserName = du.UserName;
                    user.Account = du.Account;
                    user.Email = du.Email;
                    user.Phonenumber = du.Mobile;
                    user.Avatar = du.Avatar;
                    user.UpdateTime = DateTime.Now;

                    // 关联部门（取第一个有效部门）
                    if (du.ThirdPartyDeptIds != null && du.ThirdPartyDeptIds.Count > 0)
                    {
                        var deptMapping = await _db.Queryable<SysThirdPartyMappingModel>()
                            .Where(m => m.Provider == provider && m.EntityType == "Dept" && m.ThirdPartyId == du.ThirdPartyDeptIds[0])
                            .FirstAsync();

                        if (deptMapping != null && long.TryParse(deptMapping.LocalId, out var deptId))
                            user.DeptId = deptId;
                    }

                    // 保存
                    var isNew = user.UserId == 0;
                    if (isNew)
                    {
                        user.CreateTime = DateTime.Now;
                        user.CreateBy = "system";
                        user.UserType = "00";
                        user.Status = "0";
                        user.IsDel = 0;
                        user.UserId = _db.Insertable(user).ExecuteReturnSnowflakeId();
                        result.CreatedCount++;
                    }
                    else
                    {
                        _db.Updateable(user).ExecuteCommand();
                        result.UpdatedCount++;
                    }

                    // 建立/更新三方映射
                    await SaveMapping(provider, "User", du.ThirdPartyUserId, user.UserId.ToString(), du.UserName);
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"保存用户 [{du.UserName}] 失败: {ex.Message}");
                }
            }

            return result;
        }

        #endregion

        #region ISyncDataAccess 实现 —— 供登录流程查询（返回统一模型）

        public async Task<SyncDeptData> GetDeptByThirdPartyIdAsync(string thirdPartyId)
        {
            var entity = await QueryDeptEntityByThirdPartyIdAsync(thirdPartyId);
            if (entity == null) return null;

            return new SyncDeptData
            {
                ThirdPartyDeptId = thirdPartyId,
                DeptName = entity.DeptName,
                OrderNum = entity.OrderNum ?? 0
            };
        }

        public async Task<SyncUserData> GetUserByThirdPartyIdAsync(string thirdPartyId)
        {
            var entity = await QueryUserEntityByThirdPartyIdAsync(thirdPartyId);
            if (entity == null) return null;

            return new SyncUserData
            {
                ThirdPartyUserId = thirdPartyId,
                UserName = entity.UserName,
                Account = entity.Account,
                Email = entity.Email,
                Mobile = entity.Phonenumber,
                Avatar = entity.Avatar
            };
        }

        public async Task<bool> SaveThirdPartyMappingAsync(string provider, string entityType, string thirdPartyId, string localId, string displayName)
        {
            return await SaveMapping(provider, entityType, thirdPartyId, localId, displayName);
        }

        public async Task<string> GetThirdPartyUserIdAsync(long localUserId)
        {
            var mapping = await _db.Queryable<SysThirdPartyMappingModel>()
                .Where(m => m.EntityType == "User" && m.LocalId == localUserId.ToString())
                .FirstAsync();

            return mapping?.ThirdPartyId;
        }

        #endregion

        #region 私有辅助方法

        /// <summary>
        /// 根据三方 ID 查询本地部门 DB 实体
        /// </summary>
        private async Task<SysDeptModel> QueryDeptEntityByThirdPartyIdAsync(string thirdPartyId)
        {
            var mapping = await _db.Queryable<SysThirdPartyMappingModel>()
                .Where(m => m.EntityType == "Dept" && m.ThirdPartyId == thirdPartyId)
                .FirstAsync();

            if (mapping != null && long.TryParse(mapping.LocalId, out var deptId))
                return await _db.Queryable<SysDeptModel>().Where(d => d.DeptId == deptId && d.IsDel == 0).FirstAsync();

            return null;
        }

        /// <summary>
        /// 根据三方 ID 查询本地用户 DB 实体
        /// </summary>
        private async Task<SysUserModel> QueryUserEntityByThirdPartyIdAsync(string thirdPartyId)
        {
            var mapping = await _db.Queryable<SysThirdPartyMappingModel>()
                .Where(m => m.EntityType == "User" && m.ThirdPartyId == thirdPartyId)
                .FirstAsync();

            if (mapping != null && long.TryParse(mapping.LocalId, out var userId))
                return await _db.Queryable<SysUserModel>().Where(u => u.UserId == userId && u.IsDel == 0).FirstAsync();

            return null;
        }

        /// <summary>
        /// 根据三方 ID 获取或创建本地部门实体
        /// </summary>
        private async Task<SysDeptModel> GetOrCreateDept(string provider, SyncDeptData dd)
        {
            var existing = await QueryDeptEntityByThirdPartyIdAsync(dd.ThirdPartyDeptId);
            return existing ?? new SysDeptModel();
        }

        /// <summary>
        /// 根据三方 ID 获取或创建本地用户实体
        /// </summary>
        private async Task<SysUserModel> GetOrCreateUser(string provider, SyncUserData du)
        {
            var existing = await QueryUserEntityByThirdPartyIdAsync(du.ThirdPartyUserId);
            return existing ?? new SysUserModel();
        }

        /// <summary>
        /// 解析部门的父部门关系
        /// </summary>
        private static void ResolveDeptParent(SysDeptModel dept, string thirdPartyParentId, Dictionary<string, long> mappingDict)
        {
            if (string.IsNullOrEmpty(thirdPartyParentId) || thirdPartyParentId == "0")
            {
                dept.ParentId = 0;
                dept.Ancestors = "0";
                return;
            }

            if (mappingDict.TryGetValue(thirdPartyParentId, out var parentLocalId))
            {
                dept.ParentId = parentLocalId;
                dept.Ancestors = parentLocalId.ToString(); // 精确 Ancestors 在 FixDeptAncestors 中修正
            }
            else
            {
                // 父部门尚未同步，先设占位符，后续修正
                dept.ParentId = 0;
                dept.Ancestors = "0";
            }
        }

        /// <summary>
        /// 二次修正部门的 Ancestors（首次保存时可能因父部门未就绪而使用占位符）
        /// </summary>
        private async Task FixDeptAncestors(string provider, List<SyncDeptData> depts, Dictionary<string, long> mappingDict)
        {
            foreach (var dd in depts)
            {
                if (string.IsNullOrEmpty(dd.ThirdPartyParentId) || dd.ThirdPartyParentId == "0")
                    continue;

                if (!mappingDict.TryGetValue(dd.ThirdPartyParentId, out var parentLocalId))
                    continue;

                if (!mappingDict.TryGetValue(dd.ThirdPartyDeptId, out var childLocalId))
                    continue;

                var parentMapping = await _db.Queryable<SysThirdPartyMappingModel>()
                    .Where(m => m.Provider == provider && m.EntityType == "Dept" && m.LocalId == parentLocalId.ToString())
                    .FirstAsync();

                // 从父部门的完整 Ancestors 推导子部门的 Ancestors
                var parentDept = await _db.Queryable<SysDeptModel>().Where(d => d.DeptId == parentLocalId).FirstAsync();
                if (parentDept == null) continue;

                var ancestors = string.IsNullOrEmpty(parentDept.Ancestors) || parentDept.Ancestors == "0"
                    ? parentLocalId.ToString()
                    : $"{parentDept.Ancestors},{parentLocalId}";

                await _db.Updateable<SysDeptModel>()
                    .SetColumns(d => d.ParentId == parentLocalId)
                    .SetColumns(d => d.Ancestors == ancestors)
                    .Where(d => d.DeptId == childLocalId)
                    .ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 保存/更新三方映射关系
        /// </summary>
        private async Task<bool> SaveMapping(string provider, string entityType, string thirdPartyId, string localId, string displayName)
        {
            var existing = await _db.Queryable<SysThirdPartyMappingModel>()
                .Where(m => m.Provider == provider && m.EntityType == entityType && m.ThirdPartyId == thirdPartyId)
                .FirstAsync();

            if (existing != null)
            {
                existing.LocalId = localId;
                existing.DisplayName = displayName;
                return await _db.Updateable(existing).ExecuteCommandAsync() > 0;
            }

            var mapping = new SysThirdPartyMappingModel
            {
                Provider = provider,
                EntityType = entityType,
                ThirdPartyId = thirdPartyId,
                LocalId = localId,
                DisplayName = displayName,
                CreateTime = DateTime.Now
            };

            return await _db.Insertable(mapping).ExecuteCommandAsync() > 0;
        }

        #endregion
    }
}
