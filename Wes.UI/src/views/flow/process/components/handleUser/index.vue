<template>
  <div class="handle-user">
    <div class="link_add_btn">
      <el-dropdown trigger="hover" @command="handleSlected">
        <el-link type="primary" style="font-size: 14px" :disabled="readonly">
          添加
        </el-link>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="author">发起人</el-dropdown-item>
            <el-dropdown-item command="role">角色</el-dropdown-item>
            <el-dropdown-item command="dept">部门</el-dropdown-item>
            <el-dropdown-item command="leader">部门负责人</el-dropdown-item>
            <el-dropdown-item command="user">指定人</el-dropdown-item>
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
import { ref, computed } from "vue";
import selectUser from "@/views/system/user/select.vue";
import selectRole from "@/views/system/role/select.vue";
import selectDept from "@/views/system/dept/select.vue";

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
  switch (row.type) {
    case "author":
      return "发起人";
    case "leader":
      return "部门负责人(为空往上级找)";
    case "role":
      return "角色：" + row.handleName;
    case "dept":
      return "部门：" + row.handleName;
    case "user":
      return "用户：" + row.handleName;
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
  color: #606266;
  margin-top: 8px;

  .drag-icon {
    width: 24px;
    color: #909399;
    font-size: 12px;
    text-align: center;
  }

  .row-content {
    border-radius: 4px;
    background-color: #f4f4f5;
    flex: 1;
    height: 28px;
    line-height: 28px;
    padding: 0 8px;
    font-size: 13px;
    color: #606266;

    i {
      margin-right: 6px;
      color: #909399;
      font-size: 13px;
    }
  }

  .delete-icon {
    width: 24px;
    color: #c0c4cc;
    font-size: 14px;
    text-align: center;
    cursor: pointer;
    transition: color 0.2s;

    &:hover {
      color: #909399;
    }
  }
}
</style>
