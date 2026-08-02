
<template>
  <div class="app-container">
    <el-form
      :model="queryParams"
      ref="queryRef"
      v-show="showSearch"
      :inline="true"
    >
      <el-form-item :label="t('role.authUser.account')" prop="account">
        <el-input
          v-model="queryParams.params.account"
          :placeholder="t('role.authUser.placeholder.account')"
          clearable
          style="width: 240px"
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item :label="t('role.authUser.userName')" prop="userName">
        <el-input
          v-model="queryParams.params.userName"
          :placeholder="t('role.authUser.placeholder.userName')"
          clearable
          style="width: 240px"
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item :label="t('role.authUser.phone')" prop="phonenumber">
        <el-input
          v-model="queryParams.params.phonenumber"
          :placeholder="t('role.authUser.placeholder.phone')"
          clearable
          style="width: 240px"
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">
          {{ t('common.search') }}
        </el-button>
        <el-button icon="Refresh" @click="resetQuery">{{ t('common.reset') }}</el-button>
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button
          type="primary"
          plain
          icon="Plus"
          @click="openSelectUser"
          v-hasPermi="['system:role:add']"
        >
          {{ t('role.authUser.addUser') }}
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="danger"
          plain
          icon="CircleClose"
          :disabled="multiple"
          @click="cancelAuthUserAll"
          v-hasPermi="['system:role:remove']"
        >
          {{ t('role.authUser.cancelAuthAll') }}
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="warning" plain icon="Close" @click="handleClose">
          {{ t('common.close') }}
        </el-button>
      </el-col>
      <right-toolbar
        v-model:showSearch="showSearch"
        @queryTable="getList"
      ></right-toolbar>
    </el-row>

    <el-table
      v-loading="loading"
      :data="userList"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column :label="t('role.authUser.account')" prop="account" :show-overflow-tooltip="true" />
      <el-table-column :label="t('role.authUser.userName')" prop="userName" :show-overflow-tooltip="true" />
      <el-table-column :label="t('role.authUser.email')" prop="email" :show-overflow-tooltip="true" />
      <el-table-column :label="t('role.authUser.phone')" prop="phonenumber" :show-overflow-tooltip="true" />
      <el-table-column :label="t('role.authUser.status')" align="center" prop="status">
        <template #default="scope">
          <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
        </template>
      </el-table-column>
      <el-table-column :label="t('role.authUser.createTime')" align="center" prop="createTime" min-width="170">
        <template #default="scope">
          <span>{{ formatTime(scope.row.createTime) }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('role.authUser.actions')" align="center" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button
            link
            type="primary"
            icon="CircleClose"
            @click="cancelAuthUser(scope.row)"
            v-hasPermi="['system:role:remove']"
            >{{ t('role.authUser.cancelAuth') }}</el-button>
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
    <select-user
      ref="selectRef"
      :roleId="queryParams.params.roleId"
      @ok="handleQuery"
    />
  </div>
</template>

<script setup name="AuthUser">
import selectUser from "./selectUser";
import { ref, reactive } from "vue";
import { useRoute, useRouter } from "vue-router";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import { getDict } from "@/utils";
import {
  allocatedUserList,
  authUserCancel,
  authUserCancelAll,
} from "@/api/system/role";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const { sys_normal_disable } = getDict("sys_normal_disable");

const selectRef = ref(null);
const queryRef = ref(null);
const userList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const multiple = ref(true);
const total = ref(0);
const userIds = ref([]);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  params: {
    roleId: route.params.roleId,
    account: undefined,
    userName: undefined,
    phonenumber: undefined,
  },
});

/** 查询授权用户列表 */
function getList() {
  loading.value = true;
  allocatedUserList(queryParams).then((response) => {
    userList.value = response.rows;
    total.value = response.total;
    loading.value = false;
  });
}

/** 返回按钮 */
function handleClose() {
  router.push("/system/role");
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.pageNum = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  queryRef.value?.resetFields();
  handleQuery();
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  userIds.value = selection.map((item) => item.userId);
  multiple.value = !selection.length;
}

/** 打开授权用户表弹窗 */
function openSelectUser() {
  selectRef.value?.show();
}

/** 取消授权按钮操作 */
function cancelAuthUser(row) {
  ElMessageBox.confirm(
    t("role.authUser.confirm.cancelAuth", { name: row.userName }),
    t("common.confirmTitle"),
    {
      confirmButtonText: t("common.confirm"),
      cancelButtonText: t("common.cancel"),
      type: "warning",
    }
  )
    .then(() => authUserCancel({ userId: row.userId, roleId: queryParams.params.roleId }))
    .then(() => {
      getList();
      ElMessage.success(t("role.authUser.message.cancelSuccess"));
    })
    .catch(() => {});
}

/** 批量取消授权按钮操作 */
function cancelAuthUserAll() {
  const roleId = queryParams.params.roleId;
  const uIds = userIds.value.join(",");
  ElMessageBox.confirm(
    t("role.authUser.confirm.cancelAll"),
    t("common.confirmTitle"),
    {
      confirmButtonText: t("common.confirm"),
      cancelButtonText: t("common.cancel"),
      type: "warning",
    }
  )
    .then(() => authUserCancelAll({ roleId: roleId, userIds: uIds }))
    .then(() => {
      getList();
      ElMessage.success(t("role.authUser.message.cancelSuccess"));
    })
    .catch(() => {});
}

getList();
</script>
