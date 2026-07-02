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
            v-hasPermi="['monitor:job:add']"
          >{{ t('common.add') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate"
            v-hasPermi="['monitor:job:edit']"
          >{{ t('common.edit') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
            v-hasPermi="['monitor:job:remove']"
          >{{ t('common.delete') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="warning"
            plain
            icon="VideoPlay"
            :disabled="single"
            @click="handleRun"
            v-hasPermi="['monitor:job:edit']"
          >{{ t('job.runOnce') }}</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table
        v-loading="loading"
        :data="jobList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column
          :label="t('job.jobName')"
          align="center"
          prop="jobName"
          :show-overflow-tooltip="true"
        />
        <el-table-column :label="t('job.jobGroup')" align="center" prop="jobGroup" />
        <el-table-column
          :label="t('job.invokeTarget')"
          align="center"
          prop="invokeTarget"
          :show-overflow-tooltip="true"
        />
        <el-table-column
          :label="t('job.cronExpression')"
          align="center"
          prop="cronExpression"
          width="140"
        />
        <el-table-column :label="t('common.status')" align="center" width="80">
          <template #default="scope">
            <el-switch
              v-model="scope.row.status"
              active-value="0"
              inactive-value="1"
              size="small"
              @change="handleStatusChange(scope.row)"
            ></el-switch>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" width="160">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" align="center" width="220" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              @click="handleUpdate(scope.row)"
              v-hasPermi="['monitor:job:edit']"
            >{{ t('common.edit') }}</el-button>
            <el-button
              link
              type="danger"
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['monitor:job:remove']"
            >{{ t('common.delete') }}</el-button>
            <el-button
              link
              type="primary"
              icon="View"
              @click="handleLog(scope.row)"
              v-hasPermi="['monitor:job:list']"
            >{{ t('job.log') }}</el-button>
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

    <!-- 任务日志对话框 -->
    <el-dialog :title="t('job.logTitle')" v-model="logOpen" width="85%" append-to-body>
      <query-form
        :config="logQueryConfig"
        v-model:visible="logShowSearch"
        v-model="logQueryParams.params"
        @query="handleLogQuery"
        @reset="resetLogQuery"
      />

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="logMultiple" @click="handleLogDelete()">删除</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" @click="handleLogClean">清空</el-button>
        </el-col>
      </el-row>

      <el-table v-loading="logLoading" :data="logList" @selection-change="handleLogSelectionChange">
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
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" width="160">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="120" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button link type="primary" icon="View" @click="handleLogDetail(scope.row)">详情</el-button>
            <el-button link type="danger" icon="Delete" @click="handleLogDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination
        v-show="logTotal > 0"
        :total="logTotal"
        v-model:page="logQueryParams.pageNum"
        v-model:limit="logQueryParams.pageSize"
        @pagination="getLogList"
      />

      <!-- 日志详情对话框 -->
      <el-dialog title="日志详情" v-model="logDetailOpen" width="700px" append-to-body>
        <el-form :model="logDetailForm" label-width="110px">
          <el-form-item label="任务名称">{{ logDetailForm.jobName }}</el-form-item>
          <el-form-item label="任务组名">{{ logDetailForm.jobGroup }}</el-form-item>
          <el-form-item label="调用目标">{{ logDetailForm.invokeTarget }}</el-form-item>
          <el-form-item label="日志信息">{{ logDetailForm.jobMessage }}</el-form-item>
          <el-form-item label="执行状态">
            <el-tag :type="logDetailForm.status === '0' ? 'success' : 'danger'" size="small">
              {{ logDetailForm.status === '0' ? '正常' : '失败' }}
            </el-tag>
          </el-form-item>
          <el-form-item label="执行耗时">{{ logDetailForm.elapsedTime }} ms</el-form-item>
          <el-form-item label="创建时间">{{ formatTime(logDetailForm.createTime) }}</el-form-item>
          <el-form-item v-if="logDetailForm.exceptionInfo" label="异常信息">
            <div style="max-height:200px;overflow:auto;white-space:pre-wrap;color:var(--el-color-danger);font-size:13px;">
              {{ logDetailForm.exceptionInfo }}
            </div>
          </el-form-item>
        </el-form>
        <template #footer>
          <el-button @click="logDetailOpen = false">关 闭</el-button>
        </template>
      </el-dialog>
    </el-dialog>

    <!-- 添加或修改任务对话框 -->
    <el-dialog :title="title" v-model="open" width="600px" append-to-body>
      <el-form ref="jobRef" :model="form" :rules="rules" label-width="110px">
        <el-form-item label="任务名称" prop="jobName">
          <el-input v-model="form.jobName" placeholder="请输入任务名称" maxlength="64" />
        </el-form-item>
        <el-form-item label="任务组名" prop="jobGroup">
          <el-input v-model="form.jobGroup" placeholder="请输入任务组名" maxlength="64" />
        </el-form-item>
        <el-form-item label="调用目标" prop="invokeTarget">
          <el-input v-model="form.invokeTarget" placeholder="请输入调用目标字符串" maxlength="500" />
        </el-form-item>
        <el-form-item label="cron表达式" prop="cronExpression">
          <el-input v-model="form.cronExpression" placeholder="请输入cron执行表达式" maxlength="255" />
        </el-form-item>
        <el-form-item label="错误策略" prop="misfirePolicy">
          <el-select v-model="form.misfirePolicy" placeholder="请选择错误策略">
            <el-option label="立即执行" value="1" />
            <el-option label="执行一次" value="2" />
            <el-option label="放弃执行" value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="是否并发" prop="concurrent">
          <el-radio-group v-model="form.concurrent">
            <el-radio value="0">允许</el-radio>
            <el-radio value="1">禁止</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="form.status">
            <el-radio value="0">正常</el-radio>
            <el-radio value="1">暂停</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="form.remark" type="textarea" placeholder="请输入备注" maxlength="500" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="Job">
import { ref, reactive, toRefs } from "vue";
import { useI18n } from "vue-i18n";
import { ElMessage, ElMessageBox } from "element-plus";
import { addDateRange } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";

const { t } = useI18n();
import {
  listJob,
  getJob,
  delJob,
  addJob,
  updateJob,
  changeJobStatus,
  runJob,
  listJobLog,
  getJobLog,
  delJobLog,
  cleanJobLog,
} from "@/api/system/job";

const jobRef = ref(null);

const jobList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

// 日志弹窗
const logOpen = ref(false);
const logList = ref([]);
const logLoading = ref(false);
const logTotal = ref(0);
const logIds = ref([]);
const logMultiple = ref(true);
const logShowSearch = ref(true);
const logDetailOpen = ref(false);
const logDetailForm = ref({});

const logQueryConfig = [
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

const logQueryParams = ref({
  pageNum: 1,
  pageSize: 10,
  params: {
    jobName: undefined,
    status: undefined,
  },
});

const queryConfig = [
  {
    label: "任务名称",
    prop: "jobName",
    type: "input",
    placeholder: "请输入任务名称",
  },
  {
    label: "任务组名",
    prop: "jobGroup",
    type: "input",
    placeholder: "请输入任务组名",
  },
  {
    label: "状态",
    prop: "status",
    type: "select",
    placeholder: "任务状态",
    options: [
      { value: "0", label: "正常" },
      { value: "1", label: "暂停" },
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
    jobGroup: undefined,
    status: undefined,
  },
});

const data = reactive({
  form: {},
  rules: {
    jobName: [{ required: true, message: "任务名称不能为空", trigger: "blur" }],
    invokeTarget: [{ required: true, message: "调用目标不能为空", trigger: "blur" }],
    cronExpression: [{ required: true, message: "cron表达式不能为空", trigger: "blur" }],
  },
});

const { form, rules } = toRefs(data);

/** 查询任务列表 */
function getList() {
  loading.value = true;
  listJob(addDateRange(queryParams.value, queryParams.value.params.dateRange)).then((res) => {
    jobList.value = res.rows;
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

/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

/** 表单重置 */
function reset() {
  form.value = {
    jobId: undefined,
    jobName: undefined,
    jobGroup: "DEFAULT",
    invokeTarget: undefined,
    cronExpression: undefined,
    misfirePolicy: "3",
    concurrent: "1",
    status: "0",
    remark: undefined,
  };
  if (jobRef.value) jobRef.value.resetFields();
}

/** 新增按钮操作 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加任务";
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.jobId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  const jobId = row.jobId || ids.value[0];
  getJob(jobId).then((res) => {
    form.value = res.data;
    open.value = true;
    title.value = "修改任务";
  });
}

/** 提交按钮 */
function submitForm() {
  jobRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = form.value.jobId ? updateJob(form.value) : addJob(form.value);
      apiCall.then(() => {
        ElMessage.success(form.value.jobId ? "修改成功" : "新增成功");
        open.value = false;
        getList();
      });
    }
  });
}

/** 删除按钮操作 */
function handleDelete(row) {
  const jobIds = row.jobId || ids.value.join(",");
  ElMessageBox.confirm('是否确认删除任务编号为"' + jobIds + '"的数据项？', "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delJob(jobIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
}

/** 任务状态修改 */
function handleStatusChange(row) {
  const text = row.status === "0" ? "启用" : "暂停";
  ElMessageBox.confirm('确认要"' + text + '""' + row.jobName + '"任务吗？', "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => changeJobStatus({ jobId: row.jobId, status: row.status }))
    .then(() => {
      ElMessage.success(text + "成功");
    })
    .catch(() => {
      row.status = row.status === "0" ? "1" : "0";
    });
}

/** 执行一次 */
function handleRun(row) {
  const jobId = row.jobId || ids.value[0];
  ElMessageBox.confirm('确认要立即执行"' + (row.jobName || "") + '"任务吗？', "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => runJob(jobId))
    .then(() => {
      ElMessage.success("执行成功");
    })
    .catch(() => {});
}

/** 查看日志 */
function handleLog(row) {
  logQueryParams.value.pageNum = 1;
  logQueryParams.value.params.jobName = undefined;
  logQueryParams.value.params.status = undefined;
  logQueryParams.value.params.dateRange = undefined;
  logOpen.value = true;
  getLogList();
}

/** 查询日志列表 */
function getLogList() {
  logLoading.value = true;
  const params = { ...logQueryParams.value };
  const dateRange = logQueryParams.value.params.dateRange;
  if (dateRange) {
    params.params = { ...params.params };
    delete params.params.dateRange;
  }
  listJobLog(addDateRange(params, dateRange)).then((res) => {
    logList.value = res.rows;
    logTotal.value = res.total;
    logLoading.value = false;
  });
}

/** 日志搜索 */
function handleLogQuery() {
  logQueryParams.value.pageNum = 1;
  getLogList();
}

/** 日志重置 */
function resetLogQuery() {
  handleLogQuery();
}

/** 日志多选 */
function handleLogSelectionChange(selection) {
  logIds.value = selection.map((item) => item.jobLogId);
  logMultiple.value = !selection.length;
}

/** 日志详情 */
function handleLogDetail(row) {
  getJobLog(row.jobLogId).then((res) => {
    logDetailForm.value = res.data;
    logDetailOpen.value = true;
  });
}

/** 日志删除 */
function handleLogDelete(row) {
  const jobLogIds = row ? row.jobLogId : logIds.value.join(",");
  ElMessageBox.confirm('是否确认删除日志编号为"' + jobLogIds + '"的数据项？', "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delJobLog(jobLogIds))
    .then(() => {
      getLogList();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
}

/** 清空日志 */
function handleLogClean() {
  ElMessageBox.confirm("是否确认清空所有任务日志？", "提示", {
    confirmButtonText: "确定清空",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => cleanJobLog())
    .then(() => {
      getLogList();
      ElMessage.success("清空成功");
    })
    .catch(() => {});
}

getList();
</script>
