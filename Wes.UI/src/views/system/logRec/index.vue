<template>
  <div class="app-container">
    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 登录日志 -->
      <el-tab-pane :label="t('logRec.loginLog')" name="login">
        <div class="main-panel p16">
          <query-form
            :config="loginQueryConfig"
            v-model:visible="showSearch"
            v-model="loginParams.params"
            @query="handleLoginQuery"
            @reset="resetLoginQuery"
          />

          <el-table
            v-loading="loginLoading"
            :data="loginData"
            @selection-change="handleLoginSelectionChange"
          >
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column
              :label="t('logRec.column.userName')"
              align="center"
              prop="userName"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              :label="t('logRec.column.loginAddress')"
              align="center"
              prop="ipaddr"
              width="130"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              :label="t('logRec.column.loginLocation')"
              align="center"
              prop="loginLocation"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              :label="t('logRec.column.browser')"
              align="center"
              prop="browser"
              :show-overflow-tooltip="true"
            />
            <el-table-column :label="t('logRec.os')" align="center" prop="os" />
            <el-table-column :label="t('logRec.column.loginStatus')" align="center" prop="status">
              <template #default="scope">
                <dict-tag :options="sys_common_status" :value="scope.row.status" />
              </template>
            </el-table-column>
            <el-table-column :label="t('logRec.column.operInfo')" align="center" prop="msg" />
            <el-table-column :label="t('logRec.column.loginTime')" align="center" prop="loginTime" min-width="170">
              <template #default="scope">
                <span>{{ formatTime(scope.row.loginTime) }}</span>
              </template>
            </el-table-column>
          </el-table>

          <pagination
            v-show="loginTotal > 0"
            :total="loginTotal"
            v-model:page="loginParams.pageNum"
            v-model:limit="loginParams.pageSize"
            @pagination="getLoginList"
          />
        </div>
      </el-tab-pane>

      <!-- 操作日志 -->
      <el-tab-pane :label="t('logRec.operLog')" name="oper">
        <div class="main-panel p16">
          <query-form
            :config="operQueryConfig"
            v-model:visible="showSearch"
            v-model="operParams.params"
            @query="handleOperQuery"
            @reset="resetOperQuery"
          />

          <el-table
            v-loading="operLoading"
            :data="operData"
            @selection-change="handleOperSelectionChange"
          >
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column :label="t('logRec.column.moduleTitle')" align="center" prop="title" />
            <el-table-column :label="t('logRec.actionType')" align="center" prop="businessType">
              <template #default="scope">
                <dict-tag :options="sys_oper_type" :value="scope.row.businessType" />
              </template>
            </el-table-column>
            <el-table-column :label="t('logRec.requestMethod')" align="center" prop="requestMethod" />
            <el-table-column
              :label="t('logRec.column.operName')"
              align="center"
              prop="operName"
              width="100"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              :label="t('logRec.column.operUrl')"
              align="center"
              prop="operIp"
              width="130"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              :label="t('logRec.column.operLocation')"
              align="center"
              prop="operLocation"
              :show-overflow-tooltip="true"
            />
            <el-table-column :label="t('logRec.column.operStatus')" align="center" prop="status">
              <template #default="scope">
                <dict-tag :options="sys_common_status" :value="scope.row.status" />
              </template>
            </el-table-column>
            <el-table-column :label="t('logRec.column.operTime')" align="center" prop="operTime" min-width="170">
              <template #default="scope">
                <span>{{ formatTime(scope.row.operTime) }}</span>
              </template>
            </el-table-column>
            <el-table-column
              :label="t('common.actions')"
              align="center"
              class-name="small-padding fixed-width"
            >
              <template #default="scope">
                <el-button
                  size="small"
                  type="text"
                  icon="View"
                  @click="handleView(scope.row)"
                  >{{ t('logRec.detail') }}</el-button
                >
              </template>
            </el-table-column>
          </el-table>

          <pagination
            v-show="operTotal > 0"
            :total="operTotal"
            v-model:page="operParams.pageNum"
            v-model:limit="operParams.pageSize"
            @pagination="getOperList"
          />
        </div>

        <!-- 操作日志详细 -->
        <el-dialog
          :title="t('logRec.logDetail')"
          v-model="dialogVisible"
          width="700px"
          append-to-body
        >
          <el-form :model="operForm" label-width="auto" size="small">
            <el-row>
              <el-col :span="12">
                <el-form-item :label="t('logRec.moduleTitle') + '：'">
                  {{ operForm.title }} / {{ typeFormat(operForm) }}</el-form-item
                >
                <el-form-item :label="t('logRec.operInfo') + '：'">
                  {{ operForm.operName }} / {{ operForm.operIp }} /
                  {{ operForm.operLocation }}</el-form-item
                >
              </el-col>
              <el-col :span="12">
                <el-form-item :label="t('logRec.operUrl') + '：'">{{ operForm.operUrl }}</el-form-item>
                <el-form-item :label="t('logRec.requestMethod') + '：'">{{
                  operForm.requestMethod
                }}</el-form-item>
              </el-col>
              <el-col :span="24">
                <el-form-item :label="t('logRec.method') + '：'">{{ operForm.method }}</el-form-item>
              </el-col>
              <el-col :span="24">
                <el-form-item :label="t('logRec.operParam') + '：'" class="log-content">{{
                  operForm.operParam
                }}</el-form-item>
              </el-col>
              <el-col :span="24">
                <el-form-item :label="t('logRec.jsonResult') + '：'" class="log-content">{{
                  operForm.jsonResult
                }}</el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="t('logRec.operStatus') + '：'">
                  <el-tag v-if="operForm.status === 0" type="success">{{ t('logRec.normal') }}</el-tag>
                  <el-tag v-else type="danger">{{ t('logRec.failed') }}</el-tag>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="t('logRec.operTime') + '：'">{{
                  formatTime(operForm.operTime)
                }}</el-form-item>
              </el-col>
              <el-col :span="24" v-if="operForm.status === 1">
                <el-form-item :label="t('logRec.errorMsg') + '：'">{{ operForm.errorMsg }}</el-form-item>
              </el-col>
            </el-row>
          </el-form>
          <template #footer>
            <el-button @click="dialogVisible = false">{{ t('common.close') }}</el-button>
          </template>
        </el-dialog>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from "vue";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import DictTag from "@/components/DictTag/index.vue";
