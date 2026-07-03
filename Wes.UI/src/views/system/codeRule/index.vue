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
            v-hasPermi="['system:coderule:add']"
          >
            {{ t('common.add') }}
          </el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate"
            v-hasPermi="['system:coderule:edit']"
          >
            {{ t('common.edit') }}
          </el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
            v-hasPermi="['system:coderule:remove']"
          >
            {{ t('common.delete') }}
          </el-button>
        </el-col>
        <right-toolbar :showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table
        v-loading="loading"
        :data="dataList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('codeRule.ruleCode')" align="center" prop="ruleCode" />
        <el-table-column :label="t('codeRule.ruleName')" align="center" prop="ruleName" />
        <el-table-column :label="t('codeRule.ruleType')" align="center" prop="ruleType">
          <template #default="scope">
            <dict-tag :options="sys_code_type" :value="scope.row.ruleType" />
          </template>
        </el-table-column>
        <el-table-column :label="t('common.status')" align="center" prop="status">
          <template #default="scope">
            <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
          </template>
        </el-table-column>
        <el-table-column
          :label="t('codeRule.lastUpdateTime')"
          align="center"
          prop="updateTime"
          width="180"
        >
          <template #default="scope">
            <span>{{ formatTime(scope.row.updateTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="t('common.actions')"
          align="center"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              type="primary"
              link
              icon="Edit"
              @click="handleUpdate(scope.row)"
              v-hasPermi="['system:coderule:edit']"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              type="danger"
              link
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['system:coderule:remove']"
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

    <!-- 添加或修改数据对话框 -->
    <el-dialog :title="title" v-model="open" width="50%" append-to-body>
      <el-form ref="formRef" :model="form" :rules="rules" label-width="auto">
        <el-row>
          <el-col :span="11">
            <el-form-item :label="t('codeRule.ruleCode')" prop="ruleCode">
              <el-input v-model="form.ruleCode" :placeholder="t('codeRule.placeholder.ruleCode')" />
            </el-form-item>
          </el-col>
          <el-col :span="11" :offset="1">
            <el-form-item :label="t('codeRule.ruleName')" prop="ruleName">
              <el-input v-model="form.ruleName" :placeholder="t('codeRule.placeholder.ruleName')" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="11">
            <el-form-item :label="t('codeRule.ruleType')" prop="ruleType">
              <el-select
                v-model="form.ruleType"
                :placeholder="t('codeRule.ruleType')"
                style="width: 100%"
                filterable
                clearable
              >
                <el-option
                  v-for="dict in sys_code_type"
                  :key="dict.value"
                  :label="dict.label"
                  :value="dict.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="11" :offset="1">
            <el-form-item :label="t('common.status')" prop="status">
              <el-radio-group v-model="form.status">
                <el-radio
                  v-for="dict in sys_normal_disable"
                  :key="dict.value"
                  :value="dict.value"
                >
                  {{ dict.label }}
                </el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <el-alert
        :title="t('codeRule.dragTip')"
        type="info"
        :closable="false"
        style="margin-bottom: 9px"
      >
      </el-alert>
      <el-button
        link
        type="primary"
        style="margin-bottom: 9px"
        plain
        icon="Plus"
        @click="handleEditPart(-1)"
      >
        {{ t('codeRule.addPart') }}
      </el-button>
      <el-table
        ref="partTable"
        row-key="partId"
        class="dragTable"
        :data="form.parts"
        size="small"
      >
        <el-table-column
          :label="t('codeRule.seqNo')"
          type="index"
          min-width="50"
          class-name="allowDrag"
        />
        <el-table-column :label="t('codeRule.partValue')" align="center" prop="partValue" />
        <el-table-column :label="t('codeRule.partType')" align="center" prop="partType">
          <template #default="scope">
            {{ partTypes.value[scope.row.partType] }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.remark')" align="center" prop="remark" />
        <el-table-column
          :label="t('common.actions')"
          align="center"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              @click="handleEditPart(scope.$index)"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              link
              type="danger"
              icon="Delete"
              @click="handleDelPart(scope.$index)"
            >
              {{ t('common.delete') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
          <el-button @click="cancel">{{ t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>

    <el-dialog :title="partTitle" v-model="partOpen" width="520px" append-to-body>
      <el-form ref="partFormRef" :model="partForm" :rules="partRules" label-width="auto">
        <el-form-item :label="t('codeRule.partType')" prop="partType">
          <el-select
            v-model="partForm.partType"
            :placeholder="t('codeRule.partType')"
            style="width: 100%"
          >
            <el-option
              v-for="item in partTypeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <div v-if="partForm.partType === 'string'">
          <el-form-item :label="t('codeRule.partTypes.string')" prop="partValue">
            <el-input v-model="partForm.partValue" :placeholder="t('codeRule.placeholder.fixedChar')" />
          </el-form-item>
        </div>
        <div v-if="partForm.partType === 'calc'">
          <el-form-item :label="t('codeRule.dynamicRule')" prop="partValue">
            <el-input v-model="partForm.partValue" :placeholder="t('codeRule.placeholder.dynamicRule')" />
          </el-form-item>
          <el-form-item :label="t('codeRule.resetType')" prop="resetType">
            <el-select
              v-model="partForm.resetType"
              :placeholder="t('codeRule.resetType')"
              style="width: 100%"
            >
              <el-option
                v-for="item in resetTypeOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </el-form-item>
          <el-form-item
            :label="t('codeRule.weekStartDay')"
            prop="weekStartDay"
            v-if="partForm.resetType === 'week'"
          >
            <el-select
              v-model="partForm.weekStartDay"
              :placeholder="t('codeRule.weekStartDay')"
              style="width: 100%"
            >
              <el-option :label="t('codeRule.weekDays.sunday')" :value="0" />
              <el-option :label="t('codeRule.weekDays.monday')" :value="1" />
              <el-option :label="t('codeRule.weekDays.tuesday')" :value="2" />
              <el-option :label="t('codeRule.weekDays.wednesday')" :value="3" />
              <el-option :label="t('codeRule.weekDays.thursday')" :value="4" />
              <el-option :label="t('codeRule.weekDays.friday')" :value="5" />
              <el-option :label="t('codeRule.weekDays.saturday')" :value="6" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('codeRule.isSkipZero')">
            <el-checkbox v-model="partForm.isSkipZero" :true-value="1" :false-value="0" />
          </el-form-item>
        </div>
        <div v-if="partForm.partType === 'date'">
          <el-form-item :label="t('codeRule.dynamicRule')" prop="partValue">
            <el-input v-model="partForm.partValue" :placeholder="t('codeRule.placeholder.dateFormat')" />
          </el-form-item>
        </div>
        <el-form-item :label="t('common.remark')" prop="remark">
          <el-input
            type="textarea"
            row="3"
            v-model="partForm.remark"
            :placeholder="t('codeRule.placeholder.description')"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="handleSavePart">{{ t('common.submit') }}</el-button>
          <el-button @click="partOpen = false">{{ t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="Code">
import { ref, reactive, toRefs, nextTick, computed, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { getDict } from "@/utils";
import { useI18n } from "vue-i18n";
import QueryForm from "@/components/QueryForm/index.vue";
import {
  listSysCodeRule,
  getSysCodeRule,
  delSysCodeRule,
  addSysCodeRule,
  updateSysCodeRule,
} from "@/api/system/codeRule";
import Sortable from "sortablejs";

const { t } = useI18n();

const { sys_code_type, sys_normal_disable } = getDict(
  "sys_code_type",
  "sys_normal_disable"
);

const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const showSearch = ref(true);
const total = ref(0);
const dataList = ref([]);
const title = ref("");
const open = ref(false);
const queryFormRef = ref(null);
const formRef = ref(null);
const partFormRef = ref(null);

// 查询条件配置
const queryConfig = computed(() => [
  {
    label: t('codeRule.ruleCode'),
    prop: "ruleCode",
    type: "input",
    placeholder: t('codeRule.placeholder.ruleCode'),
  },
  {
    label: t('codeRule.ruleName'),
    prop: "ruleName",
    type: "input",
    placeholder: t('codeRule.placeholder.ruleName'),
  },
  {
    label: t('codeRule.ruleType'),
    prop: "ruleType",
    type: "select",
    placeholder: t('codeRule.ruleType'),
    options: sys_code_type,
  },
  {
    label: t('common.status'),
    prop: "status",
    type: "select",
    placeholder: t('common.status'),
    options: sys_normal_disable,
  },
]);

const data = reactive({
  form: {
    parts: [],
  },
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {},
  },
  rules: {
    ruleCode: [{ required: true, message: computed(() => t('codeRule.rules.ruleCodeRequired')), trigger: "blur" }],
    ruleName: [{ required: true, message: computed(() => t('codeRule.rules.ruleNameRequired')), trigger: "blur" }],
  },
  partForm: {},
  partRules: {
    partType: [{ required: true, message: computed(() => t('codeRule.rules.partTypeRequired')), trigger: "change" }],
    partValue: [{ required: true, message: computed(() => t('codeRule.rules.charRuleRequired')), trigger: "blur" }],
    resetType: [{ required: true, message: computed(() => t('codeRule.rules.resetTypeRequired')), trigger: "change" }],
    weekStartDay: [{ required: true, message: computed(() => t('codeRule.rules.weekStartDayRequired')), trigger: "change" }],
  },
});
let { queryParams, form, rules, partForm, partRules } = toRefs(data);
const partTitle = ref("");
const partOpen = ref(false);
const partIndex = ref(-1);
const partTypes = computed(() => ({
  string: t('codeRule.partTypes.string'),
  calc: t('codeRule.partTypes.calc'),
  date: t('codeRule.partTypes.date'),
}));
const resetTypes = computed(() => ({
  week: t('codeRule.resetTypes.week'),
  month: t('codeRule.resetTypes.month'),
  quarter: t('codeRule.resetTypes.quarter'),
  year: t('codeRule.resetTypes.year'),
}));
const sortable = ref(undefined);
const originPartValue = ref("");

const partTypeOptions = computed(() => {
  return Object.keys(partTypes).map((p) => {
    return {
      value: p,
      label: partTypes[p],
    };
  });
});
const resetTypeOptions = computed(() => {
  return Object.keys(resetTypes).map((p) => {
    return {
      value: p,
      label: resetTypes[p],
    };
  });
});

onMounted(() => {
  getList();
});

/** 查询列表 */
function getList() {
  loading.value = true;
  listSysCodeRule(queryParams.value).then((response) => {
    dataList.value = response.rows;
    total.value = response.total;
    loading.value = false;
  });
}
// 取消按钮
function cancel() {
  open.value = false;
  reset();
}
// 表单重置
function reset() {
  form.value = {
    parts: [],
  };
  if (formRef.value) formRef.value.resetFields();
}
/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}
/** 重置按钮操作 */
function resetQuery() {
  if (queryFormRef.value) queryFormRef.value.resetFields();
  handleQuery();
}
// 多选框选中数据
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.ruleId);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}
/** 新增按钮操作 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = t('codeRule.addTitle');
  nextTick(() => {
    initSort();
  });
}
/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  const ruleId = row.ruleId || ids.value;
  getSysCodeRule(ruleId).then((response) => {
    form.value = response.data;
    open.value = true;
    title.value = t('codeRule.editTitle');
    nextTick(() => {
      initSort();
    });
  });
}
/** 提交按钮 */
function submitForm() {
  formRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = form.value.ruleId
        ? updateSysCodeRule(form.value)
        : addSysCodeRule(form.value);
      apiCall.then(() => {
        ElMessage.success(form.value.ruleId ? t('common.editSuccess') : t('common.addSuccess'));
        open.value = false;
        getList();
      });
    }
  });
}
/** 删除按钮操作 */
function handleDelete(row) {
  const ruleIds = row.ruleId || ids.value;
  ElMessageBox.confirm(t('codeRule.confirm.delete', { name: ruleIds }), t('common.confirmTitle'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delSysCodeRule(ruleIds))
    .then(() => {
      getList();
      ElMessage.success(t('common.deleteSuccess'));
    })
    .catch(() => {});
}
/** 片段操作 */
function handleEditPart(index) {
  partIndex.value = index;
  if (index > -1) {
    partForm.value = { ...form.value.parts[index] };
    originPartValue.value = partForm.value.partValue ?? "";
    partTitle.value = t('codeRule.editPart');
  } else {
    partForm.value = {
      weekStartDay: 1,
      isSkipZero: 0,
    };
    originPartValue.value = "";
    partTitle.value = t('codeRule.addPart');
  }
  if (partFormRef.value) partFormRef.value.resetFields();
  partOpen.value = true;
}
function handleSavePart() {
  partFormRef.value?.validate((valid) => {
    if (valid) {
      const doSave = () => {
        if (partIndex.value > -1) {
          form.value.parts[partIndex.value] = { ...partForm.value };
        } else {
          form.value.parts.push({
            ...partForm.value,
            partId: -1 - form.value.parts.length,
            sort: form.value.parts.length + 1,
          });
        }
        partOpen.value = false;
      };

      if (
        partForm.value.partType === "calc" &&
        partForm.value.partValue !== originPartValue.value
      ) {
        ElMessageBox.confirm(
          t('codeRule.ruleChangeConfirm'),
          t('codeRule.resetConfirmTitle'),
          {
            confirmButtonText: t('common.confirm'),
            cancelButtonText: t('common.cancel'),
            type: "warning",
          }
        )
          .then(() => doSave())
          .catch(() => {});
      } else {
        doSave();
      }
    }
  });
}
function handleDelPart(index) {
  form.value.parts.splice(index, 1);
}

const initSort = () => {
  const table = document.querySelector(".dragTable .el-table__body-wrapper tbody");
  Sortable.create(table, {
    group: "shared",
    animation: 150,
    ghostClass: "blue-background-class",
    easing: "cubic-bezier(1, 0, 0, 1)",
    onStart: (item) => {
      console.log(item);
    },

    // 结束拖动事件
    onEnd: (item) => {
      const currentRow = form.value.parts.splice(item.oldIndex, 1)[0];
      form.value.parts.splice(item.newIndex, 0, currentRow);
      for (let index in form.value.parts) {
        form.value.parts[index].sort = parseInt(index) + 1;
      }
    },
  });
};
</script>

<style scoped>
.dragged-item img {
  opacity: 1 !important;
  border: 3px solid gray;
}

.dragged-item {
  opacity: 1 !important;
  z-index: 1070;
}

.dragged-item::after {
  content: "放到左边空白处";
}
</style>
