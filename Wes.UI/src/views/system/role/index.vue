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
            v-hasPermi="['system:role:add']"
            >{{ t('common.add') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate"
            v-hasPermi="['system:role:edit']"
            >{{ t('common.edit') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
            v-hasPermi="['system:role:remove']"
            >{{ t('common.delete') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="warning"
            plain
            icon="Download"
            @click="handleExport"
            v-hasPermi="['system:role:export']"
            >{{ t('common.export') }}</el-button
          >
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <!-- 表格数据 -->
      <el-table
        v-loading="loading"
        :data="roleList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('role.column.roleCode')" prop="roleCode" width="120" />
        <el-table-column :label="t('role.column.roleName')" prop="roleName" :show-overflow-tooltip="true" />
        <el-table-column :label="t('role.roleKey')" prop="roleKey" :show-overflow-tooltip="true" />
        <el-table-column :label="t('role.column.roleSort')" prop="roleSort" width="100" />
        <el-table-column :label="t('common.status')" align="center" width="100">
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
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="t('common.actions')"
          align="center"
          width="210"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              @click="handleUpdate(scope.row)"
              v-hasPermi="['system:role:edit']"
              v-if="scope.row.roleId !== 1"
              >{{ t('common.edit') }}</el-button
            >
            <el-button
              link
              type="danger"
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['system:role:remove']"
              v-if="scope.row.roleId !== 1"
              >{{ t('common.delete') }}</el-button
            >
            <el-dropdown
              style="display: inline-flex; vertical-align: middle; margin-left: 12px"
              v-if="scope.row.roleId !== 1"
            >
              <el-button link type="primary">
                {{ t('common.more') }}<el-icon><ArrowDown /></el-icon>
              </el-button>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item
                    @click="handleDataScope(scope.row)"
                    v-hasPermi="['system:role:edit']"
                  >
                    <el-icon><Lock /></el-icon>{{ t('role.dataScope') }}
                  </el-dropdown-item>
                  <el-dropdown-item
                    @click="handleAuthUser(scope.row)"
                    v-hasPermi="['system:role:edit']"
                  >
                    <el-icon><User /></el-icon>{{ t('role.assignUser') }}
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
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

    <!-- 添加或修改角色配置对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="roleRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item :label="t('role.roleName')" prop="roleName">
          <el-input v-model="form.roleName" :placeholder="t('role.placeholder.roleName')" />
        </el-form-item>
        <el-form-item prop="roleKey">
          <template #label>
            <span>
              <el-tooltip
                :content="t('menuManage.placeholder.perms')"
                placement="top"
              >
                <el-icon><question-filled /></el-icon>
              </el-tooltip>
              {{ t('role.roleKey') }}
            </span>
          </template>
          <el-input v-model="form.roleKey" :placeholder="t('role.placeholder.roleKey')" />
        </el-form-item>
        <el-form-item :label="t('role.roleSort')" prop="roleSort">
          <el-input-number v-model="form.roleSort" controls-position="right" :min="0" />
        </el-form-item>
        <el-form-item :label="t('common.status')">
          <el-radio-group v-model="form.status">
            <el-radio
              v-for="dict in sys_normal_disable"
              :key="dict.value"
              :value="dict.value"
              >{{ dict.label }}</el-radio
            >
          </el-radio-group>
        </el-form-item>
        <el-form-item :label="t('role.menuPermission')">
          <el-checkbox
            v-model="menuExpand"
            @change="handleCheckedTreeExpand($event, 'menu')"
            >{{ t('role.expandCollapse') }}</el-checkbox
          >
          <el-checkbox
            v-model="menuNodeAll"
            @change="handleCheckedTreeNodeAll($event, 'menu')"
            >{{ t('role.selectDeselect') }}</el-checkbox
          >
          <el-checkbox
            v-model="form.menuCheckStrictly"
            @change="handleCheckedTreeConnect($event, 'menu')"
            >{{ t('role.parentChildLink') }}</el-checkbox
          >
          <el-tree
            class="tree-border"
            :data="menuOptions"
            show-checkbox
            ref="menuRef"
            node-key="id"
            :check-strictly="!form.menuCheckStrictly"
            :empty-text="t('common.loading')"
            :props="{ label: 'label', children: 'children' }"
          ></el-tree>
        </el-form-item>
        <el-form-item :label="t('common.remark')">
          <el-input
            v-model="form.remark"
            type="textarea"
            :placeholder="t('common.pleaseInput')"
          ></el-input>
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">{{ t('common.confirm') }}</el-button>
          <el-button @click="cancel">{{ t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 分配角色数据权限对话框 -->
    <el-dialog :title="title" v-model="openDataScope" width="500px" append-to-body>
      <el-form :model="form" label-width="auto">
        <el-form-item :label="t('role.roleName')">
          <el-input v-model="form.roleName" :disabled="true" />
        </el-form-item>
        <el-form-item :label="t('role.roleKey')">
          <el-input v-model="form.roleKey" :disabled="true" />
        </el-form-item>
        <el-form-item :label="t('role.permissionRange')">
          <el-select v-model="form.dataScope" @change="dataScopeSelectChange">
            <el-option
              v-for="item in dataScopeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item :label="t('role.dataScope')" v-show="form.dataScope == 2">
          <el-checkbox
            v-model="deptExpand"
            @change="handleCheckedTreeExpand($event, 'dept')"
            >{{ t('role.expandCollapse') }}</el-checkbox
          >
          <el-checkbox
            v-model="deptNodeAll"
            @change="handleCheckedTreeNodeAll($event, 'dept')"
            >{{ t('role.selectDeselect') }}</el-checkbox
          >
          <el-checkbox
            v-model="form.deptCheckStrictly"
            @change="handleCheckedTreeConnect($event, 'dept')"
            >{{ t('role.parentChildLink') }}</el-checkbox
          >
          <el-tree
            class="tree-border"
            :data="deptOptions"
            show-checkbox
            default-expand-all
            ref="deptRef"
            node-key="id"
            :check-strictly="!form.deptCheckStrictly"
            :empty-text="t('common.loading')"
            :props="{ label: 'label', children: 'children' }"
          ></el-tree>
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitDataScope">{{ t('common.confirm') }}</el-button>
          <el-button @click="cancelDataScope">{{ t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="Role">
import { ref, reactive, toRefs, nextTick, computed } from "vue";
import { useRouter } from "vue-router";
import { ElMessage, ElMessageBox } from "element-plus";
import { ArrowDown, Lock, User } from "@element-plus/icons-vue";
import { useI18n } from "vue-i18n";
import { getDict, download, addDateRange } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import {
  addRole,
  changeRoleStatus,
  dataScope,
  delRole,
  getRole,
  listRole,
  updateRole,
  deptTreeSelect,
} from "@/api/system/role";
import { roleMenuTreeselect, treeselect as menuTreeselect } from "@/api/system/menu";

const { t } = useI18n();

const router = useRouter();
const roleRef = ref(null);
const { sys_normal_disable } = getDict("sys_normal_disable");

// 查询条件配置
const queryConfig = computed(() => [
  {
    label: t('role.query.roleName'),
    prop: "roleName",
    type: "input",
    placeholder: t('role.placeholder.roleName'),
  },
  {
    label: t('role.query.roleKey'),
    prop: "roleKey",
    type: "input",
    placeholder: t('role.placeholder.roleKey'),
  },
  {
    label: t('role.query.status'),
    prop: "status",
    type: "select",
    placeholder: t('role.placeholder.status'),
    options: sys_normal_disable,
  },
  {
    label: t('role.query.createTime'),
    prop: "dateRange",
    type: "daterange",
    startPlaceholder: t('role.query.startDate'),
    endPlaceholder: t('role.query.endDate'),
  },
]);

const roleList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const menuOptions = ref([]);
const menuExpand = ref(false);
const menuNodeAll = ref(false);
const deptExpand = ref(true);
const deptNodeAll = ref(false);
const deptOptions = ref([]);
const openDataScope = ref(false);
const menuRef = ref(null);
const deptRef = ref(null);

/** 数据范围选项*/
const dataScopeOptions = computed(() => [
  { value: "1", label: t('role.dataScopeOption.all') },
  { value: "2", label: t('role.dataScopeOption.custom') },
  { value: "3", label: t('role.dataScopeOption.dept') },
  { value: "4", label: t('role.dataScopeOption.deptAndBelow') },
  { value: "5", label: t('role.dataScopeOption.self') },
]);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      roleName: undefined,
      roleKey: undefined,
      status: undefined,
      dateRange: [],
    },
  },
  rules: {
    roleName: [{ required: true, message: computed(() => t('role.rules.roleNameRequired')), trigger: "blur" }],
    roleKey: [{ required: true, message: computed(() => t('role.rules.roleKeyRequired')), trigger: "blur" }],
    roleSort: [{ required: true, message: computed(() => t('role.rules.roleSortRequired')), trigger: "blur" }],
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 查询角色列表 */
function getList() {
  loading.value = true;
  listRole(addDateRange(queryParams.value, queryParams.value.params.dateRange)).then(
    (response) => {
      roleList.value = response.rows;
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
  handleQuery();
}

/** 删除按钮操作 */
function handleDelete(row) {
  const roleIds = row.roleId || ids.value;
  ElMessageBox.confirm(t('role.confirm.delete', { id: roleIds }), t('common.confirmTitle'), {
    confirmButtonText: t('common.confirmDelete'),
    cancelButtonText: t('common.cancel'),
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delRole(roleIds))
    .then(() => {
      getList();
      ElMessage.success(t('role.message.deleteSuccess'));
    })
    .catch(() => {});
}

/** 导出按钮操作 */
function handleExport() {
  download(
    "system/role/export",
    {
      ...queryParams.value,
    },
    `role_${new Date().getTime()}.xlsx`
  );
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.roleId);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}

/** 角色状态修改 */
function handleStatusChange(row) {
  const text = row.status === "0" ? t('common.enable') : t('common.disable');
  const successText = row.status === "0" ? t('role.message.enableSuccess') : t('role.message.disableSuccess');
  ElMessageBox.confirm(t('role.confirm.status', { status: text, name: row.roleName }), t('common.confirmTitle'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => changeRoleStatus(row.roleId, row.status))
    .then(() => {
      ElMessage.success(successText);
    })
    .catch(() => {
      row.status = row.status === "0" ? "1" : "0";
    });
}

/** 分配用户 */
function handleAuthUser(row) {
  router.push("/system/role-auth/user/" + row.roleId);
}

/** 查询菜单树结构 */
function getMenuTreeselect() {
  menuTreeselect().then((response) => {
    menuOptions.value = response.data;
  });
}

/** 所有部门节点数据 */
function getDeptAllCheckedKeys() {
  const checkedKeys = deptRef.value?.getCheckedKeys() || [];
  const halfCheckedKeys = deptRef.value?.getHalfCheckedKeys() || [];
  return [...halfCheckedKeys, ...checkedKeys];
}

/** 重置新增的表单以及其他数据  */
function reset() {
  if (menuRef.value) {
    menuRef.value.setCheckedKeys([]);
  }
  menuExpand.value = false;
  menuNodeAll.value = false;
  deptExpand.value = true;
  deptNodeAll.value = false;
  form.value = {
    roleId: undefined,
    roleName: undefined,
    roleKey: undefined,
    roleSort: 0,
    status: "0",
    menuIds: [],
    deptIds: [],
    menuCheckStrictly: true,
    deptCheckStrictly: true,
    remark: undefined,
  };
  if (roleRef.value) roleRef.value.resetFields();
}

/** 添加角色 */
function handleAdd() {
  reset();
  getMenuTreeselect();
  open.value = true;
  title.value = t('role.addTitle');
}

/** 修改角色 */
function handleUpdate(row) {
  reset();
  const roleId = row.roleId || ids.value;
  const roleMenu = getRoleMenuTreeselect(roleId);
  getRole(roleId).then((response) => {
    form.value = response.data;
    form.value.roleSort = Number(form.value.roleSort);
    open.value = true;
    nextTick(() => {
      roleMenu.then((res) => {
        const checkedKeys = res.data.checkedKeys;
        checkedKeys.forEach((v) => {
          nextTick(() => {
            menuRef.value?.setChecked(v, true, false);
          });
        });
      });
    });
    title.value = t('role.editTitle');
  });
}

/** 根据角色ID查询菜单树结构 */
function getRoleMenuTreeselect(roleId) {
  return roleMenuTreeselect(roleId).then((response) => {
    menuOptions.value = response.data.roleTrees;
    return response;
  });
}

/** 根据角色ID查询部门树结构 */
function getDeptTree(roleId) {
  return deptTreeSelect(roleId).then((response) => {
    deptOptions.value = response.data.roleTrees;
    return response;
  });
}

/** 树权限（展开/折叠）*/
function handleCheckedTreeExpand(value, type) {
  const treeList = type === "menu" ? menuOptions.value : deptOptions.value;
  const ref = type === "menu" ? menuRef : deptRef;
  if (!ref.value) return;

  treeList.forEach((item) => {
    ref.value.store.nodesMap[item.id].expanded = value;
  });
}

/** 树权限（全选/全不选） */
function handleCheckedTreeNodeAll(value, type) {
  if (type === "menu") {
    menuRef.value?.setCheckedNodes(value ? menuOptions.value : []);
  } else if (type === "dept") {
    deptRef.value?.setCheckedNodes(value ? deptOptions.value : []);
  }
}

/** 树权限（父子联动） */
function handleCheckedTreeConnect(value, type) {
  if (type === "menu") {
    form.value.menuCheckStrictly = value;
  } else if (type === "dept") {
    form.value.deptCheckStrictly = value;
  }
}

/** 所有菜单节点数据 */
function getMenuAllCheckedKeys() {
  const checkedKeys = menuRef.value?.getCheckedKeys() || [];
  const halfCheckedKeys = menuRef.value?.getHalfCheckedKeys() || [];
  return [...halfCheckedKeys, ...checkedKeys];
}

/** 提交按钮 */
function submitForm() {
  roleRef.value?.validate((valid) => {
    if (valid) {
      form.value.menuIds = getMenuAllCheckedKeys();
      const apiCall = form.value.roleId ? updateRole(form.value) : addRole(form.value);
      apiCall.then(() => {
        ElMessage.success(form.value.roleId ? t('role.message.editSuccess') : t('role.message.addSuccess'));
        open.value = false;
        getList();
      });
    }
  });
}

/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

/** 选择角色权限范围触发 */
function dataScopeSelectChange(value) {
  if (value !== "2") {
    deptRef.value.setCheckedKeys([]);
  }
}

/** 分配数据权限操作 */
function handleDataScope(row) {
  reset();
  const deptTreeSelect = getDeptTree(row.roleId);
  getRole(row.roleId).then((response) => {
    form.value = response.data;
    openDataScope.value = true;
    nextTick(() => {
      deptTreeSelect.then((res) => {
        nextTick(() => {
          deptRef.value?.setCheckedKeys(res.data.checkedKeys);
        });
      });
    });
    title.value = t('role.dataScopeTitle');
  });
}

/** 提交按钮（数据权限） */
function submitDataScope() {
  if (form.value.roleId) {
    form.value.deptIds = getDeptAllCheckedKeys();
    dataScope(form.value).then(() => {
      ElMessage.success(t('role.message.dataScopeSuccess'));
      openDataScope.value = false;
      getList();
    });
  }
}

/** 取消按钮（数据权限）*/
function cancelDataScope() {
  openDataScope.value = false;
  reset();
}

getList();
</script>