import QueryForm from "@/components/QueryForm/index.vue";
import { getDict, addDateRange } from "@/utils";
import { loginList as fetchLoginList, operList as fetchOperList } from "@/api/system/log";

const { t } = useI18n();

const { sys_common_status, sys_oper_type } = getDict(
  "sys_common_status",
  "sys_oper_type"
);

// Tab
const activeTab = ref("login");

// 登录日志
const loginLoading = ref(false);
const loginData = ref([]);
const loginTotal = ref(0);
const showSearch = ref(true);
const loginParams = reactive({
  params: {
    ipaddr: undefined,
    userName: undefined,
    status: undefined,
    dateRange: undefined,
  },
  pageNum: 1,
  pageSize: 10,
});

// 登录日志查询配置
const loginQueryConfig = computed(() => [
  {
    label: t('logRec.loginQuery.loginAddress'),
    prop: "ipaddr",
    type: "input",
    placeholder: t('logRec.placeholder.loginAddress'),
  },
  {
    label: t('logRec.loginQuery.userName'),
    prop: "userName",
    type: "input",
    placeholder: t('logRec.placeholder.userName'),
  },
  {
    label: t('logRec.loginQuery.status'),
    prop: "status",
    type: "select",
    placeholder: t('logRec.placeholder.loginStatus'),
    options: sys_common_status,
  },
  {
    label: t('logRec.loginQuery.loginTime'),
    prop: "dateRange",
    type: "daterange",
    startPlaceholder: t('logRec.loginQuery.startDate'),
    endPlaceholder: t('logRec.loginQuery.endDate'),
  },
]);

