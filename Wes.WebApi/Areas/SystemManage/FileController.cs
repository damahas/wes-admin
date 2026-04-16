using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Utils.Model;
using Wes.DbModel;
using Wes.ViewModel.SystemManage;
using Microsoft.AspNetCore.Http;
using Wes.Utils;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Route("system/file")]
    public class FileController : ControllerBase
    {
        private ISysFlieBiz _sysFlieBiz;

        public FileController(ISysFlieBiz sysFlieBiz)
        {
            _sysFlieBiz = sysFlieBiz;
        }

        [HttpPost]
        [Route("upload")]
        public ReturnData UploadFile(IFormFile file)
        {
            string path = $"{GlobalContext.AppSettings.FilePath.TrimEnd('/')}/{DateTime.Now.ToString("yyyyMM")}/{DateTime.Now.ToString("dd")}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = Guid.NewGuid().ToString("N");
            string fileType = "";
            if (file.FileName.Split(".").Length > 1)
            {
                fileType = file.FileName.Split(".")[1];
            }
            string filePath = $"{path}/{fileName}.{fileType}";
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            return _sysFlieBiz.Save(new SysFileModel()
            {
                FileName = file.FileName,
                FilePath = filePath.Replace(GlobalContext.AppSettings.FilePath.TrimEnd('/'), ""),
                FileSize = file.Length,
                FileType = fileType,
            });
        }
    }
}