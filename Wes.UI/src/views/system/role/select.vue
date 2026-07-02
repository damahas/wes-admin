<template>
  <el-dialog :title="t('role.selectRole')" v-model="isOpen" width="800px" append-to-body>
    <el-form
      :model="queryParams"
      ref="queryFormRef"
      size="small"
      :inline="true"
      label-width="68px"
    >
      <el-form-item :label="t('role.roleName')" prop="roleName">
        <el-input
          v-model="queryParams.params.roleName"
          :placeholder="t('role.placeholder.roleName')"
          clearable
          style="width: 160px"
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item :label="t('role.roleCode')" prop="roleKey">
        <el-input
          v-model="queryParams.params.roleKey"
          :placeholder="t('role.placeholder.roleCode')"
          clearable
          style="width: 160px"
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="el-icon-search" size="small" @click="handleQuery">
          {{ t('common.search') }}
        </el-button>
        <el-button icon="el-icon-refresh" size="small" @click="resetQuery">
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
        @close="checked.splice(index, 1)"
      >
        {{ tag.roleName }}
      </el-tag>
    </el-row>

    <el-table v-loading="loading" :data="dataList" @row-click="handleSelectionChange">
      <el-table-column width="55" align="center">
        <template #default="scope">
          <input type="checkbox" :value="scope.row" v-model="checked" />
        </template>
      </el-table-column>
      <el-table-column
        :label="t('role.roleCode')"
        prop="roleKey"
        :show-overflow-tooltip="true"
        width="150"
      />
      <el-table-column
        :label="t('role.roleName')"
        prop="roleName"
        :show-overflow-tooltip="true"
        width="150"
      />
      <el-table-column :label="t('common.status')" align="center" prop="status">
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

    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="getList"
    />

    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
        <el-button @click="isOpen = false">{{ t('common.cancel') }}</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, reactive, onMounted } from "vue";
import { useI18n } from 'vue-i18n'
import { listRole } from "@/api/system/role";
import { ElMessage } from "element-plus";
import { getDict } from "@/utils";

const { t } = useI18n()

defineOptions({
  name: "RoleSelect",
});

const props = defineProps({
  isSingle: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["handleData"]);

const { sys_normal_disable } = getDict("sys_normal_disable");

// 遮罩层
const loading = ref(true);
// 选中数组
const checked = ref([]);
// 总条数
const total = ref(0);
// 表格数据
const dataList = ref([]);
// 是否显示
const isOpen = ref(false);
// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 8,
  params: {},
});

const queryFormRef = ref();

onMounted(() => {
  getList();
});

function open() {
  checked.value = [];
  isOpen.value = true;
  getList();
}

/** 查询角色列表 */
function getList() {
  loading.value = true;
  listRole(queryParams).then((response) => {
    dataList.value = response.rows;
    total.value = response.total;
    loading.value = false;
  });
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.pageNum = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  queryFormRef.value?.resetFields();
  handleQuery();
}

/** 多选框选中数据 */
function handleSelectionChange(row) {
  if (checked.value.find((p) => p.roleId == row.roleId)) {
    checked.value = checked.value.filter((p) => p.roleId != row.roleId);
    return false;
  }
  if (props.isSingle && checked.value.length) {
    checked.value = [];
  }
  checked.value.push(row);
}

/** 提交 */
function submitForm() {
  if (checked.value.length == 0) {
    ElMessage.error(t("role.pleaseSelectRole"));
    return false;
  }
  isOpen.value = false;
  emit("handleData", checked.value);
}

defineExpose({
  open,
});
</script>

<style lang="scss" scoped>
::deep(.el-dialog__body) {
  padding-top: 0;
}
</style>
