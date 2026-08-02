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
          >{{ t('online.batchForceLogout') }}</el-button>
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
        <el-table-column :label="t('online.userName')" align="center" prop="userName" :show-overflow-tooltip="true" />
        <el-table-column :label="t('online.dept')" align="center" prop="deptName" :show-overflow-tooltip="true" />
        <el-table-column :label="t('online.loginAddress')" align="center" prop="ipaddr" width="140" />
        <el-table-column :label="t('online.loginLocation')" align="center" prop="loginLocation" :show-overflow-tooltip="true" />
        <el-table-column :label="t('online.browser')" align="center" prop="browser" :show-overflow-tooltip="true" />
        <!-- <el-table-column :label="t('online.os')" align="center" prop="os" :show-overflow-tooltip="true" /> -->
        <el-table-column :label="t('online.loginTime')" align="center" prop="loginTime" min-width="170">
          <template #default="scope">
            <span>{{ formatTime(scope.row.loginTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('online.expireTime')" align="center" prop="expirationTime" min-width="170">
          <template #default="scope">
            <span>{{ formatTime(scope.row.expirationTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" align="center" width="140" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button
              v-if="isNotExpired(null, scope.row)"
              link
              type="danger"
              icon="SwitchButton"
              @click="handleForceLogout(scope.row)"
              v-hasPermi="['monitor:online:forceLogout']"
            >{{ t('online.forceLogout') }}</el-button>
            <span v-else style="color:#999;font-size:13px;">{{ t('online.expired') }}</span>
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
import { ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { ElMessage, ElMessageBox } from "element-plus";
import QueryForm from "@/components/QueryForm/index.vue";
import { listOnline, delOnline } from "@/api/system/online";

const { t } = useI18n();

const onlineList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const multiple = ref(true);
const total = ref(0);

const queryConfig = computed(() => [
  {
    label: t("online.userName"),
    prop: "userName",
    type: "input",
    placeholder: t("online.placeholder.userName"),
  },
  {
    label: t("online.loginLocation"),
    prop: "loginLocation",
    type: "input",
    placeholder: t("online.placeholder.loginLocation"),
  },
]);

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
  ElMessageBox.confirm(t('online.confirm.logout', { name: row.userName }), t('online.confirm.title'), {
    confirmButtonText: t('online.confirm.confirmLogout'),
    cancelButtonText: t('common.cancel'),
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delOnline(row.tokenId))
    .then(() => {
      getList();
      ElMessage.success(t('online.message.logoutSuccess'));
    })
    .catch(() => {});
}

/** 批量强退 */
function handleBatchLogout() {
  const tokenIds = ids.value;
  ElMessageBox.confirm(t('online.confirm.batchLogout', { count: tokenIds.length }), t('online.confirm.title'), {
    confirmButtonText: t('online.confirm.confirmLogout'),
    cancelButtonText: t('common.cancel'),
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(async () => {
      // 逐个强退
      await Promise.all(tokenIds.map((id) => delOnline(id)));
    })
    .then(() => {
      getList();
      ElMessage.success(t('online.message.batchLogoutSuccess'));
    })
    .catch(() => {});
}

getList();
</script>
