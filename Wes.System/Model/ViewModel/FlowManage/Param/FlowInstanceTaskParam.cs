namespace Wes.ViewModel.FlowManage
{
    public class FlowInstanceTaskParam
    {
        public long InstanceId { get; set; }
        public long TaskUserId { get; set; }
        public long ActualUserId { get; set; }
        public string? Comments { get; set; }
        public System.DateTime? HandleTime { get; set; }
        public int IsRecall { get; set; }
    }
}
