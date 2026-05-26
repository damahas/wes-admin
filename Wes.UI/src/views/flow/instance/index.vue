<template>
  <div class="app-container">
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
          type="success"
          plain
          icon="View"
          :disabled="single"
          @click="handleUpdate"
        >
          查看
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="danger"
          plain
          icon="Delete"
          :disabled="multiple"
          @click="handleDelete"
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
      <el-table-column label="业务id" align="center" prop="businessId" />
      <el-table-column label="业务编码" align="center" prop="businessCode" />
      <el-table-column label="流程" align="center" prop="process.processName" />
      <el-table-column label="流程版本" align="center" prop="version.version" />
      <el-table-column label="当前节点" align="center" prop="currentNode.nodeName" />
      <el-table-column label="是否加急" align="center" prop="isUrgent">
        <template #default="scope">
          <i
            v-if="scope.row.isUrgent > 0"
            class="iconfont icon-lightning"
            style="color: #f56c6c"
          ></i>
        </template>
      </el-table-column>
      <el-table-column label="状态" align="center" prop="instanceStatus">
        <template #default="scope">
          <el-tag
            :type="getStatusType(scope.row.instanceStatus)"
            effect="plain"
            size="small"
          >
            {{ getStatusTitle(scope.row.instanceStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="描述" align="center" prop="remark" />
      <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button
            size="small"
            type="text"
            icon="View"
            @click="handleUpdate(scope.row)"
          >
            查看
          </el-button>
          <el-button
            size="small"
            link
            type="danger"
            icon="Delete"
            @click="handleDelete(scope.row)"
          >
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

    <instance-edit ref="formEdit" @change="getList" />
  </div>
</template>

<script setup>
import { ref, reactive } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { listInstance, delInstance } from "@/api/flow/instance";
import instanceEdit from "@/views/flow/instance/edit";
import QueryForm from "@/components/QueryForm/index.vue";

const formEdit = ref(null);

const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const showSearch = ref(true);
const total = ref(0);
const dataList = ref([]);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  params: {},
});

const statusOptions = [
  { label: "发起", value: "start" },
  { label: "审批中", value: "doing" },
  { label: "通过", value: "pass" },
  { label: "不通过", value: "unpass" },
  { label: "挂起", value: "pending" },
];

const queryConfig = [
  {
    label: "业务编码",
    prop: "businessCode",
    type: "input",
    placeholder: "请输入业务编码",
  },
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
    label: "当前节点名称",
    prop: "currentNodeName",
    type: "input",
    placeholder: "请输入当前节点名称",
  },
  {
    label: "是否加急",
    prop: "isUrgent",
    type: "select",
    placeholder: "请选择",
    options: [
      { label: "正常", value: 0 },
      { label: "加急", value: 1 },
    ],
  },
  {
    label: "审批状态",
    prop: "instanceStatus",
    type: "select",
    placeholder: "请选择审批状态",
    options: statusOptions,
  },
];

/** 查询列表 */
function getList() {
  loading.value = true;
  listInstance(queryParams).then((response) => {
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

// 多选框选中数据
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.instanceId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 查看按钮操作 */
function handleUpdate(row) {
  const instanceId = row.instanceId || ids.value[0];
  formEdit.value.openDialog(instanceId);
}

/** 删除按钮操作 */
function handleDelete(row) {
  const instanceIds = row.instanceId || ids.value;
  ElMessageBox.confirm(`是否确认删除编号为"${instanceIds}"的数据项？`, "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => {
      return delInstance(instanceIds);
    })
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
}

function getStatusType(status) {
  switch (status) {
    case "start":
      return "info";
    case "doing":
      return "";
    case "pass":
      return "success";
    case "unpass":
      return "danger";
    case "pending":
      return "warning";
    default:
      return "";
  }
}

function getStatusTitle(status) {
  switch (status) {
    case "start":
      return "发起";
    case "doing":
      return "审批中";
    case "pass":
      return "通过";
    case "unpass":
      return "不通过";
    case "pending":
      return "挂起";
    default:
      return status;
  }
}

getList();
</script>
