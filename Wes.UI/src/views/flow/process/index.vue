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
            v-hasPermi="['flow:version:edit']"
          >
            新增
          </el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate"
            v-hasPermi="['flow:version:edit']"
          >
            修改
          </el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
            v-hasPermi="['flow:process:list']"
          >
            删除
          </el-button>
        </el-col>
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" />
      </el-row>

      <el-table
        v-loading="loading"
        :data="dataList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="流程编码" width="140" align="center" prop="processCode" />
        <el-table-column label="流程名称" align="center" prop="processName" />
        <el-table-column label="业务模块" width="120" align="center" prop="businessField">
          <template #default="scope">
            <dict-tag :options="sys_flow_field" :value="scope.row.businessField" />
          </template>
        </el-table-column>
        <el-table-column label="当前流程图" align="center">
          <template #default="scope">
            <el-link type="primary" @click="handleToVersion(scope.row.version)">
              {{ scope.row.version?.version }}
            </el-link>
          </template>
        </el-table-column>
        <el-table-column label="描述" align="center" prop="remark" />
        <el-table-column
          label="操作"
          align="center"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              v-hasPermi="['flow:version:edit']"
              @click="handleUpdate(scope.row)"
            >
              修改
            </el-button>
            <el-button
              link
              type="primary"
              icon="Wallet"
              v-hasPermi="['flow:version:edit']"
              @click="handleToVersion(scope.row.version)"
            >
              编排
            </el-button>
            <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)">
              删除
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
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="formRef" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="流程编码" prop="processCode">
          <el-input v-model="form.processCode" placeholder="请输入流程编码" />
        </el-form-item>
        <el-form-item label="流程名称" prop="processName">
          <el-input v-model="form.processName" placeholder="请输入流程名称" />
        </el-form-item>
        <el-form-item label="业务模块" prop="businessField">
          <el-select v-model="form.businessField" placeholder="请选择业务模块">
            <el-option
              v-for="dict in sys_flow_field"
              :key="dict.value"
              :label="dict.label"
              :value="dict.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="页面地址" prop="formUrl">
          <el-input v-model="form.formUrl" placeholder="请输入页面地址" />
        </el-form-item>
        <el-form-item label="业务模块" prop="backUrl">
          <el-input v-model="form.backUrl" placeholder="请输入业务模块" />
        </el-form-item>
        <el-form-item label="描述" prop="remark">
          <el-input
            type="textarea"
            :rows="5"
            v-model="form.remark"
            placeholder="请输入描述"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 流程设计对话框 -->
    <FlowDesigner ref="flowRef" @close="getList"/>
  </div>
</template>

<script setup>
import { ref, reactive, toRefs, nextTick } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { getDict } from "@/utils";
import {
  listProcess,
  getProcess,
  delProcess,
  addProcess,
  updateProcess,
} from "@/api/flow/process";
import QueryForm from "@/components/QueryForm/index.vue";
import DictTag from "@/components/DictTag/index.vue";
import FlowDesigner from "./flow.vue";

const flowRef = ref(null);

const { sys_flow_field } = getDict("sys_flow_field");

const formRef = ref(null);

const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const showSearch = ref(true);
const total = ref(0);
const dataList = ref([]);
const title = ref("");
const open = ref(false);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  params: {},
});

const form = ref({});

const rules = reactive({
  processCode: [{ required: true, message: "流程编码不能为空", trigger: "blur" }],
  processName: [{ required: true, message: "流程名称不能为空", trigger: "blur" }],
  businessField: [{ required: true, message: "业务模块不能为空", trigger: "blur" }],
});

const queryConfig = [
  {
    label: "流程编码",
    prop: "processCode",
    type: "input",
    placeholder: "请输入流程编码",
  },
  {
    label: "流程名称",
    prop: "processName",
    type: "input",
    placeholder: "请输入流程名称",
  },
  {
    label: "业务模块",
    prop: "businessField",
    type: "select",
    placeholder: "请选择业务模块",
    options: sys_flow_field,
  },
];

/** 查询列表 */
function getList() {
  loading.value = true;
  listProcess(queryParams).then((response) => {
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
  queryParams.params = {};
  queryParams.pageNum = 1;
  getList();
}

/** 取消按钮 */
function cancel() {
  open.value = false;
  form.value = {};
}

/** 表单重置 */
function reset() {
  form.value = {};
  if (formRef.value) {
    formRef.value.resetFields();
  }
}

// 多选框选中数据
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.processId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 新增按钮操作 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加审批流";
}

/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  const processId = row.processId || ids.value[0];
  getProcess(processId).then((response) => {
    form.value = response.data;
    open.value = true;
    title.value = "修改审批流";
  });
}

/** 提交按钮 */
function submitForm() {
  formRef.value.validate((valid) => {
    if (valid) {
      if (form.value.processId !== undefined) {
        updateProcess(form.value).then((response) => {
          ElMessage.success("修改成功");
          open.value = false;
          getList();
        });
      } else {
        addProcess(form.value).then((response) => {
          ElMessage.success("新增成功");
          open.value = false;
          getList();
        });
      }
    }
  });
}

/** 删除按钮操作 */
function handleDelete(row) {
  const processIds = row.processId || ids.value;
  ElMessageBox.confirm(`是否确认删除编号为"${processIds}"的数据项？`, "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => {
      return delProcess(processIds);
    })
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
}

/** 修改按钮操作 */
async function handleToVersion(row) {
  flowRef.value?.open(row);
}

getList();
</script>
