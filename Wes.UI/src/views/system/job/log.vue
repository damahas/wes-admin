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
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
            v-hasPermi="['monitor:job:remove']"
          >删除</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            @click="handleClean"
            v-hasPermi="['monitor:job:remove']"
          >清空</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            plain
            icon="ArrowLeft"
            @click="handleBack"
          >返回</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table
        v-loading="loading"
        :data="logList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="任务名称" align="center" prop="jobName" :show-overflow-tooltip="true" />
        <el-table-column label="任务组名" align="center" prop="jobGroup" />
        <el-table-column label="调用目标" align="center" prop="invokeTarget" :show-overflow-tooltip="true" />
        <el-table-column label="日志信息" align="center" prop="jobMessage" :show-overflow-tooltip="true" />
        <el-table-column label="执行状态" align="center" width="90">
          <template #default="scope">
            <el-tag :type="scope.row.status === '0' ? 'success' : 'danger'" size="small">
              {{ scope.row.status === '0' ? '正常' : '失败' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="耗时(ms)" align="center" prop="elapsedTime" width="90" />
        <el-table-column label="创建时间" align="center" prop="createTime" width="160">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="120" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="View"
              @click="handleDetail(scope.row)"
            >详情</el-button>
            <el-button
              link
              type="danger"
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['monitor:job:remove']"
            >删除</el-button>
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

    <!-- 日志详情对话框 -->
    <el-dialog title="日志详情" v-model="detailOpen" width="700px" append-to-body>
      <el-form :model="detailForm" label-width="110px">
        <el-form-item label="任务名称">{{ detailForm.jobName }}</el-form-item>
        <el-form-item label="任务组名">{{ detailForm.jobGroup }}</el-form-item>
        <el-form-item label="调用目标">{{ detailForm.invokeTarget }}</el-form-item>
        <el-form-item label="日志信息">{{ detailForm.jobMessage }}</el-form-item>
        <el-form-item label="执行状态">
          <el-tag :type="detailForm.status === '0' ? 'success' : 'danger'" size="small">
            {{ detailForm.status === '0' ? '正常' : '失败' }}
          </el-tag>
        </el-form-item>
        <el-form-item label="执行耗时">{{ detailForm.elapsedTime }} ms</el-form-item>
        <el-form-item label="创建时间">{{ formatTime(detailForm.createTime) }}</el-form-item>
        <el-form-item v-if="detailForm.exceptionInfo" label="异常信息">
          <div style="max-height:200px;overflow:auto;white-space:pre-wrap;color:var(--el-color-danger);font-size:13px;">
            {{ detailForm.exceptionInfo }}
          </div>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="detailOpen = false">关 闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="JobLog">
import { ref, reactive } from "vue";
import { useRoute, useRouter } from "vue-router";
import { ElMessage, ElMessageBox } from "element-plus";
import { addDateRange } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import { listJobLog, getJobLog, delJobLog, cleanJobLog } from "@/api/system/job";

const route = useRoute();
const router = useRouter();
const jobId = route.params.jobId;

const logList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const multiple = ref(true);
const total = ref(0);
const detailOpen = ref(false);
const detailForm = ref({});

const queryConfig = [
  {
    label: "任务名称",
    prop: "jobName",
    type: "input",
    placeholder: "请输入任务名称",
  },
  {
    label: "执行状态",
    prop: "status",
    type: "select",
    placeholder: "执行状态",
    options: [
      { value: "0", label: "正常" },
      { value: "1", label: "失败" },
    ],
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
    jobName: undefined,
    status: undefined,
  },
});

/** 查询日志列表 */
function getList() {
  loading.value = true;
  listJobLog(addDateRange(queryParams.value, queryParams.value.params.dateRange)).then((res) => {
    logList.value = res.rows;
    total.value = res.total;
    loading.value = false;
  });
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  handleQuery();
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.jobLogId);
  multiple.value = !selection.length;
}

/** 详情 */
function handleDetail(row) {
  getJobLog(row.jobLogId).then((res) => {
    detailForm.value = res.data;
    detailOpen.value = true;
  });
}

/** 删除 */
function handleDelete(row) {
  const logIds = row.jobLogId || ids.value.join(",");
  ElMessageBox.confirm('是否确认删除日志编号为"' + logIds + '"的数据项？', "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delJobLog(logIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
}

/** 清空 */
function handleClean() {
  ElMessageBox.confirm("是否确认清空所有任务日志？", "提示", {
    confirmButtonText: "确定清空",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => cleanJobLog())
    .then(() => {
      getList();
      ElMessage.success("清空成功");
    })
    .catch(() => {});
}

/** 返回 */
function handleBack() {
  router.push({ path: "/system/job" });
}

getList();
</script>
