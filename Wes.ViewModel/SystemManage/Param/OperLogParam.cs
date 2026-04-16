using System;
namespace Wes.ViewModel.SystemManage
{
	public class OperLogParam
	{
		/// <summary>
		/// 模块标题
		/// </summary>
		public string Title { set; get; }

		/// <summary>
		/// 操作人员
		/// </summary>
		public string OperName { set; get; }

		/// <summary>
		/// 业务类型（0其它 1新增 2修改 3删除）
		/// </summary>
		public int? BusinessType { set; get; }

		/// <summary>
		/// 操作状态（0正常 1异常）
		/// </summary>
		public int? Status { set; get; }

		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime? BeginTime { set; get; }

		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime? EndTime { set; get; }
	}
}

