<template>
  <div class="app-container">
    <div class="split-panel">
      <!--部门数据-->
      <div class="split-left-panel">
        <div class="split-left__header">
          <div class="split-left__title">部门</div>
          <div class="split-left__search">
            <el-input
              v-model="deptName"
              placeholder="请输入部门名称"
              clearable
              prefix-icon="Search"
            />
          </div>
        </div>
        <div class="split-left__body">
          <el-tree
            :data="deptOptions"
            :props="{ label: 'label', children: 'children' }"
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            ref="deptTreeRef"
            node-key="id"
            highlight-current
            default-expand-all
            @node-click="handleNodeClick"
          />
        </div>
      </div>
      <!--用户数据-->
      <div class="split-right-panel">
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
              v-hasPermi="['system:user:add']"
            >
              新增
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <el-button
              type="success"
              plain
              icon="Edit"
              :disabled="single"
              @click="handleUpdate"
              v-hasPermi="['system:user:edit']"
            >
              修改
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <el-button
              type="danger"
              plain
              icon="Delete"
              :disabled="multiple"
              @click="handleDelete"
              v-hasPermi="['system:user:remove']"
            >
              删除
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <import-file
              button-type="info"
              button-text="导入"
              :upload-url="upload.uploadUrl"
              :template-url="upload.templateUrl"
              v-hasPermi="['system:user:import']"
              @success="handleImportSuccess"
            />
          </el-col>
          <el-col :span="1.5">
            <el-button
              type="warning"
              plain
              icon="Download"
              @click="handleExport"
              v-hasPermi="['system:user:export']"
            >
              导出
            </el-button>
          </el-col>
          <right-toolbar
            v-model:showSearch="showSearch"
            @queryTable="getList"
            :columns="columns"
          ></right-toolbar>
        </el-row>
        <div class="table-container">
          <el-table
            v-loading="loading"
            :data="userList"
            ref="tableRef"
            layout="auto"
            @selection-change="handleSelectionChange"
          >
            <el-table-column type="selection" width="50" align="center" />
            <el-table-column
              label="用户账户"
              align="center"
              prop="account"
              v-if="columns.account.visible"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="用户名称"
              align="center"
              prop="userName"
              v-if="columns.userName.visible"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="部门"
              align="center"
              prop="dept.deptName"
              v-if="columns.deptName.visible"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="手机号码"
              align="center"
              prop="phonenumber"
              v-if="columns.phonenumber.visible"
              width="120"
            />
            <el-table-column label="状态" align="center" v-if="columns.status.visible">
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
            <el-table-column
              label="创建时间"
              align="center"
              prop="createTime"
              v-if="columns.createTime.visible"
              width="160"
            >
              <template #default="scope">
                <span>{{ formatTime(scope.row.createTime) }}</span>
              </template>
            </el-table-column>
            <el-table-column
              label="操作"
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
                  v-hasPermi="['system:user:edit']"
                >
                  修改
                </el-button>
                <el-button
                  link
                  type="danger"
                  icon="Delete"
                  @click="handleDelete(scope.row)"
                  v-hasPermi="['system:user:remove']"
                >
                  删除
                </el-button>
                <el-dropdown
                  style="display: inline-flex; vertical-align: middle; margin-left: 12px"
                >
                  <el-button link type="primary">
                    更多<el-icon><ArrowDown /></el-icon>
                  </el-button>
                  <template #dropdown>
                    <el-dropdown-menu>
                      <el-dropdown-item
                        @click="handleResetPwd(scope.row)"
                        v-hasPermi="['system:user:resetPwd']"
                      >
                        <el-icon><Unlock /></el-icon>重置密码
                      </el-dropdown-item>
                      <el-dropdown-item
                        @click="handleAuthRole(scope.row)"
                        v-hasPermi="['system:user:edit']"
                      >
                        <el-icon><Files /></el-icon>分配角色
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
      </div>
    </div>

    <!-- 添加或修改用户配置对话框 -->
    <el-dialog :title="title" v-model="open" width="600px" append-to-body>
      <el-form :model="form" :rules="rules" ref="userRef" label-width="80px">
        <el-row>
          <el-col :span="12">
            <el-form-item label="用户名称" prop="userName">
              <el-input
                v-model="form.userName"
                placeholder="请输入用户名称"
                maxlength="30"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="归属部门" prop="deptId">
              <el-tree-select
                v-model="form.deptId"
                :data="deptOptions"
                :props="{ value: 'id', label: 'label', children: 'children' }"
                value-key="id"
                placeholder="请选择归属部门"
                check-strictly
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="手机号码" prop="phonenumber">
              <el-input
                v-model="form.phonenumber"
                placeholder="请输入手机号码"
                maxlength="11"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="邮箱" prop="email">
              <el-input v-model="form.email" placeholder="请输入邮箱" maxlength="50" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12" v-if="form.userId == undefined">
            <el-form-item label="用户账户" prop="account">
              <el-input
                v-model="form.account"
                placeholder="请输入用户账户"
                maxlength="100"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.userId == undefined">
            <el-form-item label="用户密码" prop="password">
              <el-input
                v-model="form.password"
                placeholder="请输入用户密码"
                type="password"
                maxlength="20"
                show-password
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="用户性别">
              <el-select v-model="form.sex" placeholder="请选择">
                <el-option value="0" label="男"></el-option>
                <el-option value="1" label="女"></el-option>
                <el-option value="2" label="未知"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态">
              <el-radio-group v-model="form.status">
                <el-radio
                  v-for="dict in sys_normal_disable"
                  :key="dict.value"
                  :value="dict.value"
                >
                  {{ dict.label }}
                </el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="岗位">
              <el-select v-model="form.postIds" multiple placeholder="请选择">
                <el-option
                  v-for="item in postOptions"
                  :key="item.postId"
                  :label="item.postName"
                  :value="item.postId"
                  :disabled="item.status == 1"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="角色">
              <el-select v-model="form.roleIds" multiple placeholder="请选择">
                <el-option
                  v-for="item in roleOptions"
                  :key="item.roleId"
                  :label="item.roleName"
                  :value="item.roleId"
                  :disabled="item.status == 1"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="备注">
              <el-input
                v-model="form.remark"
                type="textarea"
                placeholder="请输入内容"
              ></el-input>
            </el-form-item>
          </el-col>
        </el-row>
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

