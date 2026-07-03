export default (t) => ({
  type: "end",
  color: "#909399",
  icon: "fa-stop-circle",
  label: t('flow.designer.node.end'),
  components: [
    {
      label: t('flow.designer.nodeName'),
      param: "meta.name",
      type: "input",
    },
  ],
  defaultValue: {
    name: t('flow.designer.node.end'),
  }
});
