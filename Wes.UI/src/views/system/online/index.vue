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
            icon="SwitchButton"
            :disabled="multiple"
            @click="handleBatchLogout"
            v-hasPermi="['monitor:online:forceLogout']"
          >批量强退</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table
        v-loading="loading"
        :data="onlineList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" :selectable="isNotExpired" />
        <el-table-column label="用户名称" align="center" prop="userName" :show-overflow-tooltip="true" />
        <el-table-column label="部门" align="center" prop="deptName" :show-overflow-tooltip="true" />
        <el-table-column label="登录IP" align="center" prop="ipaddr" width="140" />
        <el-table-column label="登录地点" align="center" prop="loginLocation" :show-overflow-tooltip="true" />
        <el-table-column label="浏览器" align="center" prop="browser" :show-overflow-tooltip="true" />
        <!-- <el-table-column label="操作系统" align="center" prop="os" :show-overflow-tooltip="true" /> -->
        <el-table-column label="登录时间" align="center" prop="loginTime" min-width="160">
          <template #default="scope">
            <span>{{ formatTime(scope.row.loginTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="过期时间" align="center" prop="expirationTime" min-width="160">
          <template #default="scope">
            <span>{{ formatTime(scope.row.expirationTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="100" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button
              v-if="isNotExpired(null, scope.row)"
              link
              type="danger"
              icon="SwitchButton"
              @click="handleForceLogout(scope.row)"
              v-hasPermi="['monitor:online:forceLogout']"
            >强退</el-button>
            <span v-else style="color:#999;font-size:13px;">已过期</span>
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
  </div>
</template>

<script setup name="Online">
import { ref } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import QueryForm from "@/components/QueryForm/index.vue";
import { listOnline, delOnline } from "@/api/system/online";

const onlineList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const multiple = ref(true);
const total = ref(0);

const queryConfig = [
  {
    label: "用户名称",
    prop: "userName",
    type: "input",
    placeholder: "请输入用户名称",
  },
  {
    label: "登录地点",
    prop: "loginLocation",
    type: "input",
    placeholder: "请输入登录地点",
  },
];

const queryParams = ref({
  pageNum: 1,
  pageSize: 10,
  params: {
    userName: undefined,
    loginLocation: undefined,
  },
});

/** 查询在线用户列表 */
function getList() {
  loading.value = true;
  listOnline(queryParams.value).then((res) => {
    onlineList.value = res.rows;
    total.value = res.total;
    loading.value = false;
  });
}

/** 搜索 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置 */
function resetQuery() {
  handleQuery();
}

/** 多选 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.tokenId);
  multiple.value = !selection.length;
}

/** 判断 token 是否未过期 */
function isNotExpired(_row, row) {
  return new Date(row.expirationTime).getTime() > Date.now();
}

/** 强退单个 */
function handleForceLogout(row) {
  ElMessageBox.confirm('确认强退用户"' + row.userName + '"吗？该用户将被强制下线。', "提示", {
    confirmButtonText: "确定强退",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delOnline(row.tokenId))
    .then(() => {
      getList();
      ElMessage.success("强退成功");
    })
    .catch(() => {});
}

/** 批量强退 */
function handleBatchLogout() {
  const tokenIds = ids.value;
  ElMessageBox.confirm('确认强退选中的 ' + tokenIds.length + ' 个用户吗？', "提示", {
    confirmButtonText: "确定强退",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(async () => {
      // 逐个强退
      await Promise.all(tokenIds.map((id) => delOnline(id)));
    })
    .then(() => {
      getList();
      ElMessage.success("批量强退成功");
    })
    .catch(() => {});
}

getList();
</script>
