<template>
  <div class="app-container">
    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 登录日志 -->
      <el-tab-pane label="登录日志" name="login">
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
              label="用户名称"
              align="center"
              prop="userName"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="登录地址"
              align="center"
              prop="ipaddr"
              width="130"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="登录地点"
              align="center"
              prop="loginLocation"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="浏览器"
              align="center"
              prop="browser"
              :show-overflow-tooltip="true"
            />
            <el-table-column label="操作系统" align="center" prop="os" />
            <el-table-column label="登录状态" align="center" prop="status">
              <template #default="scope">
                <dict-tag :options="sys_common_status" :value="scope.row.status" />
              </template>
            </el-table-column>
            <el-table-column label="操作信息" align="center" prop="msg" />
            <el-table-column label="登录日期" align="center" prop="loginTime" width="180">
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
      <el-tab-pane label="操作日志" name="oper">
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
            <el-table-column label="系统模块" align="center" prop="title" />
            <el-table-column label="操作类型" align="center" prop="businessType">
              <template #default="scope">
                <dict-tag :options="sys_oper_type" :value="scope.row.businessType" />
              </template>
            </el-table-column>
            <el-table-column label="请求方式" align="center" prop="requestMethod" />
            <el-table-column
              label="操作人员"
              align="center"
              prop="operName"
              width="100"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="操作地址"
              align="center"
              prop="operIp"
              width="130"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="操作地点"
              align="center"
              prop="operLocation"
              :show-overflow-tooltip="true"
            />
            <el-table-column label="操作状态" align="center" prop="status">
              <template #default="scope">
                <dict-tag :options="sys_common_status" :value="scope.row.status" />
              </template>
            </el-table-column>
            <el-table-column label="操作日期" align="center" prop="operTime" width="180">
              <template #default="scope">
                <span>{{ formatTime(scope.row.operTime) }}</span>
              </template>
            </el-table-column>
            <el-table-column
              label="操作"
              align="center"
              class-name="small-padding fixed-width"
            >
              <template #default="scope">
                <el-button
                  size="small"
                  type="text"
                  icon="View"
                  @click="handleView(scope.row)"
                  >详细</el-button
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
          title="操作日志详细"
          v-model="dialogVisible"
          width="700px"
          append-to-body
        >
          <el-form :model="operForm" label-width="100px" size="small">
            <el-row>
              <el-col :span="12">
                <el-form-item label="操作模块："
                  >{{ operForm.title }} / {{ typeFormat(operForm) }}</el-form-item
                >
                <el-form-item label="登录信息："
                  >{{ operForm.operName }} / {{ operForm.operIp }} /
                  {{ operForm.operLocation }}</el-form-item
                >
              </el-col>
              <el-col :span="12">
                <el-form-item label="请求地址：">{{ operForm.operUrl }}</el-form-item>
                <el-form-item label="请求方式：">{{
                  operForm.requestMethod
                }}</el-form-item>
              </el-col>
              <el-col :span="24">
                <el-form-item label="操作方法：">{{ operForm.method }}</el-form-item>
              </el-col>
              <el-col :span="24">
                <el-form-item label="请求参数：" class="log-content">{{
                  operForm.operParam
                }}</el-form-item>
              </el-col>
              <el-col :span="24">
                <el-form-item label="返回参数：" class="log-content">{{
                  operForm.jsonResult
                }}</el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="操作状态：">
                  <el-tag v-if="operForm.status === 0" type="success">正常</el-tag>
                  <el-tag v-else type="danger">失败</el-tag>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="操作时间：">{{
                  formatTime(operForm.operTime)
                }}</el-form-item>
              </el-col>
              <el-col :span="24" v-if="operForm.status === 1">
                <el-form-item label="异常信息：">{{ operForm.errorMsg }}</el-form-item>
              </el-col>
            </el-row>
          </el-form>
          <template #footer>
            <el-button @click="dialogVisible = false">关 闭</el-button>
          </template>
        </el-dialog>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive } from "vue";
import { ElMessage } from "element-plus";
import DictTag from "@/components/DictTag/index.vue";
import QueryForm from "@/components/QueryForm/index.vue";
import { getDict, addDateRange } from "@/utils";
import { loginList as fetchLoginList, operList as fetchOperList } from "@/api/system/log";

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
const loginQueryConfig = [
  {
    label: "登录地址",
    prop: "ipaddr",
    type: "input",
    placeholder: "请输入登录地址",
  },
  {
    label: "用户名称",
    prop: "userName",
    type: "input",
    placeholder: "请输入用户名称",
  },
  {
    label: "状态",
    prop: "status",
    type: "select",
    placeholder: "登录状态",
    options: sys_common_status,
  },
  {
    label: "登录时间",
    prop: "dateRange",
    type: "daterange",
    startPlaceholder: "开始日期",
    endPlaceholder: "结束日期",
  },
];

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
      ElMessage.error("加载登录日志失败");
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
const operQueryConfig = [
  {
    label: "系统模块",
    prop: "title",
    type: "input",
    placeholder: "请输入系统模块",
  },
  {
    label: "操作人员",
    prop: "operName",
    type: "input",
    placeholder: "请输入操作人员",
  },
  {
    label: "类型",
    prop: "businessType",
    type: "select",
    placeholder: "操作类型",
    options: sys_oper_type,
  },
  {
    label: "状态",
    prop: "status",
    type: "select",
    placeholder: "操作状态",
    options: sys_common_status,
  },
  {
    label: "操作时间",
    prop: "dateRange",
    type: "daterange",
    startPlaceholder: "开始日期",
    endPlaceholder: "结束日期",
  },
];

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
      ElMessage.error("加载操作日志失败");
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
