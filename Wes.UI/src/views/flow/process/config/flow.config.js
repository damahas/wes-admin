export default {
  label: "流程配置",
  components: [
    {
      label: "流程版本",
      param: "version",
      type: "label",
    },
    {
      label: "启用版本",
      param: "enableVersionId",
      type: "enableVersion",
    },
    {
      label: "创建时间",
      param: "createTime",
      type: "label",
    },
    {
      label: "备注",
      param: "remark",
      type: "input",
      props: {
        type: "textarea",
        rows: 3,
      },
    },
  ],
};
