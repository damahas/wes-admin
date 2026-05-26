export default {
  type: "start",
  color: "#909399",
  icon: "fa-play-circle",
  label: "开始节点",
  components: [
    {
      label: "节点名称",
      param: "meta.name",
      type: "input",
    },
  ],
  defaultValue: {
    name: '结束节点',
  },
};
