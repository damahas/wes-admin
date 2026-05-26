using System;
using System.Linq;
using Wes.Service;
using Wes.Utils.Extension;
using Wes.DbModel;
using Wes.Utils.Model;
using System.Collections.Generic;

namespace Wes.Business
{
    public class FlowProcessBiz : IFlowProcessBiz
    {
        private IFlowProcessService _flowProcessService;
        private IFlowProcessVersionService _flowProcessVersionService;

        public FlowProcessBiz(IFlowProcessService flowProcessService, IFlowProcessVersionService flowProcessVersionService)
        {
            _flowProcessService = flowProcessService;
            _flowProcessVersionService = flowProcessVersionService;
        }

        public ResultData<FlowProcessModel> GetById(long id)
        {
            return new ResultData<FlowProcessModel>(_flowProcessService.GetById(id));
        }

        public ResultData<List<FlowProcessModel>> GetAll()
        {
            return new ResultData<List<FlowProcessModel>>(_flowProcessService.GetAll());
        }

        public RowData<FlowProcessModel> GetList(ParamData<FlowProcessModel> param)
        {
            int total = 0;
            RowData<FlowProcessModel> result = new RowData<FlowProcessModel>(_flowProcessService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Save(FlowProcessModel model)
        {
            bool isNew = model.ProcessId == 0 ? true : false;
            if (!_flowProcessService.Save(model))
            {
                return new ReturnData(500, "保存失败！");
            }
            if (isNew)
            {
                var version = new FlowProcessVersionModel()
                {
                    Version = "V1",
                    IsLock = 0,
                    ProcessId = model.ProcessId,
                    Content = ""
                };
                _flowProcessVersionService.Save(version);
                model.CurVersionId = version.VersionId;
                _flowProcessService.Save(model);
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
            if (_flowProcessService.Delete(delIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        #region 流程版本

        public ResultData<FlowProcessVersionModel> GetVersionById(long id)
        {
            return new ResultData<FlowProcessVersionModel>(_flowProcessVersionService.GetById(id));
        }

        public ResultData<List<FlowProcessVersionModel>> GetVersionAll()
        {
            return new ResultData<List<FlowProcessVersionModel>>(_flowProcessVersionService.GetAll());
        }

        public RowData<FlowProcessVersionModel> GetVersionList(ParamData<FlowProcessVersionModel> param)
        {
            int total = 0;
            RowData<FlowProcessVersionModel> result = new RowData<FlowProcessVersionModel>(_flowProcessVersionService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData SaveVersion(FlowProcessVersionModel model)
        {
            if (model.VersionId > 0)
            {
                var version = _flowProcessVersionService.GetById(model.VersionId);
                if (version == null)
                {
                    return new ReturnData(500, "找不到版本");
                }
                if (version.IsLock > 0)
                {
                    return new ReturnData(500, "当前版本已锁定");
                }
            }
            else
            {
                var lastVersion = _flowProcessVersionService.GetLastVersion(model.ProcessId);
                int versionNo = 1;
                if (lastVersion != null)
                {
                    versionNo = int.Parse(lastVersion.Version.Replace("V", "")) + 1;
                }
                model.Version = $"V{versionNo}";
            }
            if (!_flowProcessVersionService.Save(model))
            {
                return new ReturnData(500, "保存失败！");
            }
            return new ResultData<FlowProcessVersionModel>(model);
        }

        public ReturnData DeleteVersion(long versionId)
        {
            var version = _flowProcessVersionService.GetById(versionId);
            if (version != null && version.VersionId == version.Process?.CurVersionId)
            {
                return new ReturnData(500, "当前版本已启用，无法删除");
            }
            if (_flowProcessVersionService.Delete(new List<long> { version.VersionId }))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public ReturnData CopyVersion(long versionId)
        {
            var version = _flowProcessVersionService.GetById(versionId);
            if (version == null)
            {
                return new ReturnData(500, "找不到版本");
            }
            version.VersionId = 0;
            version.IsLock = 0;
            return SaveVersion(version);
        }

        public ReturnData UseVersion(long versionId)
        {
            var version = _flowProcessVersionService.GetById(versionId);
            if (version == null)
            {
                return new ReturnData(500, "找不到版本");
            }
            var process = _flowProcessService.GetById(version.ProcessId);
            if (process == null)
            {
                return new ReturnData(500, "找不到流程");
            }
            process.CurVersionId = versionId;
            _flowProcessService.Save(process);
            return new ReturnData();
        }

        #endregion
    }
}