<script setup name="User">
import { ElMessage, ElMessageBox } from "element-plus";
import { ArrowDown, Unlock, Files } from "@element-plus/icons-vue";
import { useRouter } from "vue-router";
import { ref, reactive, toRefs, watch, inject, unref } from "vue";
import { download, getDict, addDateRange } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import ImportFile from "@/components/ImportFile/index.vue";
import {
  changeUserStatus,
  listUser,
  resetUserPwd,
  delUser,
  getUser,
  updateUser,
  addUser,
  deptTreeSelect,
} from "@/api/system/user";

const router = useRouter();
const queryRef = ref(null);
const deptTreeRef = ref(null);
const userRef = ref(null);
const { sys_normal_disable } = getDict("sys_normal_disable");

const userList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const deptName = ref("");
const deptOptions = ref(undefined);
const postOptions = ref([]);
const roleOptions = ref([]);

// 导入功能配置
const upload = reactive({
  // 上传的地址
  uploadUrl: import.meta.env.VITE_APP_BASE_API + "/system/user/importData",
  templateUrl: "system/user/importTemplate",
});

// 查询条件配置
const queryConfig = [
  {
    label: "用户名称",
    prop: "userName",
    type: "input",
    placeholder: "请输入用户名称",
  },
  {
    label: "手机号码",
    prop: "phonenumber",
    type: "input",
    placeholder: "请输入手机号码",
  },
  {
    label: "状态",
    prop: "status",
    type: "select",
    placeholder: "用户状态",
    options: sys_normal_disable,
  },
  {
    label: "创建时间",
    prop: "dateRange",
    type: "daterange",
    startPlaceholder: "开始日期",
    endPlaceholder: "结束日期",
  },
];

