export default (t) => ({
  label: t('flow.designer.flowConfig'),
  components: [
    {
      label: t('flow.designer.version'),
      param: "version",
      type: "label",
    },
    {
      label: t('flow.designer.enableVersion'),
      param: "enableVersionId",
      type: "enableVersion",
    },
    {
      label: t('flow.designer.createTime'),
      param: "createTime",
      type: "label",
    },
    {
      label: t('flow.designer.remark'),
      param: "remark",
      type: "input",
      props: {
        type: "textarea",
        rows: 3,
      },
    },
  ],
});
