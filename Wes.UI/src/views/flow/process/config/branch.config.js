export default (t) => ({
  type: "branch",
  color: "#f0b04a",
  icon: "fa-code-fork",
  label: t('flow.designer.node.branch'),
  components: [
    {
      label: t('flow.designer.nodeName'),
      param: "meta.name",
      type: "input",
    },
    {
      label: t('flow.designer.condition'),
      param: "meta.conditions",
      type: "conditions",
    },
  ],
  defaultValue: {
    name: t('flow.designer.node.branch'),
    conditions: [],
  },
});
