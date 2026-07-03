<template>
  <div class="handle-user">
    <div class="link_add_btn">
      <el-dropdown trigger="hover" @command="handleSlected">
        <el-link type="primary" style="font-size: 14px" :disabled="readonly">
          {{ t('flow.designer.add') }}
        </el-link>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="author">{{ t('flow.designer.userType.author') }}</el-dropdown-item>
            <el-dropdown-item command="role">{{ t('flow.designer.userType.role') }}</el-dropdown-item>
            <el-dropdown-item command="dept">{{ t('flow.designer.userType.dept') }}</el-dropdown-item>
            <el-dropdown-item command="leader">{{ t('flow.designer.userType.leader') }}</el-dropdown-item>
            <el-dropdown-item command="user">{{ t('flow.designer.userType.user') }}</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
    <div v-if="value && value.length">
      <div v-for="(row, index) in value" :key="index" class="handle-user-row">
        <i class="fa fa-arrows drag-icon"></i>
        <div class="row-content">
          <i :class="toIcon(row.type)"></i>{{ toShowName(row) }}
        </div>
        <i class="fa fa-times delete-icon" @click="handleRemove(index)"></i>
      </div>
    </div>

    <select-user ref="selectUserRef" @handleData="handleSelectUser"></select-user>
    <select-role ref="selectRoleRef" @handleData="handleSelectRole"></select-role>
    <select-dept ref="selectDeptRef" @handleData="handleSelectDept"></select-dept>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import selectUser from "@/views/system/user/select.vue";
import selectRole from "@/views/system/role/select.vue";
import selectDept from "@/views/system/dept/select.vue";

const { t } = useI18n();

const props = defineProps({
  value: {
    type: Array,
    default: () => [],
  },
  readonly: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["updateValue"]);

const selectUserRef = ref(null);
const selectRoleRef = ref(null);
const selectDeptRef = ref(null);

const toIcon = (icon) => {
  switch (icon) {
    case "author":
      return "fa fa-user";
    case "leader":
      return "fa fa-user-tie";
    case "role":
      return "fa fa-users";
    case "dept":
      return "fa fa-sitemap";
    case "user":
      return "fa fa-user";
    default:
      return "";
  }
};

const toShowName = (row) => {
  const typeLabel = t('flow.designer.userType.' + row.type);
  switch (row.type) {
    case "author":
      return typeLabel;
    case "leader":
      return t('flow.designer.leaderDesc');
    case "role":
      return typeLabel + "：" + row.handleName;
    case "dept":
      return typeLabel + "：" + row.handleName;
    case "user":
      return typeLabel + "：" + row.handleName;
    default:
      return "";
  }
};

const handleSlected = (e) => {
  let form = {
    type: e,
  };
  switch (e) {
    case "author":
    case "leader":
      break;
    case "role":
      selectRoleRef.value?.open();
      return;
    case "dept":
      selectDeptRef.value?.open();
      return;
    case "user":
      selectUserRef.value?.open();
      return;
    default:
      return;
  }
  emit("updateValue", [...props.value, form]);
};

const handleRemove = (index) => {
  const newValue = [...props.value];
  newValue.splice(index, 1);
  emit("updateValue", newValue);
};

const handleSelectUser = (rows) => {
  if (!rows || !rows.length) return;
  emit("updateValue", [
    ...props.value,
    ...rows.map((p) => ({
      type: "user",
      handleId: p.userId,
      handleName: p.userName,
    })),
  ]);
};

const handleSelectRole = (rows) => {
  if (!rows || !rows.length) return;
  emit("updateValue", [
    ...props.value,
    ...rows.map((p) => ({
      type: "role",
      handleId: p.roleId,
      handleName: p.roleName,
    })),
  ]);
};

const handleSelectDept = (rows) => {
  if (!rows || !rows.length) return;
  emit("updateValue", [
    ...props.value,
    ...rows.map((p) => ({
      type: "dept",
      handleId: p.deptId,
      handleName: p.deptName,
    })),
  ]);
};
</script>

<style lang="scss" scoped>
.handle-user {
  .link_add_btn {
    position: absolute;
    right: 0;
    top: 0;
  }
}

.handle-user-row {
  display: flex;
  align-items: center;
  height: 28px;
  color: var(--text-primary);
  margin-top: 8px;

  .drag-icon {
    width: 24px;
    color: var(--text-secondary);
    font-size: 12px;
    text-align: center;
  }

  .row-content {
    border-radius: 4px;
    background-color: var(--bg-hover);
    flex: 1;
    height: 28px;
    line-height: 28px;
    padding: 0 8px;
    font-size: 13px;
    color: var(--text-primary);

    i {
      margin-right: 6px;
      color: var(--text-secondary);
      font-size: 13px;
    }
  }

  .delete-icon {
    width: 24px;
    color: var(--text-placeholder);
    font-size: 14px;
    text-align: center;
    cursor: pointer;
    transition: color 0.2s;

    &:hover {
      color: var(--text-secondary);
    }
  }
}
</style>
