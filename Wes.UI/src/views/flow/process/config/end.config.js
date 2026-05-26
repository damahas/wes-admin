export default {
  type: "end",
  color: "#909399",
  icon: "fa-stop-circle",
  label: "结束节点",
  components: [
    {
      label: "节点名称",
      param: "meta.name",
      type: "input",
    },
  ],
  defaultValue: {
    name: '结束节点',
  }
};
