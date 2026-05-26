<template>
  <el-tag :type="getStatusType(status)" effect="plain" size="small">
    {{ getStatusTitle(status) }}
  </el-tag>
</template>

<script>
export default {
  props: {
    status: String,
  },
  methods: {
    getStatusType(status) {
      switch (status) {
        case "start":
          return "info";
        case "doing":
          return "";
        case "pass":
          return "success";
        case "unpass":
          return "danger";
        case "pending":
          return "warning";
        default:
          return "";
      }
    },
    getStatusTitle(status) {
      switch (status) {
        case "start":
          return "待处理";
        case "doing":
          return "审批中";
        case "pass":
          return "通过";
        case "unpass":
          return "不通过";
        case "pending":
          return "挂起";
        case "auto":
          return "自动处理";
        case "delegation":
          return "委托";
        default:
          return status;
      }
    },
  },
};
</script>
