export default {
  type: "task",
  color: "#409eff",
  icon: "fa-cogs",
  label: "处理节点",
  components: [
    {
      label: "节点名称",
      param: "meta.name",
      type: "input",
    },
    {
      label: "处理人",
      param: "meta.handleBy",
      type: "handleUser",
    },
    {
      label: "处理方式",
      param: "meta.handleRule",
      type: "radio",
      props: {
        isRow: false,
        options: [
          { key: "one", label: "一个人同意" },
          { key: "all", label: "所有人同意" },
          { key: "select", label: "上一节点最后处理人选择" },
        ],
      },
    },
    {
      label: "重复审批自动跳过",
      param: "meta.isNoRepeatHandle",
      type: "switch",
    },
  ],
  defaultValue: {
    name: '处理节点',
    isNoRepeatHandle: true,
    handleBy: [],
    handleRule: 'one',
  }
};
