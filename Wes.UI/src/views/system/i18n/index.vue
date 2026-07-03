<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form
        :config="queryConfig"
        v-model:visible="showSearch"
        v-model="queryParams.params"
        @query="handleQuery"
        @reset="resetQuery"
      />

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button
            type="primary"
            plain
            icon="Plus"
            @click="handleAdd"
            v-hasPermi="['system:i18n:add']"
            >{{ t('common.add') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate"
            v-hasPermi="['system:i18n:edit']"
            >{{ t('common.edit') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
            v-hasPermi="['system:i18n:remove']"
            >{{ t('common.delete') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="warning"
            plain
            icon="Refresh"
            @click="handleRefreshCache"
            v-hasPermi="['system:i18n:refresh']"
            >{{ t('i18n.refreshCache') }}</el-button
          >
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table
        v-loading="loading"
        :data="tableList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('i18n.i18nKey')" prop="i18nKey" min-width="200" :show-overflow-tooltip="true" />
        <el-table-column :label="t('i18n.lang')" prop="lang" width="120" align="center" />
        <el-table-column :label="t('i18n.i18nValue')" prop="i18nValue" min-width="250" :show-overflow-tooltip="true" />
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" width="180">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="t('common.actions')"
          align="center"
          width="220"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              @click="handleUpdate(scope.row)"
              v-hasPermi="['system:i18n:edit']"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              link
              type="danger"
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['system:i18n:remove']"
            >
              {{ t('common.delete') }}
            </el-button>
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
    </div>

    <!-- 新增/编辑对话框 -->
    <el-dialog :title="dialogTitle" v-model="dialogOpen" width="550px" append-to-body>
      <el-form ref="formRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item :label="t('i18n.i18nKey')" prop="i18nKey">
          <el-input v-model="form.i18nKey" :placeholder="t('i18n.placeholder.i18nKey')" />
        </el-form-item>
        <el-form-item :label="t('i18n.lang')" prop="lang">
          <el-select v-model="form.lang" :placeholder="t('i18n.placeholder.lang')">
            <el-option
              v-for="item in langOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('i18n.i18nValue')" prop="i18nValue">
          <el-input v-model="form.i18nValue" type="textarea" :rows="3" :placeholder="t('i18n.placeholder.i18nValue')" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button type="primary" @click="submitForm">{{ t('common.confirm') }}</el-button>
        <el-button @click="dialogOpen = false">{{ t('common.cancel') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="I18n">
import { ref, reactive, computed, nextTick } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import QueryForm from "@/components/QueryForm/index.vue";
import { listI18n, getI18n, addI18n, updateI18n, delI18n, refreshI18nCache } from "@/api/system/i18n";

const { t } = useI18n();
const store = useStore();

// 搜索
const showSearch = ref(true);
const loading = ref(false);
const total = ref(0);
const tableList = ref([]);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);

// 查询参数
const queryParams = ref({
  pageNum: 1,
  pageSize: 10,
  params: {
    i18nKey: undefined,
    lang: undefined,
    i18nValue: undefined,
  },
});

// 查询表单配置
const queryConfig = computed(() => [
  { label: t('i18n.i18nKey'), prop: "i18nKey", type: "input", placeholder: t('i18n.placeholder.i18nKey') },
  { label: t('i18n.lang'), prop: "lang", type: "select", placeholder: t('i18n.placeholder.lang'), options: langOptions },
  { label: t('i18n.i18nValue'), prop: "i18nValue", type: "input", placeholder: t('i18n.placeholder.i18nValue') },
]);

// 语言选项：从 store 的 langList 获取
const langOptions = computed(() =>
  store.getters["system/langList"].map((item) => ({
    value: item.langCode,
    label: item.langName,
  }))
);

// 对话框
const dialogOpen = ref(false);
const dialogTitle = ref("");
const formRef = ref(null);
const form = ref({
  i18nId: undefined,
  i18nKey: "",
  lang: "zh-CN",
  i18nValue: "",
});

const rules = {
  i18nKey: [{ required: true, message: computed(() => t('i18n.rules.i18nKeyRequired')), trigger: "blur" }],
  lang: [{ required: true, message: computed(() => t('i18n.rules.langRequired')), trigger: "change" }],
  i18nValue: [{ required: true, message: computed(() => t('i18n.rules.i18nValueRequired')), trigger: "blur" }],
};

/** 获取列表 */
function getList() {
  loading.value = true;
  listI18n(queryParams.value).then((res) => {
    tableList.value = res.rows;
    total.value = res.total;
    loading.value = false;
  });
}

/** 搜索 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置 */
function resetQuery() {
  queryParams.value.params.i18nKey = undefined;
  queryParams.value.params.lang = undefined;
  queryParams.value.params.i18nValue = undefined;
  handleQuery();
}

/** 选择 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.i18nId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 新增 */
function handleAdd() {
  form.value = { i18nId: undefined, i18nKey: "", lang: "zh-CN", i18nValue: "" };
  dialogTitle.value = t('i18n.addTitle');
  dialogOpen.value = true;
  nextTick(() => formRef.value?.resetFields());
}

/** 修改 */
function handleUpdate(row) {
  const id = row ? row.i18nId : ids.value[0];
  getI18n(id).then((res) => {
    form.value = res.data;
    dialogTitle.value = t('i18n.editTitle');
    dialogOpen.value = true;
  });
}

/** 删除 */
function handleDelete(row) {
  const i18nIds = row ? [row.i18nId] : ids.value;
  ElMessageBox.confirm(
    t('i18n.confirm.delete', { ids: i18nIds.join("，") }),
    t('common.confirmTitle'),
    {
      confirmButtonText: t('common.confirmDelete'),
      cancelButtonText: t('common.cancel'),
      confirmButtonType: "danger",
      type: "warning",
    }
  )
    .then(() => delI18n(i18nIds.join(",")))
    .then(() => {
      ElMessage.success(t('common.deleteSuccess'));
      getList();
    })
    .catch(() => {});
}

/** 提交 */
function submitForm() {
  formRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = form.value.i18nId ? updateI18n(form.value) : addI18n(form.value);
      apiCall.then(() => {
        ElMessage.success(form.value.i18nId ? t('common.editSuccess') : t('common.addSuccess'));
        dialogOpen.value = false;
        getList();
      });
    }
  });
}

/** 刷新缓存 */
function handleRefreshCache() {
  refreshI18nCache().then(() => {
    ElMessage.success(t('i18n.message.cacheRefreshed'));
  });
}

// 初始化
getList();
</script>
