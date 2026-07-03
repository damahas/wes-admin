export default (t) => ({
  type: "notice",
  color: "#e6a23c",
  icon: "fa-bell",
  label: t('flow.designer.node.notice'),
  components: [
    {
      label: t('flow.designer.nodeName'),
      param: "meta.name",
      type: "input",
    },
    {
      label: t('flow.designer.handleUser'),
      param: "meta.handleBy",
      type: "handleUser",
    },
  ],
  defaultValue: {
    name: t('flow.designer.node.notice'),
  },
});
