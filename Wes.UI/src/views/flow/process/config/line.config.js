export default (t) => ({
  label: t('flow.designer.line'),
  components: [
    {
      label: t('flow.designer.lineName'),
      param: "meta.name",
      type: "input",
    },
  ],
});
