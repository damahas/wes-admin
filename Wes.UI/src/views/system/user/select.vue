<template>
  <el-dialog title="选择用户" v-model="isOpen" width="998px" append-to-body>
    <query-form
      :config="queryConfig"
      v-model="queryParams.params"
      @query="handleQuery"
      @reset="resetQuery"
    />

    <el-row :gutter="10" class="mb8" style="margin-top: 10px">
      <span>已选择：</span>
      <el-tag
        v-for="(tag, index) in checked"
        :key="index"
        closable
        style="margin-right: 6px"
        @close="handleCloseTag(index)"
      >
        {{ tag.userName }}
      </el-tag>
    </el-row>

    <el-table v-loading="loading" :data="dataList" @row-click="handleRowClick">
      <el-table-column width="55" align="center">
        <template #default="scope">
          <el-check-tag :checked="isChecked(scope.row)" @change="handleToggle(scope.row)">
            {{ isChecked(scope.row) ? "已选" : "选择" }}
          </el-check-tag>
        </template>
      </el-table-column>
      <el-table-column label="用户编号" align="center" prop="userId" />
      <el-table-column
        label="账户"
        align="center"
        prop="userName"
        :show-overflow-tooltip="true"
      />
      <el-table-column
        label="用户昵称"
        align="center"
        prop="nickName"
        :show-overflow-tooltip="true"
      />
      <el-table-column
        label="部门"
        align="center"
        prop="dept.deptName"
        :show-overflow-tooltip="true"
      />
      <el-table-column label="手机号码" align="center" prop="phonenumber" width="120" />
      <el-table-column label="状态" align="center" prop="status">
        <template #default="scope">
          <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" width="160">
        <template #default="scope">
          <span>{{ formatTime(scope.row.createTime) }}</span>
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

    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="submitForm">确 定</el-button>
        <el-button @click="isOpen = false">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, reactive, defineEmits, defineExpose, defineProps } from "vue";
import { ElMessage } from "element-plus";
import { listUser } from "@/api/system/user";
import { getDict } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";

const { sys_normal_disable } = getDict("sys_normal_disable");

const loading = ref(true);
const checked = ref([]);
const total = ref(0);
const dataList = ref([]);
const isOpen = ref(false);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 8,
  params: {},
});

const queryConfig = [
  { label: "账户", prop: "userName", type: "input", placeholder: "请输入账户" },
  { label: "名称", prop: "nickName", type: "input", placeholder: "请输入名称" },
  {
    label: "手机号码",
    prop: "phonenumber",
    type: "input",
    placeholder: "请输入手机号码",
  },
];

const emit = defineEmits(["handleData"]);

function open() {
  checked.value = [];
  isOpen.value = true;
  getList();
}

function getList() {
  loading.value = true;
  listUser(queryParams).then((response) => {
    dataList.value = response.rows;
    total.value = response.total;
    loading.value = false;
  });
}

function handleQuery() {
  queryParams.pageNum = 1;
  getList();
}

function resetQuery() {
  queryParams.params = {};
  queryParams.pageNum = 1;
  getList();
}

function isChecked(row) {
  return checked.value.some((p) => p.userId == row.userId);
}

function handleToggle(row) {
  if (isChecked(row)) {
    checked.value = checked.value.filter((p) => p.userId != row.userId);
  } else {
    if (checked.value.length && isSingle) {
      checked.value = [];
    }
    checked.value.push(row);
  }
}

function handleRowClick(row) {
  handleToggle(row);
}

function handleCloseTag(index) {
  checked.value.splice(index, 1);
}

function submitForm() {
  if (checked.value.length == 0) {
    ElMessage.error("请选择用户");
    return false;
  }
  isOpen.value = false;
  emit("handleData", checked.value);
}

defineExpose({ open });
</script>

<style lang="scss" scoped>
:deep(.el-dialog__body) {
  padding-top: 0;
}
</style>
