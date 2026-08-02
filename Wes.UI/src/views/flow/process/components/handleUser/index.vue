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
            <el-dropdown-item command="dataservice">{{ t('flow.designer.userType.dataservice') }}</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
    <div v-if="value && value.length" ref="listRef">
      <div v-for="(row, index) in value" :key="getItemKey(row, index)" class="handle-user-row">
        <i class="fa fa-arrows drag-icon" :class="{ 'is-disabled': readonly }"></i>
        <div class="row-content">
          <i :class="toIcon(row.type)"></i>{{ toShowName(row) }}
        </div>
        <i
          v-if="row.type === 'dataservice' && !readonly"
          class="fa fa-edit edit-icon"
          @click="openDsDialog(index, row)"
        ></i>
        <i class="fa fa-times delete-icon" @click="handleRemove(index)"></i>
      </div>
    </div>

    <select-user ref="selectUserRef" @handleData="handleSelectUser"></select-user>
    <select-role ref="selectRoleRef" @handleData="handleSelectRole"></select-role>
    <select-dept ref="selectDeptRef" @handleData="handleSelectDept"></select-dept>

    <!-- 数据服务选择弹窗 -->
    <el-dialog
      v-model="dsDialogVisible"
      :title="t('flow.designer.userType.dataservice')"
      width="560px"
      append-to-body
    >
      <el-form label-width="100px" @submit.prevent>
        <el-form-item :label="t('flow.designer.userType.dataservice')">
          <el-select
            v-model="dsForm.dsId"
            filterable
            :placeholder="t('common.pleaseSelect')"
            style="width: 100%"
            @change="onDsChange"
          >
            <el-option
              v-for="item in dsOptions"
              :key="item.dsId"
              :label="item.serviceName"
              :value="item.dsId"
            />
          </el-select>
        </el-form-item>
        <el-form-item
          v-for="param in dsForm.params"
          :key="param.key"
          :label="param.key"
        >
          <el-input
            v-model="param.value"
            :placeholder="`${param.type || 'string'}`"
          />
        </el-form-item>
        <el-form-item v-if="dsForm.params.length === 0 && dsForm.dsId">
          <el-text type="info" size="small">该数据服务无输入参数</el-text>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dsDialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" @click="confirmDs">{{ t('common.confirm') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch, nextTick } from "vue";
import { useI18n } from "vue-i18n";
import Sortable from "sortablejs";
import selectUser from "@/views/system/user/select.vue";
import selectRole from "@/views/system/role/select.vue";
import selectDept from "@/views/system/dept/select.vue";
import { listDataService, getDataService } from "@/api/system/dataService";

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

const listRef = ref(null);
let sortableInstance = null;

function initSortable() {
  if (sortableInstance) sortableInstance.destroy();
  if (!listRef.value || props.readonly) return;
  sortableInstance = Sortable.create(listRef.value, {
    handle: ".drag-icon",
    animation: 200,
    ghostClass: "handle-user-row--ghost",
    onEnd(evt) {
      if (evt.oldIndex === evt.newIndex) return;
      const newValue = [...props.value];
      const moved = newValue.splice(evt.oldIndex, 1)[0];
      newValue.splice(evt.newIndex, 0, moved);
      emit("updateValue", newValue);
    },
  });
}

watch(
  () => [props.value.length, props.readonly],
  () => nextTick(initSortable)
);

onBeforeUnmount(() => {
  if (sortableInstance) sortableInstance.destroy();
});

const selectUserRef = ref(null);
const selectRoleRef = ref(null);
const selectDeptRef = ref(null);

// ==================== 数据服务 ====================
const dsDialogVisible = ref(false);
const dsOptions = ref([]);
const dsForm = ref({ dsId: null, serviceName: "", serviceCode: "", params: [] });
// 编辑模式：记录当前编辑的行索引，null 表示新增
const dsEditIndex = ref(null);

