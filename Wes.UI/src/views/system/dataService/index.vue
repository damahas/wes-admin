<template>
  <div class="app-container">
    <div class="split-panel">
      <!-- 分类树 -->
      <div class="split-left-panel main-panel">
        <el-input
          v-model="categoryName"
          placeholder="请输入分类名称"
          clearable
          prefix-icon="Search"
          style="margin-bottom: 20px"
        />
        <el-tree
          :data="categoryOptions"
          :props="{ label: 'label', children: 'children' }"
          :expand-on-click-node="false"
          :filter-node-method="filterNode"
          ref="categoryTreeRef"
          node-key="value"
          highlight-current
          default-expand-all
          @node-click="handleNodeClick"
        >
          <template #default="{ data }">
            <span class="tree-node">
              <el-icon><FolderOpened /></el-icon>
              <span>{{ data.label }}</span>
            </span>
          </template>
        </el-tree>
      </div>
      <!-- 数据服务列表 -->
      <div class="split-right-panel main-panel">
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
              v-hasPermi="['system:dataService:add']"
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
              v-hasPermi="['system:dataService:edit']"
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
              v-hasPermi="['system:dataService:remove']"
            >
              删除
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <el-button
              type="warning"
              plain
              icon="Download"
              @click="handleExport"
              v-hasPermi="['system:dataService:export']"
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
            :data="serviceList"
            @selection-change="handleSelectionChange"
          >
            <el-table-column type="selection" width="50" align="center" />
            <el-table-column
              label="服务编码"
              align="center"
              prop="serviceCode"
              v-if="columns.serviceCode && columns.serviceCode.visible"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="服务名称"
              align="center"
              prop="serviceName"
              v-if="columns.serviceName && columns.serviceName.visible"
              :show-overflow-tooltip="true"
            />
            <el-table-column
              label="分类"
              align="center"
              prop="category"
              v-if="columns.category && columns.category.visible"
              :show-overflow-tooltip="true"
              width="120"
            >
              <template #default="scope">
                <dict-tag
                  :options="sys_data_service_category"
                  :value="scope.row.category"
                />
              </template>
            </el-table-column>
            <el-table-column
              label="状态"
              align="center"
              prop="status"
              v-if="columns.status && columns.status.visible"
              width="100"
            >
              <template #default="scope">
                <dict-tag
                  :options="sys_normal_disable"
                  :value="scope.row.status"
                />
              </template>
            </el-table-column>
            <el-table-column
              label="创建时间"
              align="center"
              prop="createTime"
              v-if="columns.createTime && columns.createTime.visible"
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
                <el-tooltip content="预览" placement="top">
                  <el-button
                    link
                    type="primary"
                    icon="View"
                    @click="handlePreview(scope.row)"
                  >预览</el-button>
                </el-tooltip>
                <el-tooltip content="修改" placement="top">
                  <el-button
                    link
                    type="primary"
                    icon="Edit"
                    @click="handleUpdate(scope.row)"
                    v-hasPermi="['system:dataService:edit']"
                  >修改</el-button>
                </el-tooltip>
                <el-tooltip content="删除" placement="top">
                  <el-button
                    link
                    type="danger"
                    icon="Delete"
                    @click="handleDelete(scope.row)"
                    v-hasPermi="['system:dataService:remove']"
                  >删除</el-button>
                </el-tooltip>
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

    <!-- 数据服务编辑弹窗 -->
    <data-service-edit-dialog
      v-model:visible="editDialogVisible"
      :edit-id="editId"
      @success="handleEditSuccess"
      @cancel="editDialogVisible = false"
    />

    <!-- 数据服务预览弹窗 -->
    <data-service-preview-dialog
      v-model:visible="previewDialogVisible"
      :ds-id="previewDsId"
      @close="previewDialogVisible = false"
    />
  </div>
</template>

<script setup name="DataService">
import { ref, reactive, toRefs, computed, watch } from "vue";
import { useRouter } from "vue-router";
import { ElMessage, ElMessageBox } from "element-plus";
import { getDict, handleTree } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import DictTag from "@/components/DictTag/index.vue";
import DataServiceEditDialog from "./edit.vue";
import DataServicePreviewDialog from "./preview.vue";
import { listDataService, delDataService } from "@/api/system/dataService";

const categoryName = ref("");
const categoryTreeRef = ref(null);
const router = useRouter();

const { sys_data_service_category, sys_normal_disable } = getDict("sys_data_service_category", "sys_normal_disable");

const serviceList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);