function getLoginList() {
  loginLoading.value = true;
  fetchLoginList(addDateRange(loginParams, loginParams.params.dateRange))
    .then((res) => {
      loginData.value = res.rows || res.data?.rows || [];
      loginTotal.value = res.total || res.data?.total || 0;
      loginLoading.value = false;
    })
    .catch((error) => {
      console.error("登录日志请求失败:", error);
      loginLoading.value = false;
      ElMessage.error(t('logRec.message.loadLoginFailed'));
    });
}

function handleLoginQuery() {
  loginParams.pageNum = 1;
  getLoginList();
}

function resetLoginQuery() {
  loginParams.pageNum = 1;
  getLoginList();
}

function handleLoginSelectionChange() {
  // 可以添加选中逻辑
}

// 操作日志
const operLoading = ref(false);
const operData = ref([]);
const operTotal = ref(0);
const dialogVisible = ref(false);
const operForm = ref({});
const operParams = reactive({
  params: {
    title: undefined,
    operName: undefined,
    businessType: undefined,
    status: undefined,
    dateRange: undefined,
  },
  pageNum: 1,
  pageSize: 10,
});

// 操作日志查询配置
const operQueryConfig = computed(() => [
  {
    label: t('logRec.operQuery.moduleTitle'),
    prop: "title",
    type: "input",
    placeholder: t('logRec.placeholder.moduleTitle'),
  },
  {
    label: t('logRec.operQuery.operName'),
    prop: "operName",
    type: "input",
    placeholder: t('logRec.placeholder.userName'),
  },
  {
    label: t('logRec.operQuery.businessType'),
    prop: "businessType",
    type: "select",
    placeholder: t('logRec.placeholder.actionType'),
    options: sys_oper_type,
  },
  {
    label: t('logRec.operQuery.status'),
    prop: "status",
    type: "select",
    placeholder: t('logRec.placeholder.loginStatus'),
    options: sys_common_status,
  },
  {
    label: t('logRec.operQuery.operTime'),
    prop: "dateRange",
    type: "daterange",
    startPlaceholder: t('logRec.operQuery.startDate'),
    endPlaceholder: t('logRec.operQuery.endDate'),
  },
]);

function getOperList() {
  operLoading.value = true;
  fetchOperList(addDateRange(operParams, operParams.params.dateRange))
    .then((res) => {
      console.log("操作日志返回数据:", res);
      operData.value = res.rows || res.data?.rows || [];
      operTotal.value = res.total || res.data?.total || 0;
      operLoading.value = false;
    })
    .catch((error) => {
      console.error("操作日志请求失败:", error);
      operLoading.value = false;
      ElMessage.error(t('logRec.message.loadOperFailed'));
    });
}

function handleOperQuery() {
  operParams.pageNum = 1;
  getOperList();
}

function resetOperQuery() {
  operParams.pageNum = 1;
  getOperList();
}

function handleView(row) {
  operForm.value = row;
  dialogVisible.value = true;
}

function typeFormat(row) {
  const item = sys_oper_type.value?.find((d) => d.value === row.businessType);
  return item ? item.label : "";
}

// Tab切换
function handleTabChange(tab) {
  if (tab === "login") {
    if (loginData.value.length === 0) {
      getLoginList();
    }
  } else if (tab === "oper") {
    if (operData.value.length === 0) {
      getOperList();
    }
  }
}

// 初始加载
getLoginList();
</script>

<style scoped>
.log-content :deep(.el-form-item__content) {
  word-break: break-all;
  word-wrap: break-word;
  white-space: pre-wrap;
  max-height: 200px;
  overflow-y: auto;
  background: #f5f7fa;
  padding: 8px;
  border-radius: 4px;
}
.mb8 {
  margin-bottom: 8px;
}
</style>
