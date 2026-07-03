export default (t) => ({
  type: "task",
  color: "#409eff",
  icon: "fa-cogs",
  label: t('flow.designer.node.task'),
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
    {
      label: t('flow.designer.handleRule'),
      param: "meta.handleRule",
      type: "radio",
      props: {
        isRow: false,
        options: [
          { key: "one", label: t('flow.designer.handleRuleOption.one') },
          { key: "all", label: t('flow.designer.handleRuleOption.all') },
          { key: "select", label: t('flow.designer.handleRuleOption.select') },
        ],
      },
    },
    {
      label: t('flow.designer.skipRepeat'),
      param: "meta.isNoRepeatHandle",
      type: "switch",
    },
  ],
  defaultValue: {
    name: t('flow.designer.node.task'),
    isNoRepeatHandle: true,
    handleBy: [],
    handleRule: 'one',
  }
});
