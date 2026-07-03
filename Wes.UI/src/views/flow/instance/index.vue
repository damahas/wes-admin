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
            type="success"
            plain
            icon="View"
            :disabled="single"
            @click="handleUpdate"
          >
            {{ t('flow.instance.view') }}
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
            {{ t('flow.instance.delete') }}
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
        <el-table-column :label="t('flow.instance.belongFlow')" align="center" prop="process.processName" />
        <el-table-column :label="t('flow.instance.businessCode')" align="center" prop="businessCode">
          <template #default="scope">
            <span>{{ scope.row.businessCode }}</span>
            <i
              v-if="scope.row.isUrgent > 0"
              class="fa fa-fire"
              style="color: #f56c6c; font-size: 12px; margin-left: 6px"
              :title="t('flow.instance.urgent')"
            ></i>
          </template>
        </el-table-column>
        <el-table-column :label="t('flow.instance.flowName')" align="center" prop="process.processName" />
        <el-table-column :label="t('flow.instance.flowVersion')" align="center" prop="version.version" />
        <el-table-column :label="t('flow.instance.currentNode')" align="center" prop="currentNode.nodeName" />
        <el-table-column :label="t('flow.instance.status')" align="center" prop="instanceStatus">
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
        <el-table-column :label="t('flow.instance.createTime')" align="center" prop="createTime" width="180">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <!-- <el-table-column label="描述" align="center" prop="remark" /> -->
        <el-table-column
          :label="t('flow.instance.actions')"
          align="center"
          min-width="100px"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button link type="primary" icon="View" @click="handleUpdate(scope.row)">
              {{ t('flow.instance.view') }}
            </el-button>
            <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)">
              {{ t('flow.instance.delete') }}
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

    <instance-edit ref="formEdit" @change="getList" />
  </div>
</template>

<script setup>
import { ref, reactive } from "vue";
import { useI18n } from "vue-i18n";
import { ElMessage, ElMessageBox } from "element-plus";
import { listInstance, delInstance } from "@/api/flow/instance";
import instanceEdit from "@/views/flow/instance/edit";
import QueryForm from "@/components/QueryForm/index.vue";

const { t } = useI18n();

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

const statusMap = {
  0: "flow.instance.statusMap.start",
  10: "flow.instance.statusMap.approving",
  100: "flow.instance.statusMap.approved",
  101: "flow.instance.statusMap.rejected",
  200: "flow.instance.statusMap.suspended",
  201: "flow.instance.statusMap.delegated",
  9999: "flow.instance.statusMap.systemAuto",
};

const statusOptions = [
  { label: t("flow.instance.statusMap.start"), value: 0 },
  { label: t("flow.instance.statusMap.approving"), value: 10 },
  { label: t("flow.instance.statusMap.approved"), value: 100 },
  { label: t("flow.instance.statusMap.rejected"), value: 101 },
  { label: t("flow.instance.statusMap.suspended"), value: 200 },
  { label: t("flow.instance.statusMap.delegated"), value: 201 },
  { label: t("flow.instance.statusMap.systemAuto"), value: 9999 },
];

const queryConfig = [
  {
    label: t("flow.instance.queryConfig.businessCode"),
    prop: "businessCode",
    type: "input",
    placeholder: t("flow.instance.placeholder.businessCode"),
  },
  {
    label: t("flow.instance.queryConfig.flowCode"),
    prop: "processCode",
    type: "input",
    placeholder: t("flow.instance.placeholder.flowCode"),
  },
  {
    label: t("flow.instance.queryConfig.flowName"),
    prop: "processName",
    type: "input",
    placeholder: t("flow.instance.placeholder.flowName"),
  },
  {
    label: t("flow.instance.queryConfig.currentNodeName"),
    prop: "currentNodeName",
    type: "input",
    placeholder: t("flow.instance.placeholder.currentNodeName"),
  },
  {
    label: t("flow.instance.queryConfig.isUrgent"),
    prop: "isUrgent",
    type: "select",
    placeholder: t("flow.instance.placeholder.select"),
    options: [
      { label: t("flow.instance.normal"), value: 0 },
      { label: t("flow.instance.urgentYes"), value: 1 },
    ],
  },
  {
    label: t("flow.instance.queryConfig.approvalStatus"),
    prop: "instanceStatus",
    type: "select",
    placeholder: t("flow.instance.placeholder.selectApprovalStatus"),
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
  ElMessageBox.confirm(
    t("flow.instance.confirmDel.delete", { ids: instanceIds }),
    t("flow.instance.tip"),
    {
      confirmButtonText: t("flow.instance.confirmDelete"),
      cancelButtonText: t("flow.instance.cancel"),
      confirmButtonType: "danger",
      type: "warning",
    }
  )
    .then(() => {
      return delInstance(instanceIds);
    })
    .then(() => {
      getList();
      ElMessage.success(t("flow.instance.message.deleteSuccess"));
    })
    .catch(() => {});
}

function getStatusType(status) {
  switch (status) {
    case 0:
      return "info";
    case 10:
      return "";
    case 100:
      return "success";
    case 101:
      return "danger";
    case 200:
      return "warning";
    case 201:
      return "warning";
    case 9999:
      return "";
    default:
      return "";
  }
}

function getStatusTitle(status) {
  return t(statusMap[status]) || status;
}

getList();
</script>
