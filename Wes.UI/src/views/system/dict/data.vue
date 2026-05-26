<template>
  <div class="app-container">
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
          v-hasPermi="['system:dict:add']"
          >新增</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="success"
          plain
          icon="Edit"
          :disabled="single"
          @click="handleUpdate"
          v-hasPermi="['system:dict:edit']"
          >修改</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="danger"
          plain
          icon="Delete"
          :disabled="multiple"
          @click="handleDelete"
          v-hasPermi="['system:dict:remove']"
          >删除</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="warning"
          plain
          icon="Download"
          @click="handleExport"
          v-hasPermi="['system:dict:export']"
          >导出</el-button
        >
      </el-col>
      <el-col :span="1.5">
        <el-button type="warning" plain icon="Close" @click="handleClose">关闭</el-button>
      </el-col>
      <right-toolbar
        v-model:showSearch="showSearch"
        @queryTable="getList"
      ></right-toolbar>
    </el-row>
    <el-table
      v-loading="loading"
      :data="dataList"
      row-key="dictDataId"
      default-expand-all
      :tree-props="{ children: 'children' }"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="字典标签" align="left" prop="dictLabel">
        <template #default="scope">
          <span
            v-if="
              (scope.row.listClass == '' || scope.row.listClass == 'default') &&
              (scope.row.cssClass == '' || scope.row.cssClass == null)
            "
          >
            {{ scope.row.dictLabel }}
          </span>
          <el-tag
            v-else
            :type="scope.row.listClass == 'primary' ? '' : scope.row.listClass"
            :class="scope.row.cssClass"
          >
            {{ scope.row.dictLabel }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="字典键值" align="center" prop="dictValue" />
      <el-table-column label="字典排序" align="center" prop="dictSort" />
      <el-table-column label="状态" align="center" prop="status">
        <template #default="scope">
          <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
        </template>
      </el-table-column>
      <el-table-column
        label="备注"
        align="center"
        prop="remark"
        :show-overflow-tooltip="true"
      />
      <el-table-column label="创建时间" align="center" prop="createTime" width="180">
        <template #default="scope">
          <span>{{ formatTime(scope.row.createTime) }}</span>
        </template>
      </el-table-column>
      <el-table-column
        label="操作"
        align="center"
        width="160"
        class-name="small-padding fixed-width"
      >
        <template #default="scope">
          <el-button
            link
            type="primary"
            icon="Edit"
            @click="handleUpdate(scope.row)"
            v-hasPermi="['system:dict:edit']"
            >修改</el-button
          >
          <el-button
            link
            type="danger"
            icon="Delete"
            @click="handleDelete(scope.row)"
            v-hasPermi="['system:dict:remove']"
            >删除</el-button
          >
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

    <!-- 添加或修改参数配置对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="dataRef" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="字典类型">
          <el-input v-model="form.dictType" :disabled="true" />
        </el-form-item>
        <el-form-item label="上级节点">
          <el-tree-select
            v-model="form.parentId"
            :data="treeSelectData"
            :props="{ value: 'dictDataId', label: 'dictLabel', children: 'children' }"
            placeholder="请选择上级节点"
            check-strictly
            clearable
            :render-after-expand="false"
          />
        </el-form-item>
        <el-form-item label="数据标签" prop="dictLabel">
          <el-input v-model="form.dictLabel" placeholder="请输入数据标签" />
        </el-form-item>
        <el-form-item label="数据键值" prop="dictValue">
          <el-input v-model="form.dictValue" placeholder="请输入数据键值" />
        </el-form-item>
        <el-form-item label="样式属性" prop="cssClass">
          <el-input v-model="form.cssClass" placeholder="请输入样式属性" />
        </el-form-item>
        <el-form-item label="显示排序" prop="dictSort">
          <el-input-number v-model="form.dictSort" controls-position="right" :min="0" />
        </el-form-item>
        <el-form-item label="回显样式" prop="listClass">
          <el-select v-model="form.listClass">
            <el-option
              v-for="item in listClassOptions"
              :key="item.value"
              :label="item.label + '(' + item.value + ')'"
              :value="item.value"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="状态" prop="status">
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
        <el-form-item label="备注" prop="remark">
          <el-input
            v-model="form.remark"
            type="textarea"
            placeholder="请输入内容"
          ></el-input>
        </el-form-item>
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

<script setup name="Data">
import { ref, reactive, toRefs, getCurrentInstance, onMounted } from "vue";
import { useStore } from "vuex";
import { ElMessage, ElMessageBox } from "element-plus";
import { useRoute } from "vue-router";
import { getDict, download, handleTree } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import { optionselect as getDictOptionselect, getType } from "@/api/system/dict";
import { listData, getData, delData, addData, updateData } from "@/api/system/dict";

const { proxy } = getCurrentInstance();
const store = useStore();
const { sys_normal_disable } = getDict("sys_normal_disable");
const route = useRoute();

const dataList = ref([]);
const flatDataList = ref([]);
const treeSelectData = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const defaultDictType = ref("");
const typeOptions = ref([]);

const dataRef = ref(null);
const queryRef = ref(null);

// 查询条件配置
const queryConfig = [
  {
    label: "字典名称",
    prop: "dictType",
    type: "select",
    placeholder: "请选择字典名称",
    options: typeOptions,
  },
  {
    label: "字典标签",
    prop: "dictLabel",
    type: "input",
    placeholder: "请输入字典标签",
  },
  {
    label: "状态",
    prop: "status",
    type: "select",
    placeholder: "数据状态",
    options: sys_normal_disable,
  },
];
// 数据标签回显样式
const listClassOptions = ref([
  { value: "default", label: "默认" },
  { value: "primary", label: "主要" },
  { value: "success", label: "成功" },
  { value: "info", label: "信息" },
  { value: "warning", label: "警告" },
  { value: "danger", label: "危险" },
]);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      dictType: undefined,
      dictLabel: undefined,
      status: undefined,
    },
  },
  rules: {
    dictLabel: [{ required: true, message: "数据标签不能为空", trigger: "blur" }],
    dictValue: [{ required: true, message: "数据键值不能为空", trigger: "blur" }],
    dictSort: [{ required: true, message: "数据顺序不能为空", trigger: "blur" }],
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 查询字典类型详细 */
function getTypes(dictId) {
  getType(dictId).then((response) => {
    queryParams.value.params.dictType = response.data.dictType;
    defaultDictType.value = response.data.dictType;
    getList();
  });
}

/** 查询字典类型列表 */
function getTypeList() {
  getDictOptionselect().then((response) => {
    typeOptions.value = response.data;
  });
}

/** 查询字典数据列表 */
function getList() {
  loading.value = true;
  listData(queryParams.value).then((response) => {
    flatDataList.value = response.rows;
    dataList.value = handleTree(response.rows, "dictDataId", "parentId", "children");
    treeSelectData.value = [
      { dictDataId: '0', dictLabel: "根节点", children: dataList.value },
    ];
    total.value = response.total;
    loading.value = false;
  });
}

/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

/** 表单重置 */
function reset() {
  form.value = {
    dictCode: undefined,
    dictLabel: undefined,
    dictValue: undefined,
    cssClass: undefined,
    listClass: "default",
    dictSort: 0,
    status: "0",
    remark: undefined,
    parentId: 0,
  };
  if (dataRef.value) dataRef.value.resetFields();
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  if (queryRef.value) queryRef.value.resetFields();
  queryParams.value.params.dictType = defaultDictType.value;
  handleQuery();
}

/** 返回按钮操作 */
function handleClose() {
  const obj = { path: "/system/dict" };
  proxy.$tab.closeOpenPage(obj);
}

/** 新增按钮操作 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加字典数据";
  form.value.dictType = queryParams.value.params.dictType;
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.dictDataId);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}

/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  const dictDataId = row.dictDataId || ids.value;
  getData(dictDataId).then((response) => {
    form.value = response.data;
    open.value = true;
    title.value = "修改字典数据";
  });
}

/** 提交按钮 */
function submitForm() {
  dataRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = form.value.dictCode ? updateData(form.value) : addData(form.value);
      apiCall.then(() => {
        store.dispatch("dict/deleteDict", queryParams.value.params.dictType);
        ElMessage.success(form.value.dictCode ? "修改成功" : "新增成功");
        open.value = false;
        getList();
        store.dispatch("dict/deleteDict", queryParams.value.params.dictType);
      });
    }
  });
}

/** 删除按钮操作 */
function handleDelete(row) {
  const dictDataIds = row.dictDataId || ids.value;
  const dictNames = flatDataList.value
    .filter((p) => dictDataIds.includes(p.dictDataId))
    .map((p) => p.dictLabel);
  ElMessageBox.confirm("是否确认删除字典 " + dictNames.join("，") + " ？", "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delData(dictDataIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
      store.dispatch("dict/deleteDict", queryParams.value.params.dictType);
    })
    .catch(() => {});
}

/** 导出按钮操作 */
function handleExport() {
  download(
    "system/dict/data/export",
    {
      ...queryParams.value,
    },
    `dict_data_${new Date().getTime()}.xlsx`
  );
}

onMounted(() => {
  getTypes(route.params && route.params.dictId);
  getTypeList();
});
</script>
