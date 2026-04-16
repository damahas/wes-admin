using System;
using SqlSugar;
using System.Collections.Generic;
using Wes.DbModel;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.Service
{
    public class SysFileService : Repository<SysFileModel>, ISysFileService
    {
        public SysFileService(ISqlSugarClient db) : base(db) { }

        public List<SysFileModel> GetList(ParamData<SysFileModel> param, out int total)
        {
            Expressionable<SysFileModel> express = Expressionable.Create<SysFileModel>();
            express.And(p => p.IsDel == 0);
            if (param.Params != null)
            {
                if (!string.IsNullOrWhiteSpace(param.Params.FileName))
                {
                    express.And(p => p.FileName == param.Params.FileName.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.FileType))
                {
                    express.And(p => p.FileType == param.Params.FileType.Trim());
                }
                if (param.Params.FileSize > 0)
                {
                    express.And(p => p.FileSize == param.Params.FileSize);
                }
                if (!string.IsNullOrWhiteSpace(param.Params.FilePath))
                {
                    express.And(p => p.FilePath == param.Params.FilePath.Trim());
                }
                if (!string.IsNullOrWhiteSpace(param.Params.TableName))
                {
                    express.And(p => p.TableName == param.Params.TableName.Trim());
                }
                if (param.Params.TableId != null)
                {
                    express.And(p => p.TableId == param.Params.TableId);
                }
            }
            total = 0;
            var query = Context.Queryable<SysFileModel>().Where(express.ToExpression());
            if (param.PageSize == 0)
                return query.ToList();
            return query.ToPageList(param.PageNum, param.PageSize, ref total);
        }

        public List<SysFileModel> GetListByTableId(string tableName, long tableId)
        {
            return GetList(p => p.IsDel == 0 && p.TableName == tableName && p.TableId == tableId);
        }

        public List<SysFileModel> GetAll()
        {
            return GetList(p => p.IsDel == 0);
        }

        public SysFileModel GetById(long id)
        {
            return GetFirst(p => p.FileId == id && p.IsDel == 0);
        }

        public bool Save(SysFileModel model)
        {
            if (model.FileId > 0)
            {
                return Update(model);
            }
            model.IsDel = 0;
            model.CreateTime = DateTime.Now;
            model.CreateBy = GlobalContext.CurrentUser.Account;
            model.FileId = Context.Insertable(model).ExecuteReturnSnowflakeId();
            return model.FileId > 0;
        }

        public bool Delete(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return false;
            return Context.Updateable<SysFileModel>().SetColumns(p => p.IsDel == 1)
                .Where(p => ids.Contains(p.FileId)).ExecuteCommand() > 0;
        }

    }
}