// 弹窗相关状态
const editDialogVisible = ref(false);
const editId = ref(undefined);
const previewDialogVisible = ref(false);
const previewDsId = ref(undefined);
const categoryOptions = computed(() => {
  if (sys_data_service_category?.value && Array.isArray(sys_data_service_category.value) && sys_data_service_category.value.length > 0) {
    return handleTree(sys_data_service_category.value);
  }
  return [];
});

// 查询条件配置
const queryConfig = computed(() => [
  {
    label: "服务编码",
    prop: "serviceCode",
    type: "input",
    placeholder: "请输入服务编码",
  },
  {
    label: "服务名称",
    prop: "serviceName",
    type: "input",
    placeholder: "请输入服务名称",
  },
  {
    label: "状态",
    prop: "status",
    type: "select",
    placeholder: "状态",
    options: sys_normal_disable?.value || [],
  },
]);

// 列显隐信息
const columns = ref({
  serviceCode: { label: "服务编码", visible: true },
  serviceName: { label: "服务名称", visible: true },
  category: { label: "分类", visible: true },
  status: { label: "状态", visible: true },
  createTime: { label: "创建时间", visible: true },
});

const data = reactive({
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      serviceCode: undefined,
      serviceName: undefined,
      category: undefined,
      status: undefined,
    },
  },
});

const { queryParams } = toRefs(data);

/** 通过条件过滤节点 */
const filterNode = (value, data) => {
  if (!value) return true;
  return data.label.indexOf(value) !== -1;
};

watch(categoryName, (val) => {
  categoryTreeRef.value?.filter(val);
});

/** 查询数据服务列表 */
function getList() {
  loading.value = true;
  listDataService(queryParams.value).then((res) => {
    loading.value = false;
    serviceList.value = res.rows;
    total.value = res.total;
  });
}

/** 节点单击事件 */
function handleNodeClick(data) {
  const clickedKey = data.value;
  const isSame = queryParams.value.params.category === clickedKey;
  
  queryParams.value.params.category = isSame ? undefined : clickedKey;
  categoryTreeRef.value?.setCurrentKey(isSame ? null : clickedKey);
  
  handleQuery();
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  queryParams.value.params = {
  serviceCode: undefined,
  serviceName: undefined,
  category: undefined,
  status: undefined,
  };
  categoryTreeRef.value?.setCurrentKey(null);
  handleQuery();
}

/** 删除按钮操作 */
function handleDelete(row) {
  const serviceIds = row.dsId || ids.value;
  
  // 获取服务编码用于提示信息
  let serviceCodes = [];
  if (row.dsId) {
    serviceCodes = [row.serviceCode];
  } else {
    serviceCodes = serviceList.value
      .filter(item => ids.value.includes(item.dsId))
      .map(item => item.serviceCode || `ID:${item.dsId}`);
  }
  
  const codeText = serviceCodes.length > 0 ? serviceCodes.join(", ") : serviceIds;
  
  ElMessageBox.confirm(
    `是否确认删除服务编码为"${codeText}"的数据项？`,
    "提示",
    {
      confirmButtonText: "确定删除",
      cancelButtonText: "取消",
      confirmButtonType: "danger",
      type: "warning",
    }
  )
    .then(() => delDataService(serviceIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    })
    .catch(() => {});
}

/** 导出按钮操作 */
function handleExport() {
  ElMessage.info("导出功能暂未实现");
}

/** 选择条数 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.dsId);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}

/** 时间格式化 */
function formatTime(time) {
  if (!time) return "";
  return new Date(time).toLocaleString();
}

/** 新增按钮操作 */
function handleAdd() {
  editId.value = undefined;
  editDialogVisible.value = true;
}

/** 修改按钮操作 */
function handleUpdate(row) {
  let dsId;
  if (row.dsId) {
    // 单条修改，直接使用行中的 dsId
    dsId = row.dsId;
  } else if (ids.value.length === 1) {
    // 批量修改但只选了一条
    dsId = ids.value[0];
  } else {
    // 批量修改多条，提示用户只能选择一条
    ElMessage.warning("请选择一条记录进行修改");
    return;
  }

  editId.value = dsId;
  editDialogVisible.value = true;
}

/** 预览按钮操作 */
function handlePreview(row) {
  if (row.dsId) {
    previewDsId.value = row.dsId;
    previewDialogVisible.value = true;
  }
}

/** 弹窗成功回调 */
function handleEditSuccess() {
  editDialogVisible.value = false;
  // 刷新列表
  getList();
}



getList();
</script>

<style scoped>
.tree-node {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
}

.tree-node .el-icon {
  font-size: 14px;
}
</style>