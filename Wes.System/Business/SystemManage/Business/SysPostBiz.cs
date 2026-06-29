using System;
using System.Collections.Generic;
using System.Text;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Extension;
using Wes.DbModel;
using Wes.Utils.Model;
using Wes.Utils;
using System.Linq;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public class SysPostBiz : ISysPostBiz
    {
        private ISysPostService _sysPostService;
        private ISysDicDataService _sysDicDataService;

        public SysPostBiz(ISysPostService sysPostService, ISysDicDataService sysDicDataService)
        {
            _sysPostService = sysPostService;
            _sysDicDataService = sysDicDataService;
        }

        #region 岗位操作

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysPostService.Delete(delIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public ResultData<SysPostModel> GetById(long id)
        {
            return new ResultData<SysPostModel>(_sysPostService.GetById(id));
        }

        public RowData<SysPostModel> GetList(ParamData<PostParam> param)
        {
            int total = 0;
            RowData<SysPostModel> result = new RowData<SysPostModel>(_sysPostService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public ReturnData Save(SysPostModel post)
        {
            var exist = _sysPostService.GetByPostCode(post.PostCode);
            if (exist != null && exist.PostId != post.PostId)
            {
                return new ReturnData(500, "已存在该岗位编码，请勿重复添加！");
            }
            if (_sysPostService.Save(post))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "保存失败！");
        }

        public byte[] Export(ParamData<PostParam> param)
        {
            int total = 0;
            param.PageSize = 0;
            return NPOIHepler.ExportExcel(_sysPostService.GetList(param, out total), _sysDicDataService);
        }

        #endregion
    }
}
