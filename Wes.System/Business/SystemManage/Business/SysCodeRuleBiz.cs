using Newtonsoft.Json;
using NPOI.SS.Formula.PTG;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Wes.DbModel;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Cache;
using Wes.Utils.Extension;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysCodeRuleBiz : ISysCodeRuleBiz
    {
        private ISysCodeRuleService _sysCodeRuleService;
        private ISqlSugarClient client;
        // $ 结束符
        private string character = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ&";

        public SysCodeRuleBiz(ISysCodeRuleService sysCodeRuleService, ISqlSugarClient sqlSugarClient)
        {
            _sysCodeRuleService = sysCodeRuleService;
            client = sqlSugarClient;
        }

        public ResultData<CodeRuleInfo> GetById(long id)
        {
            var codeRule = _sysCodeRuleService.GetById(id);
            if (codeRule == null)
            {
                return new ResultData<CodeRuleInfo>(500, "找不到此编码规则！");
            }
            CodeRuleInfo result = codeRule.ToEntityCopy<SysCodeRuleModel, CodeRuleInfo>();
            result.Parts = _sysCodeRuleService.GetPartListByRuleId(id).OrderBy(p => p.Sort).ToList();
            return new ResultData<CodeRuleInfo>(result);
        }

        //public bool GetCode(long id, out string errMsg, out List<string> codes, int count)
        //{
        //    errMsg = "";
        //    codes = new List<string>();
        //    var key = Utils.CacheKey.CodeRule + id;
        //    var startTime = DateTime.Now;
        //    while (true)
        //    {
        //        // 超时逻辑 2min
        //        if ((DateTime.Now - startTime).Seconds > 120)
        //        {
        //            errMsg = "序号生成超时!";
        //            return false;
        //        }
        //        // 用缓存做的分布式锁
        //        if (CacheFactory.Cache.GetCache<string>(key) != null)
        //        {
        //            Thread.Sleep(100);
        //            continue;
        //        }
        //        string lockKey = Guid.NewGuid().ToString();
        //        CacheFactory.Cache.SetCache(key, lockKey);
        //        var cachLockKey = CacheFactory.Cache.GetCache<string>(key);
        //        if (cachLockKey != lockKey)
        //        {
        //            Thread.Sleep(100);
        //            continue;
        //        }
        //        // 处理序号生成
        //        var parts = _sysCodeRuleService.GetPartListByRuleId(id).OrderBy(p => p.Sort).ToList();
        //        for (int i = 0; i < count; i++)
        //        {
        //            string code = "";
        //            if (!PartsToCode(ref parts, out code, out errMsg))
        //            {
        //                CacheFactory.Cache.RemoveCache(key);
        //                return false;
        //            }
        //            codes.Add(code);
        //        }

        //        // 保存序号片段
        //        client.Ado.BeginTran();
        //        try
        //        {
        //            foreach (var item in parts)
        //            {
        //                _sysCodeRuleService.SavePart(item, client);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            client.Ado.RollbackTran();
        //            CacheFactory.Cache.RemoveCache(key);
        //            errMsg = $"编码片段更新失败，请重试";
        //            return false;
        //        }
        //        client.Ado.CommitTran();
        //        CacheFactory.Cache.RemoveCache(key);
        //        return true;
        //    }
        //}

        public string GetCode(string ruleCode, out string errMsg)
        {
            var rule = _sysCodeRuleService.GetByRuleCode(ruleCode);
            if (rule == null)
            {
                errMsg = $"找不到编码生成规则【{ruleCode}】";
                return null;
            }
            return GetCode(rule.RuleId, out errMsg);
        }

        public string GetCode(long id, out string errMsg)
        {
            errMsg = "";
            string code = "";
            var key = Utils.CacheKey.CodeRule + id;
            var startTime = DateTime.Now;
            while (true)
            {
                // 超时逻辑 2min
                if ((DateTime.Now - startTime).Seconds > 120)
                {
                    errMsg = "序号生成超时!";
                    return null;
                }
                // 用缓存做的分布式锁
                if (CacheFactory.Cache.GetCache<string>(key) != null)
                {
                    Thread.Sleep(100);
                    continue;
                }
                string lockKey = Guid.NewGuid().ToString();
                CacheFactory.Cache.SetCache(key, lockKey);
                var cachLockKey = CacheFactory.Cache.GetCache<string>(key);
                if (cachLockKey != lockKey)
                {
                    Thread.Sleep(100);
                    continue;
                }
                // 处理序号生成
                var parts = _sysCodeRuleService.GetPartListByRuleId(id).OrderBy(p => p.Sort).ToList();
                if (!PartsToCode(ref parts, out code, out errMsg))
                {
                    CacheFactory.Cache.RemoveCache(key);
                    return null;
                }
                // 保存序号片段
                client.Ado.BeginTran();
                try
                {
                    foreach (var item in parts)
                    {
                        _sysCodeRuleService.SavePart(item, client);
                    }
                }
                catch (Exception ex)
                {
                    client.Ado.RollbackTran();
                    CacheFactory.Cache.RemoveCache(key);
                    errMsg = $"编码片段更新失败，请重试";
                    return null;
                }
                client.Ado.CommitTran();
                CacheFactory.Cache.RemoveCache(key);
                return code;
            }
        }

        public ResultData<List<SysCodeRuleModel>> GetAll()
        {
            return new ResultData<List<SysCodeRuleModel>>(_sysCodeRuleService.GetAll());
        }

        public RowData<SysCodeRuleModel> GetList(ParamData<CodeRuleParam> param)
        {
            int total = 0;
            RowData<SysCodeRuleModel> result = new RowData<SysCodeRuleModel>(_sysCodeRuleService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Save(CodeRuleInfo model)
        {
            client.Ado.BeginTran();
            try
            {
                if (!_sysCodeRuleService.Save(model, client))
                {
                    client.Ado.RollbackTran();
                    return new ReturnData(500, "保存失败！");
                }
                // 处理代码片段
                var parts = _sysCodeRuleService.GetPartListByRuleId(model.RuleId).ToDictionary(p => p.PartId, p => p);
                foreach (var item in model.Parts)
                {
                    item.RuleId = model.RuleId;
                    // 这里处理重置问题
                    SysCodeRulePartModel oldPart = null;
                    if (parts.ContainsKey(item.PartId))
                    {
                        oldPart = parts[item.PartId];
                        parts.Remove(item.PartId);
                    }
                    if (item.PartType == "calc")
                    {
                        if (oldPart == null || !(oldPart.PartType == "calc" && oldPart.PartValue == item.PartValue))
                        {
                            // 重置编码位置
                            var errMsg = ResetPartIndex(item);
                            if (!string.IsNullOrWhiteSpace(errMsg))
                            {
                                client.Ado.RollbackTran();
                                return new ReturnData(500, errMsg);
                            }
                        }
                        item.ResetTime = GetNextResetTime(item);
                    }
                    _sysCodeRuleService.SavePart(item, client);
                }
                _sysCodeRuleService.DeletePart(parts.Keys.ToList(), client);
                client.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                client.Ado.RollbackTran();
                return new ReturnData(500, ex.ToString());
            }

            return new ReturnData();
        }

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysCodeRuleService.Delete(delIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        #region 私有方法

        private bool PartsToCode(ref List<SysCodeRulePartModel> parts, out string code, out string errMsg)
        {
            errMsg = "";
            code = "";
            foreach (var part in parts)
            {
                switch (part.PartType)
                {
                    case "string":
                        code += part.PartValue;
                        break;
                    case "date":
                        try
                        {
                            code += DateTime.Now.ToString(part.PartValue);
                        }
                        catch (Exception ex)
                        {
                            errMsg = $"编码片段时间格式{part.PartValue}解析失败";
                            return false;
                        }
                        break;
                    case "calc":
                        var calcParts = JsonConvert.DeserializeObject<List<CodePartInfo>>(part.CurrentIndex);
                        // 重置逻辑
                        if (DateTime.Now < part.ResetTime)
                        {
                            part.ResetTime = GetNextResetTime(part);
                            // 重置currentIndex
                            errMsg = ResetPartIndex(part);
                            if (!string.IsNullOrWhiteSpace(errMsg))
                            {
                                return false;
                            }
                        }
                        // 获取序号逻辑
                        string calcCode = GetCalcPartCode(calcParts, part.PartValue, out errMsg);
                        if (!string.IsNullOrWhiteSpace(errMsg))
                        {
                            return false;
                        }
                        // 跳过全部0
                        if (part.IsSkipZero > 0 && calcCode.Trim('0').Length == 0)
                        {
                            calcCode = GetCalcPartCode(calcParts, part.PartValue, out errMsg);
                            if (!string.IsNullOrWhiteSpace(errMsg))
                            {
                                return false;
                            }
                        }
                        part.CurrentIndex = JsonConvert.SerializeObject(calcParts);
                        code += calcCode;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        private DateTime GetNextResetTime(SysCodeRulePartModel part)
        {
            switch (part.ResetType)
            {
                case "week":
                    if (part.WeekStartDay > (int)DateTime.Now.DayOfWeek)
                    {
                        return DateTime.Now.AddDays(part.WeekStartDay - (int)DateTime.Now.DayOfWeek).Date;
                    }
                    else
                    {
                        return DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek + part.WeekStartDay).Date;
                    }
                case "month":
                    return DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).Date;
                case "quarter":
                    return Convert.ToDateTime(DateTime.Now.AddMonths(0 - (DateTime.Now.Month - 1) % 3).ToString("yyyy-MM-01"));
                case "year":
                    return Convert.ToDateTime($"{DateTime.Now.Year}-01-01").AddYears(1);
                default:
                    return DateTime.MinValue;
            }
        }

        private string ResetPartIndex(SysCodeRulePartModel part)
        {
            List<CodePartInfo> partInfos = new List<CodePartInfo>();
            var partValue = part.PartValue.TrimStart('[').TrimEnd(']');
            if (string.IsNullOrWhiteSpace(partValue))
            {
                client.Ado.RollbackTran();
                return part.PartValue + " 解析失败！字符规则不能为空";
            }
            var partChar = partValue.Split("][");
            foreach (var charSection in partChar)
            {
                var chars = charSection.Split('-');
                if (chars.Length != 2)
                {
                    client.Ado.RollbackTran();
                    return part.PartValue + " 解析失败！字符规则有误";
                }
                var startIndex = character.IndexOf(chars[0]);
                var endIndex = character.IndexOf(chars[1]);
                if (startIndex < 0 || endIndex < 0 || startIndex > endIndex)
                {
                    client.Ado.RollbackTran();
                    return part.PartValue + " 解析失败！只能取[a-z][A-Z][0-9]";
                }
                partInfos.Add(new CodePartInfo()
                {
                    StartChar = chars[0],
                    EndChar = chars[1],
                    CurrentChar = chars[0],
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    CurrentIndex = startIndex
                });
            }
            part.CurrentIndex = JsonConvert.SerializeObject(partInfos);
            return string.Empty;
        }

        private string GetCalcPartCode(List<CodePartInfo> calcParts, string partValue, out string errMsg)
        {
            errMsg = string.Empty;
            string calcCode = "";
            bool iscarry = true;
            for (int i = calcParts.Count - 1; i > -1; i--)
            {
                var curPart = calcParts[i];
                if (curPart.CurrentIndex > curPart.EndIndex)
                {
                    errMsg = $"编码片段{partValue}已经用完，请修改规则或等重置时间";
                    return calcCode;
                }
                if (!iscarry)
                {
                    calcCode = curPart.CurrentChar + calcCode;
                    continue;
                }
                calcCode = curPart.CurrentChar + calcCode;
                // 最后一个数，如果是第一位则进1溢出，否则回滚到开始
                if (curPart.CurrentIndex == curPart.EndIndex)
                {
                    curPart.CurrentIndex = curPart.StartIndex;
                    iscarry = true;
                    if (i == 0)
                    {
                        curPart.CurrentIndex = curPart.EndIndex + 1;
                    }
                }
                else
                {
                    curPart.CurrentIndex = curPart.CurrentIndex + 1;
                    iscarry = false;
                }
                curPart.CurrentChar = character[curPart.CurrentIndex].ToString();
            }
            return calcCode;
        }

        #endregion

    }
}
