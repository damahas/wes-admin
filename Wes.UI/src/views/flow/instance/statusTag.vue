<template>
  <el-tag :type="getStatusType(status)" effect="plain" size="small">
    {{ getStatusTitle(status) }}
  </el-tag>
</template>

<script>
import { useI18n } from "vue-i18n";

export default {
  props: {
    status: Number,
  },
  setup() {
    const { t } = useI18n();
    const statusMap = {
      0: "flow.instance.statusMap.start",
      10: "flow.instance.statusMap.approving",
      100: "flow.instance.statusMap.approved",
      101: "flow.instance.statusMap.rejected",
      200: "flow.instance.statusMap.suspended",
      201: "flow.instance.statusMap.delegated",
      9999: "flow.instance.statusMap.autoProcessed",
    };
    return { t, statusMap };
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
      return this.t(this.statusMap[status]) || status;
    },
  },
};
</script>
