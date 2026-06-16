using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Wes.Utils.Model
{
    /// <summary>
    /// 接口返回值基类
    /// </summary>
    public class ReturnData
    {
        public long Code { set; get; }
        public string Msg { set; get; }

        public ReturnData()
        {
            this.Code = 200;
            this.Msg = "操作成功";
        }
        public ReturnData(long code, string msg)
        {
            this.Code = code;
            this.Msg = msg;
        }

    }

    /// <summary>
    /// 多行返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RowData<T> : ReturnData
    {
        public int total { set; get; }

        public List<T> Rows { set; get; }

        public RowData(long code, string msg) : base(code, msg)
        { }

        public RowData(List<T> rows)
        {
            this.Code = 200;
            this.Rows = rows;
        }
    }

    /// <summary>
    /// 结果返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultData<T> : ReturnData
    {

        public T Data { set; get; }

        public ResultData()
        { }

        public ResultData(long code, string msg) : base(code, msg)
        { }

        public ResultData(T Result)
        {
            this.Code = 200;
            this.Data = Result;
        }
    }

    /// <summary>
    /// 登录接口返回值
    /// </summary>
    public class LoginData : ReturnData
    {
        public string Token { set; get; }

        public UserInfo UserInfo { set; get; }

        public LoginData(long code, string msg) : base(code, msg)
        { }

        public LoginData(string token)
        {
            this.Code = 200;
            this.Token = token;
        }
    }

    /// <summary>
    /// 个人信息返回值
    /// </summary>
    public class UserInfoData : ReturnData
    {
        public UserInfo User { set; get; }

        public List<string> Roles
        {
            get
            {
                return this.User?.Roles?.Select(p => p.RoleKey).ToList() ?? new List<string>();
            }
        }

        public List<string> Permissions
        {
            get
            {
                if (this.Roles.Contains("admin"))
                {
                    return new List<string>() { "*:*:*" };
                }
                return this.User?.Permissions;
            }
        }

        public UserInfoData(long code, string msg) : base(code, msg)
        { }

        public UserInfoData(UserInfo userInfo)
        {
            this.Code = 200;
            this.User = userInfo;
        }
    }
}
