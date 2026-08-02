export default (t) => ({
  label: t('flow.designer.line'),
  components: [
    {
      label: t('flow.designer.lineName'),
      param: "name",
      type: "input",
    },
    {
      label: t('flow.designer.branchCondition'),
      param: "conditionId",
      type: "conditionSelect",
    },
  ],
});
