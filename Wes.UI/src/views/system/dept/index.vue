<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form
        :config="queryConfig"
        v-model:visible="showSearch"
        v-model="queryParams"
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
            v-hasPermi="['system:dept:add']"
            >{{ t('common.add') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button type="info" plain icon="Sort" @click="toggleExpandAll"
            >{{ t('common.expand') }}</el-button
          >
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table
        v-if="refreshTable"
        v-loading="loading"
        :data="deptList"
        row-key="deptId"
        :default-expand-all="isExpandAll"
        :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
      >
        <el-table-column prop="deptName" :label="t('deptManage.deptName')"></el-table-column>
        <el-table-column prop="orderNum" :label="t('common.sort')" width="200"></el-table-column>
        <el-table-column prop="status" :label="t('common.status')" width="100">
          <template #default="scope">
            <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
          </template>
        </el-table-column>
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" width="220">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="t('common.actions')"
          align="center"
          width="260"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              @click="handleUpdate(scope.row)"
              v-hasPermi="['system:dept:edit']"
              >{{ t('common.edit') }}</el-button
            >
            <el-button
              link
              type="primary"
              icon="Plus"
              @click="handleAdd(scope.row)"
              v-hasPermi="['system:dept:add']"
              >{{ t('common.add') }}</el-button
            >
            <el-button
              v-if="scope.row.parentId != 0"
              link
              type="primary"
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['system:dept:remove']"
              >{{ t('common.delete') }}</el-button
            >
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- 添加或修改部门对话框 -->
    <el-dialog :title="title" v-model="open" width="600px" append-to-body>
      <el-form ref="deptRef" :model="form" :rules="rules" label-width="auto">
        <el-row>
          <el-col :span="24" v-if="form.parentId !== 0">
            <el-form-item :label="t('deptManage.parentDept')" prop="parentId">
              <el-tree-select
                v-model="form.parentId"
                :data="deptOptions"
                :props="{ value: 'deptId', label: 'deptName', children: 'children' }"
                value-key="deptId"
                :placeholder="t('deptManage.placeholder.parentDept')"
                check-strictly
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('deptManage.deptName')" prop="deptName">
              <el-input v-model="form.deptName" :placeholder="t('deptManage.placeholder.deptName')" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('deptManage.displayOrder')" prop="orderNum">
              <el-input-number
                v-model="form.orderNum"
                controls-position="right"
                :min="0"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('deptManage.leader')" prop="leader">
              <el-input v-model="form.leader" :placeholder="t('deptManage.placeholder.leader')" maxlength="20" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('deptManage.phone')" prop="phone">
              <el-input
                v-model="form.phone"
                :placeholder="t('deptManage.placeholder.phone')"
                maxlength="11"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('deptManage.email')" prop="email">
              <el-input v-model="form.email" :placeholder="t('deptManage.placeholder.email')" maxlength="50" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('deptManage.deptStatus')">
              <el-radio-group v-model="form.status">
                <el-radio
                  v-for="dict in sys_normal_disable"
                  :key="dict.value"
                  :value="dict.value"
                  >{{ dict.label }}</el-radio
                >
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
          <el-button @click="cancel">{{ t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="Dept">
import { ref, reactive, toRefs, nextTick, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import { getDict, handleTree } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import {
  listDept,
  getDept,
  delDept,
  addDept,
  updateDept,
  listDeptExcludeChild,
} from "@/api/system/dept";

const { t } = useI18n();
const { sys_normal_disable } = getDict("sys_normal_disable");

const deptList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const title = ref("");
const deptOptions = ref([]);
const isExpandAll = ref(true);
const refreshTable = ref(true);
const queryRef = ref(null);
const deptRef = ref(null);

// 查询条件配置
const queryConfig = computed(() => [
  {
    label: t('deptManage.deptName'),
    prop: "deptName",
    type: "input",
    placeholder: t('deptManage.placeholder.deptName'),
  },
  {
    label: t('common.status'),
    prop: "status",
    type: "select",
    placeholder: t('deptManage.placeholder.status'),
    options: sys_normal_disable,
  },
]);

const data = reactive({
  form: {},
  queryParams: {
    deptName: undefined,
    status: undefined,
  },
  rules: {
    parentId: [{ required: true, message: computed(() => t('deptManage.rules.parentRequired')), trigger: "blur" }],
    deptName: [{ required: true, message: computed(() => t('deptManage.rules.deptNameRequired')), trigger: "blur" }],
    orderNum: [{ required: true, message: computed(() => t('deptManage.rules.orderNumRequired')), trigger: "blur" }],
    email: [
      { type: "email", message: computed(() => t('deptManage.rules.emailInvalid')), trigger: ["blur", "change"] },
    ],
    phone: [
      {
        pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
        message: computed(() => t('deptManage.rules.phoneInvalid')),
        trigger: "blur",
      },
    ],
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 查询部门列表 */
function getList() {
  loading.value = true;
  listDept(queryParams.value).then((response) => {
    deptList.value = handleTree(response.data, "deptId");
    loading.value = false;
  });
}

/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

/** 表单重置 */
function reset() {
  form.value = {
    deptId: undefined,
    parentId: undefined,
    deptName: undefined,
    orderNum: 0,
    leader: undefined,
    phone: undefined,
    email: undefined,
    status: "0",
  };
  if (deptRef.value) deptRef.value.resetFields();
}

/** 搜索按钮操作 */
function handleQuery() {
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  if (queryRef.value) queryRef.value.resetFields();
  handleQuery();
}

/** 新增按钮操作 */
function handleAdd(row) {
  reset();
  listDept().then((response) => {
    deptOptions.value = handleTree(response.data, "deptId");
  });
  if (row != undefined) {
    form.value.parentId = row.deptId;
  }
  open.value = true;
  title.value = t('deptManage.addTitle');
}

/** 展开/折叠操作 */
function toggleExpandAll() {
  refreshTable.value = false;
  isExpandAll.value = !isExpandAll.value;
  nextTick(() => {
    refreshTable.value = true;
  });
}

/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  listDeptExcludeChild(row.deptId).then((response) => {
    deptOptions.value = handleTree(response.data, "deptId");
  });
  getDept(row.deptId).then((response) => {
    form.value = response.data;
    open.value = true;
    title.value = t('deptManage.editTitle');
  });
}

/** 提交按钮 */
function submitForm() {
  deptRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = form.value.deptId ? updateDept(form.value) : addDept(form.value);
      apiCall.then(() => {
        ElMessage.success(form.value.deptId ? t('common.editSuccess') : t('common.addSuccess'));
        open.value = false;
        getList();
      });
    }
  });
}

/** 删除按钮操作 */
function handleDelete(row) {
  ElMessageBox.confirm(t('deptManage.confirm.delete', { name: row.deptName }), t('common.confirmTitle'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delDept(row.deptId))
    .then(() => {
      getList();
      ElMessage.success(t('common.deleteSuccess'));
    })
    .catch(() => {});
}

getList();
</script>
