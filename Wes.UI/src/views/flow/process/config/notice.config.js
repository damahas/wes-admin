export default {
  type: "notice",
  color: "#e6a23c",
  icon: "fa-bell",
  label: "通知节点",
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
  ],
  defaultValue: {
    name: '通知节点',
  },
};
