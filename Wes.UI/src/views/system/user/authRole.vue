<template>
  <div class="app-container">
    <h4 class="form-header h4">{{ t('user.authRole.basicInfo') }}</h4>
    <el-form :model="form" label-width="auto">
      <el-row>
        <el-col :span="8" :offset="2">
          <el-form-item :label="t('user.authRole.account')" prop="account">
            <el-input v-model="form.account" disabled />
          </el-form-item>
        </el-col>
        <el-col :span="8" :offset="2">
          <el-form-item :label="t('user.authRole.loginAccount')" prop="userName">
            <el-input v-model="form.userName" disabled />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <h4 class="form-header h4">{{ t('user.authRole.roleInfo') }}</h4>
    <el-table
      v-loading="loading"
      :row-key="getRowKey"
      @row-click="clickRow"
      ref="roleRef"
      @selection-change="handleSelectionChange"
      :data="roles?.slice((pageNum - 1) * pageSize, pageNum * pageSize)"
    >
      <el-table-column :label="t('common.sort')" width="55" type="index" align="center">
        <template #default="scope">
          <span>{{ (pageNum - 1) * pageSize + scope.$index + 1 }}</span>
        </template>
      </el-table-column>
      <el-table-column
        type="selection"
        :reserve-selection="true"
        width="55"
      ></el-table-column>
      <el-table-column :label="t('role.roleCode')" align="center" prop="roleId" />
      <el-table-column :label="t('role.roleName')" align="center" prop="roleName" />
      <el-table-column :label="t('role.roleKey')" align="center" prop="roleKey" />
      <el-table-column
        :label="t('common.createTime')"
        align="center"
        prop="createTime"
        width="180"
      >
        <template #default="scope">
          <span>{{ formatTime(scope.row.createTime) }}</span>
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="pageNum"
      v-model:limit="pageSize"
    />

    <el-form label-width="auto">
      <div style="text-align: center; margin-left: -120px; margin-top: 30px">
        <el-button type="primary" @click="submitForm()">{{ t('common.submit') }}</el-button>
        <el-button @click="close()">{{ t('user.authRole.back') }}</el-button>
      </div>
    </el-form>
  </div>
</template>

<script setup name="AuthRole">
import { ref, nextTick } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
import { getAuthRole, updateAuthRole } from "@/api/system/user";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const roleRef = ref(null);
const loading = ref(true);
const total = ref(0);
const pageNum = ref(1);
const pageSize = ref(10);
const roleIds = ref([]);
const roles = ref([]);
const form = ref({
  account: undefined,
  userName: undefined,
  userId: undefined,
});

/** 单击选中行数据 */
function clickRow(row) {
  roleRef.value?.toggleRowSelection(row);
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  roleIds.value = selection.map((item) => item.roleId);
}

/** 保存选中的数据编号 */
function getRowKey(row) {
  return row.roleId;
}

/** 关闭按钮 */
function close() {
  router.push("/system/user");
}

/** 提交按钮 */
function submitForm() {
  const userId = form.value.userId;
  const rIds = roleIds.value.join(",");
  updateAuthRole({ userId: userId, roleIds: rIds }).then(() => {
    ElMessage.success(t("user.authRole.message.authSuccess"));
    close();
  });
}

(() => {
  const userId = route.params?.userId;
  if (userId) {
    loading.value = true;
    getAuthRole(userId).then((response) => {
      form.value = response.data.user;
      roles.value = response.data.roles;
      total.value = roles.value.length;
      nextTick(() => {
        roles.value.forEach((row) => {
          if (form.value.roleIds?.find((p) => p == row.roleId)) {
            roleRef.value?.toggleRowSelection(row);
          }
        });
      });
      loading.value = false;
    });
  }
})();
</script>
