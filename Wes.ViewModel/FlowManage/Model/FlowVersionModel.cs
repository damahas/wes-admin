using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Wes.DbModel;

namespace Wes.ViewModel.FlowManage
{
    /// <summary>
    /// 流程版本实体
    /// </summary>
    public class FlowVersionModel
    {
        public long processId { set; get; }
        public long versionId { set; get; }
        public List<FlowVersionNodeModel> nodes { get; set; }
        public List<FlowVersionLineModel> lines { get; set; }
    }

    public class FlowVersionNodeModel
    {
        public string id { get; set; }
        public FlowVersionNodeMateModel meta { get; set; }
    }

    public class FlowVersionNodeMateModel
    {
        public string name { get; set; }
        public FlowNodeTypeEnum type { get; set; }
        public bool isNoRepeatHandle { get; set; }
        public bool isCanEmpty { get; set; }
        public List<FlowVersionNodeMateHandleModel> handleBy { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NodeHandleTypeEnum handleRule { get; set; }
    }

    public class FlowVersionNodeMateHandleModel
    {
        // author发起人 leader部门负责人 role角色 dept部门 user员工
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NodeHandleByEnum type { get; set; }
        public long handleId { set; get; }
    }

    public class FlowVersionLineModel
    {
        public string id { get; set; }
        public string startId { get; set; }
        public string endId { get; set; }
        public object meta { get; set; }
    }

}