async function loadDsOptions() {
  try {
    const res = await listDataService({ pageNum: 1, pageSize: 999, status: 1 });
    dsOptions.value = res.rows || [];
  } catch {
    dsOptions.value = [];
  }
}

onMounted(() => {
  loadDsOptions();
  nextTick(initSortable);
});

async function onDsChange(dsId) {
  if (!dsId) {
    dsForm.value.params = [];
    return;
  }
  try {
    const res = await getDataService(dsId);
    const ds = res.data || {};
    dsForm.value.serviceName = ds.serviceName || "";
    dsForm.value.serviceCode = ds.serviceCode || "";
    // 解析 paramConfig，生成参数输入项
    let paramConfig = [];
    if (ds.paramConfig) {
      paramConfig =
        typeof ds.paramConfig === "string"
          ? JSON.parse(ds.paramConfig)
          : ds.paramConfig;
    }
    dsForm.value.params = (paramConfig || []).map((p) => ({
      key: p.key,
      type: p.type || "string",
      value: "",
    }));
  } catch {
    dsForm.value.params = [];
  }
}

function openDsDialog(index, row) {
  dsEditIndex.value = index;
  dsForm.value = {
    dsId: row.handleId,
    serviceName: row.handleName || "",
    serviceCode: row.serviceCode || "",
    params: Object.entries(row.params || {}).map(([key, value]) => ({
      key,
      type: typeof value === "number" ? "number" : typeof value === "boolean" ? "boolean" : "string",
      value: String(value ?? ""),
    })),
  };
  // 确保选项已加载
  if (!dsOptions.value.length) loadDsOptions();
  // 如果有 dsId，重新拉取参数定义以获取 type 信息
  if (dsForm.value.dsId) {
    onDsChange(dsForm.value.dsId).then(() => {
      // 回填已有值
      if (row.params) {
        dsForm.value.params.forEach((p) => {
          if (row.params[p.key] !== undefined) p.value = String(row.params[p.key]);
        });
      }
    });
  }
  dsDialogVisible.value = true;
}

function confirmDs() {
  if (!dsForm.value.dsId) return;
  // 组装参数对象
  const paramObj = {};
  dsForm.value.params.forEach((p) => {
    if (p.value !== "" && p.value != null) {
      paramObj[p.key] = p.type === "number" ? Number(p.value) : p.value;
    }
  });
  const newRow = {
    type: "dataservice",
    handleId: dsForm.value.dsId,
    handleName: dsForm.value.serviceName,
    serviceCode: dsForm.value.serviceCode,
    params: paramObj,
  };
  const newValue = [...props.value];
  if (dsEditIndex.value !== null) {
    newValue[dsEditIndex.value] = newRow;
  } else {
    newValue.push(newRow);
  }
  emit("updateValue", newValue);
  dsDialogVisible.value = false;
}

// ==================== 通用 ====================
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
    case "dataservice":
      return "fa fa-database";
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
    case "dept":
    case "user":
      return typeLabel + "：" + row.handleName;
    case "dataservice":
      return typeLabel + "：" + row.handleName;
    default:
      return "";
  }
};

const handleSlected = (e) => {
  if (e === "dataservice") {
    dsEditIndex.value = null;
    dsForm.value = { dsId: null, serviceName: "", serviceCode: "", params: [] };
    if (!dsOptions.value.length) loadDsOptions();
    dsDialogVisible.value = true;
    return;
  }
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

const getItemKey = (row, index) => {
  if (row.type === "author") return "author";
  if (row.type === "leader") return "leader";
  return `${row.type}_${row.handleId || index}`;
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
    cursor: grab;

    &.is-disabled {
      cursor: not-allowed;
    }

    &:active {
      cursor: grabbing;
    }
  }

  &--ghost {
    opacity: 0.4;
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

  .edit-icon {
    width: 24px;
    color: var(--text-placeholder);
    font-size: 12px;
    text-align: center;
    cursor: pointer;
    transition: color 0.2s;

    &:hover {
      color: var(--theme-color);
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
