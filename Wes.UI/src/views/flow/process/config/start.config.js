export default (t) => ({
  type: "start",
  color: "#909399",
  icon: "fa-play-circle",
  label: t('flow.designer.node.start'),
  components: [
    {
      label: t('flow.designer.nodeName'),
      param: "meta.name",
      type: "input",
    },
  ],
  defaultValue: {
    name: t('flow.designer.node.start'),
  },
});