// 列显隐信息
const columns = ref({
  account: { label: "用户账号", visible: true },
  userName: { label: "用户名称", visible: true },
  deptName: { label: "部门", visible: true },
  phonenumber: { label: "手机号码", visible: true },
  status: { label: "状态", visible: true },
  createTime: { label: "创建时间", visible: true },
});

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      userName: undefined,
      phonenumber: undefined,
      status: undefined,
      deptId: undefined,
    },
  },
  rules: {
    userName: [
      { required: true, message: "用户名称不能为空", trigger: "blur" },
      {
        min: 2,
        max: 20,
        message: "用户名称长度必须介于 2 和 20 之间",
        trigger: "blur",
      },
    ],
    account: [
      { required: true, message: "用户账户不能为空", trigger: "blur" },
      {
        min: 2,
        max: 100,
        message: "用户账户长度必须介于 2 和 100 之间",
        trigger: "blur",
      },
    ],
    password: [
      { required: true, message: "用户密码不能为空", trigger: "blur" },
      {
        min: 5,
        max: 20,
        message: "用户密码长度必须介于 5 和 20 之间",
        trigger: "blur",
      },
      {
        pattern: /^[^<>"'|\\]+$/,
        message: "不能包含非法字符：< > \" ' \\ |",
        trigger: "blur",
      },
    ],
    email: [
      {
        type: "email",
        message: "请输入正确的邮箱地址",
        trigger: ["blur", "change"],
      },
    ],
    phonenumber: [
      {
        pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
        message: "请输入正确的手机号码",
        trigger: "blur",
      },
    ],
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 通过条件过滤节点  */
const filterNode = (value, data) => {
  if (!value) return true;
  return data.label.indexOf(value) !== -1;
};

/** 根据名称筛选部门树 */
watch(deptName, (val) => {
  deptTreeRef.value?.filter(val);
});

/** 查询部门下拉树结构 */
function getDeptTree() {
  deptTreeSelect().then((response) => {
    deptOptions.value = response.data;
  });
}

/** 查询用户列表 */
function getList() {
  loading.value = true;
  listUser(addDateRange(queryParams.value, queryParams.value.params.dateRange)).then(
    (res) => {
      loading.value = false;
      userList.value = res.rows;
      total.value = res.total;
    }
  );
}

/** 节点单击事件 */
function handleNodeClick(data) {
  queryParams.value.params.deptId = data.id;
  handleQuery();
}

/** 搜索按钮操作 */
function handleQuery(formData) {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  if (queryRef.value) queryRef.value.resetFields();
  queryParams.value.params.deptId = undefined;
  deptTreeRef.value?.setCurrentKey(null);
  handleQuery();
}

/** 删除按钮操作 */
function handleDelete(row) {
  const userIds = row.userId || ids.value;
  ElMessageBox.confirm('是否确认删除用户编号为"' + userIds + '"的数据项？', "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(function () {
      return delUser(userIds);
    })
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
}

/** 导出按钮操作 */
function handleExport() {
  download(
    "system/user/export",
    {
      ...queryParams.value,
    },
    `user_${new Date().getTime()}.xlsx`
  );
}

/** 用户状态修改  */
function handleStatusChange(row) {
  let text = row.status === "0" ? "启用" : "停用";
  ElMessageBox.confirm('确认要"' + text + '""' + row.userName + '"用户吗?', "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(function () {
      return changeUserStatus(row.userId, row.status);
    })
    .then(() => {
      ElMessage.success(text + "成功");
    })
    .catch(function () {
      row.status = row.status === "0" ? "1" : "0";
    });
}

/** 更多操作 */
// function handleCommand(command, row) {
//   switch (command) {
//     case "handleResetPwd":
//       handleResetPwd(row);
//       break;
//     case "handleAuthRole":
//       handleAuthRole(row);
//       break;
//     default:
//       break;
//   }
// }

/** 跳转角色分配 */
function handleAuthRole(row) {
  const userId = row.userId;
  router.push("/system/user-auth/role/" + userId);
}

/** 重置密码按钮操作 */
function handleResetPwd(row) {
  ElMessageBox.prompt('请输入"' + row.userName + '"的新密码', "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    closeOnClickModal: false,
    inputPattern: /^.{5,20}$/,
    inputErrorMessage: "用户密码长度必须介于 5 和 20 之间",
    inputValidator: (value) => {
      if (/<|>|"|'|\||\\/.test(value)) {
        return "不能包含非法字符：< > \" ' \\ |";
      }
    },
  })
    .then(({ value }) => {
      resetUserPwd(row.userId, value).then(() => {
        ElMessage.success("修改成功，新密码是：" + value);
      });
    })
    .catch(() => {});
}

/** 选择条数  */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.userId);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}

/** 导入成功回调 */
function handleImportSuccess(response) {
  ElMessageBox.alert(
    "<div style='overflow: auto;overflow-x: hidden;max-height: 70vh;padding: 10px 20px 0;'>" +
      response.msg +
      "</div>",
    "导入结果",
    { dangerouslyUseHTMLString: true }
  );
  getList();
}

/** 重置操作表单 */
function reset() {
  form.value = {
    userId: undefined,
    deptId: undefined,
    userName: undefined,
    account: undefined,
    password: undefined,
    phonenumber: undefined,
    email: undefined,
    sex: undefined,
    status: "0",
    remark: undefined,
    postIds: [],
    roleIds: [],
  };
  if (userRef.value) userRef.value.resetFields();
}

/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

/** 新增按钮操作 */
function handleAdd() {
  reset();
  getUser(0).then((response) => {
    postOptions.value = response.posts;
    roleOptions.value = response.roles;
    open.value = true;
    title.value = "添加用户";
    form.value.password = undefined;
  });
}

/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  const userId = row.userId || ids.value;
  getUser(userId).then((response) => {
    form.value = response.data;
    postOptions.value = response.posts;
    roleOptions.value = response.roles;
    form.value.postIds = response.postIds;
    form.value.roleIds = response.roleIds;
    open.value = true;
    title.value = "修改用户";
    form.password = "";
  });
}

/** 提交按钮 */
function submitForm() {
  userRef.value?.validate((valid) => {
    if (valid) {
      if (form.value.userId != undefined) {
        updateUser(form.value).then(() => {
          ElMessage.success("修改成功");
          open.value = false;
          getList();
        });
      } else {
        addUser(form.value).then(() => {
          ElMessage.success("新增成功");
          open.value = false;
          getList();
        });
      }
    }
  });
}

getDeptTree();
getList();
</script>
