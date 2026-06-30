<template>
  <el-tag :type="getStatusType(status)" effect="plain" size="small">
    {{ getStatusTitle(status) }}
  </el-tag>
</template>

<script>
export default {
  props: {
    status: Number,
  },
  methods: {
    getStatusType(status) {
      switch (status) {
        case 0:
          return "info";
        case 10:
          return "";
        case 100:
          return "success";
        case 101:
          return "danger";
        case 200:
          return "warning";
        case 201:
          return "warning";
        case 9999:
          return "";
        default:
          return "";
      }
    },
    getStatusTitle(status) {
      switch (status) {
        case 0:
          return "待处理";
        case 10:
          return "审批中";
        case 100:
          return "通过";
        case 101:
          return "不通过";
        case 200:
          return "挂起";
        case 201:
          return "委托";
        case 9999:
          return "自动处理";
        default:
          return status;
      }
    },
  },
};
</script>
