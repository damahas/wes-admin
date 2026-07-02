<template>
  <el-dialog :title="t('deptManage.selectDept')" v-model="isOpen" width="800px" append-to-body>
    <el-form
      :model="queryParams"
      ref="queryFormRef"
      size="small"
      :inline="true"
      label-width="68px"
      @submit.prevent
    >
      <el-form-item :label="t('deptManage.deptName')" prop="deptName">
        <el-input
          v-model="queryParams.deptName"
          :placeholder="t('deptManage.placeholder.deptName')"
          style="width: 160px"
          clearable
          @keyup.enter="getList"
        />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" size="small" @click="getList">
          {{ t('common.search') }}
        </el-button>
        <el-button icon="Refresh" size="small" @click="resetQuery">
          {{ t('common.reset') }}
        </el-button>
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <span>{{ t('common.selectedLabel') }}</span>
      <el-tag
        v-for="(tag, index) in checked"
        :key="index"
        closable
        style="margin-right: 6px"
        @close="handleCloseTag(index)"
      >
        {{ tag.deptName }}
      </el-tag>
    </el-row>
    <el-table
      v-loading="loading"
      :data="dataList"
      row-key="deptId"
      default-expand-all
      :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
      @row-click="handleSelectionChange"
    >
      <el-table-column prop="deptName" :label="t('deptManage.deptName')" width="260" />
      <el-table-column prop="orderNum" :label="t('common.sort')" width="200" />
      <el-table-column prop="status" :label="t('common.status')" width="100">
        <template #default="scope">
          <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.createTime')" align="center" width="160">
        <template #default="scope">
          <span>{{ formatTime(scope.row.createTime) }}</span>
        </template>
      </el-table-column>
    </el-table>

    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
        <el-button @click="isOpen = false">{{ t('common.cancel') }}</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, reactive, defineEmits, defineExpose, defineProps } from "vue";
import { useI18n } from 'vue-i18n'
import { listDept } from "@/api/system/dept";
import DictTag from "@/components/DictTag/index.vue";
import { getDict } from "@/utils";
import { ElMessage } from "element-plus";

const { t } = useI18n()

const { sys_normal_disable } = getDict("sys_normal_disable");

const props = defineProps({
  isSingle: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["handleData"]);

const queryFormRef = ref(null);
const loading = ref(true);
const checked = ref([]);
const dataList = ref([]);
const isOpen = ref(false);

const queryParams = reactive({
  deptName: undefined,
  status: undefined,
});

function open() {
  checked.value = [];
  isOpen.value = true;
  getList();
}

function getList() {
  loading.value = true;
  listDept(queryParams).then((response) => {
    dataList.value = handleTree(response.data, "deptId");
    loading.value = false;
  });
}

function resetQuery() {
  if (queryFormRef.value) {
    queryFormRef.value.resetFields();
  }
  getList();
}

function handleSelectionChange(row) {
  const index = checked.value.findIndex((p) => p.deptId == row.deptId);
  if (index > -1) {
    checked.value.splice(index, 1);
  } else {
    if (props.isSingle && checked.value.length) {
      checked.value = [];
    }
    checked.value.push(row);
  }
}

function handleCloseTag(index) {
  checked.value.splice(index, 1);
}

function submitForm() {
  if (checked.value.length == 0) {
    ElMessage.error(t("deptManage.pleaseSelectDept"));
    return;
  }
  isOpen.value = false;
  emit("handleData", checked.value);
}

// 树形结构处理函数
function handleTree(data, idKey = "id", parentKey = "parentId", childKey = "children") {
  if (!data || !Array.isArray(data)) return [];

  const tree = [];
  const map = {};

  data.forEach((item) => {
    map[item[idKey]] = { ...item, [childKey]: [] };
  });

  data.forEach((item) => {
    const node = map[item[idKey]];
    if (item[parentKey] && map[item[parentKey]]) {
      map[item[parentKey]][childKey].push(node);
    } else {
      tree.push(node);
    }
  });

  return tree;
}

defineExpose({ open });
</script>

<style lang="scss" scoped>
::deep(.el-dialog__body) {
  padding-top: 0;
}
</style>
