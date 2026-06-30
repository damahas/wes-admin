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
            v-hasPermi="['system:config:add']"
            >新增</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate"
            v-hasPermi="['system:config:edit']"
            >修改</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
            v-hasPermi="['system:config:remove']"
            >删除</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="warning"
            plain
            icon="Download"
            @click="handleExport"
            v-hasPermi="['system:config:export']"
            >导出</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Refresh"
            @click="handleRefreshCache"
            v-hasPermi="['system:config:remove']"
            >刷新缓存</el-button
          >
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table
        v-loading="loading"
        :data="configList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column
          label="参数名称"
          align="center"
          prop="configName"
          :show-overflow-tooltip="true"
        />
        <el-table-column
          label="参数键名"
          align="center"
          prop="configKey"
          :show-overflow-tooltip="true"
        />
        <el-table-column
          label="参数键值"
          align="center"
          prop="configValue"
          :show-overflow-tooltip="true"
        />
        <el-table-column label="系统内置" align="center" prop="configType" width="120">
          <template #default="scope">
            <dict-tag :options="sys_yes_no" :value="scope.row.configType" />
          </template>
        </el-table-column>
        <el-table-column
          label="备注"
          align="center"
          prop="remark"
          :show-overflow-tooltip="true"
        />
        <el-table-column label="创建时间" align="center" prop="createTime" width="180">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          label="操作"
          align="center"
          width="150"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              @click="handleUpdate(scope.row)"
              v-hasPermi="['system:config:edit']"
              >修改</el-button
            >
            <el-button
              link
              type="primary"
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['system:config:remove']"
              >删除</el-button
            >
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

    <edit ref="editRef" @change="getList"></edit>
  </div>
</template>

<script setup name="Config">
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { getDict, download, addDateRange } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import edit from "./edit.vue";
import { listConfig, delConfig, refreshCache } from "@/api/system/config";

const { sys_yes_no } = getDict("sys_yes_no");
const editRef = ref(null);
const queryRef = ref(null);

const configList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);

// 查询条件配置
const queryConfig = [
  {
    label: "参数名称",
    prop: "configName",
    type: "input",
    placeholder: "请输入参数名称",
  },
  {
    label: "参数键名",
    prop: "configKey",
    type: "input",
    placeholder: "请输入参数键名",
  },
  {
    label: "系统内置",
    prop: "configType",
    type: "select",
    placeholder: "系统内置",
    options: sys_yes_no,
  },
  {
    label: "创建时间",
    prop: "dateRange",
    type: "daterange",
    startPlaceholder: "开始日期",
    endPlaceholder: "结束日期",
  },
];

const queryParams = ref({
  pageNum: 1,
  pageSize: 10,
  params: {
    configName: undefined,
    configKey: undefined,
    configType: undefined,
  },
});

/** 查询参数列表 */
function getList() {
  loading.value = true;
  listConfig(addDateRange(queryParams.value, queryParams.value.params.dateRange)).then(
    (response) => {
      configList.value = response.rows;
      total.value = response.total;
      loading.value = false;
    }
  );
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  if (queryRef.value) queryRef.value.resetFields();
  handleQuery();
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.configId);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}

/** 新增按钮操作 */
function handleAdd() {
  editRef.value.openDialog();
}

/** 修改按钮操作 */
function handleUpdate(row) {
  const configId = row.configId || ids.value;
  editRef.value.openDialog(configId);
}

/** 删除按钮操作 */
function handleDelete(row) {
  const configIds = row.configId || ids.value;
  ElMessageBox.confirm('是否确认删除参数编号为"' + configIds + '"的数据项？', "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delConfig(configIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
}

/** 导出按钮操作 */
function handleExport() {
  download(
    "system/config/export",
    {
      ...queryParams.value,
    },
    `config_${new Date().getTime()}.xlsx`
  );
}

/** 刷新缓存按钮操作 */
function handleRefreshCache() {
  refreshCache().then(() => {
    ElMessage.success("刷新缓存成功");
  });
}

getList();
</script>
