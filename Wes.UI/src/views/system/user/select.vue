<template>
  <el-dialog :title="t('user.selectUser')" v-model="isOpen" width="998px" append-to-body>
    <query-form
      :config="queryConfig"
      v-model="queryParams.params"
      @query="handleQuery"
      @reset="resetQuery"
    />

    <el-row :gutter="10" class="mb8" style="margin-top: 10px">
      <span>{{ t('common.selectedLabel') }}</span>
      <el-tag
        v-for="(tag, index) in checked"
        :key="index"
        closable
        style="margin-right: 6px"
        @close="handleCloseTag(index)"
      >
        {{ tag.userName }}
      </el-tag>
    </el-row>

    <el-table v-loading="loading" :data="dataList" @row-click="handleRowClick">
      <el-table-column width="55" align="center">
        <template #default="scope">
          <el-check-tag :checked="isChecked(scope.row)" @change="handleToggle(scope.row)">
            {{ isChecked(scope.row) ? t('user.selected') : t('user.select') }}
          </el-check-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('user.userId')" align="center" prop="userId" />
      <el-table-column
        :label="t('user.account')"
        align="center"
        prop="userName"
        :show-overflow-tooltip="true"
      />
      <el-table-column
        :label="t('user.nickName')"
        align="center"
        prop="nickName"
        :show-overflow-tooltip="true"
      />
      <el-table-column
        :label="t('user.dept')"
        align="center"
        prop="dept.deptName"
        :show-overflow-tooltip="true"
      />
      <el-table-column :label="t('user.phone')" align="center" prop="phonenumber" width="120" />
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
import { ref, reactive, computed, defineEmits, defineExpose, defineProps } from "vue";
import { useI18n } from 'vue-i18n'
import { ElMessage } from "element-plus";
import { listUser } from "@/api/system/user";
import { getDict } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";

const { t } = useI18n()
const { sys_normal_disable } = getDict("sys_normal_disable");

const loading = ref(true);
const checked = ref([]);
const total = ref(0);
const dataList = ref([]);
const isOpen = ref(false);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 8,
  params: {},
});

const queryConfig = computed(() => [
  { label: t("user.account"), prop: "userName", type: "input", placeholder: t("user.placeholder.searchAccount") },
  { label: t("user.nickName"), prop: "nickName", type: "input", placeholder: t("user.placeholder.searchName") },
  {
    label: t("user.phone"),
    prop: "phonenumber",
    type: "input",
    placeholder: t("user.placeholder.phoneSearch"),
  },
]);

const emit = defineEmits(["handleData"]);

function open() {
  checked.value = [];
  isOpen.value = true;
  getList();
}

function getList() {
  loading.value = true;
  listUser(queryParams).then((response) => {
    dataList.value = response.rows;
    total.value = response.total;
    loading.value = false;
  });
}

function handleQuery() {
  queryParams.pageNum = 1;
  getList();
}

function resetQuery() {
  queryParams.params = {};
  queryParams.pageNum = 1;
  getList();
}

function isChecked(row) {
  return checked.value.some((p) => p.userId == row.userId);
}

function handleToggle(row) {
  if (isChecked(row)) {
    checked.value = checked.value.filter((p) => p.userId != row.userId);
  } else {
    if (checked.value.length && isSingle) {
      checked.value = [];
    }
    checked.value.push(row);
  }
}

function handleRowClick(row) {
  handleToggle(row);
}

function handleCloseTag(index) {
  checked.value.splice(index, 1);
}

function submitForm() {
  if (checked.value.length == 0) {
    ElMessage.error(t("user.pleaseSelectUser"));
    return false;
  }
  isOpen.value = false;
  emit("handleData", checked.value);
}

defineExpose({ open });
</script>

<style lang="scss" scoped>
::deep(.el-dialog__body) {
  padding-top: 0;
}
</style>
