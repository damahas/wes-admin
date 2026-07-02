<template>
  <div class="app-container">
    <div class="split-panel">
      <!-- 左侧：字典类型列表 -->
      <div class="split-left-panel">
        <div class="split-left__header">
          <div class="split-left__title">字典类型</div>
          <div class="split-left__search">
            <el-input
              v-model="dictSearch"
              placeholder="搜索字典名称/类型"
              clearable
              @keyup.enter="handleDictSearch"
              @blur="handleDictSearch"
              @clear="handleDictClear"
            >
              <template #suffix>
                <el-button link type="primary" icon="Plus" class="dict-plus-btn" @click="handleTypeAdd" v-hasPermi="['system:dict:add']" />
              </template>
            </el-input>
          </div>
        </div>
        <div class="split-left__body" v-loading="typeLoading">
          <div
            v-for="item in filteredTypeList"
            :key="item.dictId"
            class="dict-item"
            :class="{ 'is-active': activeDictId === item.dictId }"
            @click="selectDict(item)"
            @mouseenter="item._hover = true"
            @mouseleave="item._hover = false"
          >
            <div class="dict-item__info">
              <span class="dict-item__name">
                {{ item.dictName }}
                <span class="dict-item__dot" :class="item.status === '0' ? 'is-on' : 'is-off'"></span>
              </span>
              <span class="dict-item__type">{{ item.dictType }}</span>
            </div>
            <div v-show="item._hover" class="dict-item__actions">
              <el-button link type="primary" icon="Edit" @click.stop="handleTypeUpdate(item)" v-hasPermi="['system:dict:edit']" />
              <el-button link type="danger" icon="Delete" @click.stop="handleTypeDelete(item)" v-hasPermi="['system:dict:remove']" />
            </div>
          </div>
          <el-empty v-if="filteredTypeList.length === 0 && !typeLoading" description="暂无字典" :image-size="60" />
        </div>
      </div>

      <!-- 右侧：字典数据 -->
      <div class="split-right-panel">
        <template v-if="activeDictType">
          <div class="split-right__header">
            <span class="split-right__title">{{ activeDictType.dictName }}</span>
            <span class="split-right__sub">（{{ activeDictType.dictType }}）</span>
          </div>

          <query-form
            :config="dataQueryConfig"
            v-model:visible="dataShowSearch"
            v-model="dataQueryParams.params"
            @query="handleDataQuery"
            @reset="resetDataQuery"
          />

          <el-row :gutter="10" class="mb8">
            <el-col :span="1.5">
              <el-button type="primary" plain icon="Plus" @click="handleDataAdd" v-hasPermi="['system:dict:add']">新增</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="success" plain icon="Edit" :disabled="dataSingle" @click="handleDataUpdate" v-hasPermi="['system:dict:edit']">修改</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="danger" plain icon="Delete" :disabled="dataMultiple" @click="handleDataDelete" v-hasPermi="['system:dict:remove']">删除</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="dataShowSearch" @queryTable="getDataList" />
          </el-row>

          <el-table
            v-loading="dataLoading"
            :data="dataTreeList"
            row-key="dictDataId"
            default-expand-all
            :tree-props="{ children: 'children' }"
            @selection-change="handleDataSelectionChange"
          >
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column label="字典标签" align="left" prop="dictLabel" min-width="160">
              <template #default="scope">
                <span v-if="(!scope.row.listClass || scope.row.listClass === 'default') && (!scope.row.cssClass || scope.row.cssClass == null)">
                  {{ scope.row.dictLabel }}
                </span>
                <el-tag v-else :type="scope.row.listClass === 'primary' ? '' : scope.row.listClass" :class="scope.row.cssClass">
                  {{ scope.row.dictLabel }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="字典键值" align="center" prop="dictValue" min-width="100" />
            <el-table-column label="字典排序" align="center" prop="dictSort" min-width="80" />
            <el-table-column label="状态" align="center" prop="status" width="90">
              <template #default="scope">
                <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
              </template>
            </el-table-column>
            <el-table-column label="操作" align="center" min-width="120" class-name="small-padding fixed-width">
              <template #default="scope">
                <el-button link type="primary" icon="Edit" @click="handleDataUpdate(scope.row)" v-hasPermi="['system:dict:edit']">修改</el-button>
                <el-button link type="danger" icon="Delete" @click="handleDataDelete(scope.row)" v-hasPermi="['system:dict:remove']">删除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <pagination
            v-show="dataTotal > 0"
            :total="dataTotal"
            v-model:page="dataQueryParams.pageNum"
            v-model:limit="dataQueryParams.pageSize"
            @pagination="getDataList"
          />
        </template>
        <el-empty v-else description="请选择一个字典类型" :image-size="80" />
      </div>
    </div>

    <!-- 字典类型新增/编辑对话框 -->
    <el-dialog :title="typeTitle" v-model="typeOpen" width="500px" append-to-body>
      <el-form ref="typeRef" :model="typeForm" :rules="typeRules" label-width="80px">
        <el-form-item label="字典名称" prop="dictName">
          <el-input v-model="typeForm.dictName" placeholder="请输入字典名称" />
        </el-form-item>
        <el-form-item label="字典类型" prop="dictType">
          <el-input v-model="typeForm.dictType" placeholder="请输入字典类型" />
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="typeForm.status">
            <el-radio v-for="dict in sys_normal_disable" :key="dict.value" :value="dict.value">{{ dict.label }}</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="typeForm.remark" type="textarea" placeholder="请输入内容" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button type="primary" @click="submitTypeForm">确 定</el-button>
        <el-button @click="typeOpen = false">取 消</el-button>
      </template>
    </el-dialog>

    <!-- 字典数据新增/编辑对话框 -->
    <el-dialog :title="dataTitle" v-model="dataOpen" width="500px" append-to-body>
      <el-form ref="dataRef" :model="dataForm" :rules="dataRules" label-width="80px">
        <el-form-item label="字典类型">
          <el-input :model-value="activeDictType?.dictType" disabled />
        </el-form-item>
        <el-form-item label="上级节点">
          <el-tree-select
            v-model="dataForm.parentId"
            :data="treeSelectData"
            :props="{ value: 'dictDataId', label: 'dictLabel', children: 'children' }"
            placeholder="请选择上级节点"
            check-strictly
            clearable
            :render-after-expand="false"
          />
        </el-form-item>
        <el-form-item label="数据标签" prop="dictLabel">
          <el-input v-model="dataForm.dictLabel" placeholder="请输入数据标签" />
        </el-form-item>
        <el-form-item label="数据键值" prop="dictValue">
          <el-input v-model="dataForm.dictValue" placeholder="请输入数据键值" />
        </el-form-item>
        <el-form-item label="样式属性" prop="cssClass">
          <el-input v-model="dataForm.cssClass" placeholder="请输入样式属性" />
        </el-form-item>
        <el-form-item label="显示排序" prop="dictSort">
          <el-input-number v-model="dataForm.dictSort" controls-position="right" :min="0" />
        </el-form-item>
        <el-form-item label="回显样式" prop="listClass">
          <el-select v-model="dataForm.listClass">
            <el-option v-for="item in listClassOptions" :key="item.value" :label="item.label + '(' + item.value + ')'" :value="item.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="dataForm.status">
            <el-radio v-for="dict in sys_normal_disable" :key="dict.value" :value="dict.value">{{ dict.label }}</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="dataForm.remark" type="textarea" placeholder="请输入内容" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button type="primary" @click="submitDataForm">确 定</el-button>
        <el-button @click="dataOpen = false">取 消</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="Dict">
import { ref, reactive, toRefs, computed, nextTick } from "vue";
import { useStore } from "vuex";
import { ElMessage, ElMessageBox } from "element-plus";
import { getDict, handleTree } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import {
  getType,
  delType,
  addType,
  updateType,
  getAllType,
  listData,
  getData,
  delData,
  addData,
  updateData,
} from "@/api/system/dict";

const store = useStore();
const { sys_normal_disable } = getDict("sys_normal_disable");

// ==================== 左侧：字典类型 ====================

const typeRef = ref(null);
const typeLoading = ref(false);
const typeOpen = ref(false);
const typeTitle = ref("");
const typeList = ref([]);
const typeAllList = ref([]);
const dictSearch = ref("");
const activeDictId = ref(null);
const activeDictType = ref(null);

const typeForm = ref({
  dictId: undefined,
  dictName: undefined,
  dictType: undefined,
  status: "0",
  remark: undefined,
});

const typeRules = {
  dictName: [{ required: true, message: "字典名称不能为空", trigger: "blur" }],
  dictType: [{ required: true, message: "字典类型不能为空", trigger: "blur" }],
};

/** 过滤后的字典列表 */
const filteredTypeList = computed(() => {
  if (!dictSearch.value) return typeList.value;
  const kw = dictSearch.value.toLowerCase();
  return typeList.value.filter((item) => {
    return (item.dictName && item.dictName.toLowerCase().includes(kw)) ||
           (item.dictType && item.dictType.toLowerCase().includes(kw));
  });
});

/** 获取字典类型列表 */
function getTypeList() {
  typeLoading.value = true;
  getAllType().then((res) => {
    typeList.value = res.data || [];
    typeLoading.value = false;
    // 保持选中状态
    if (activeDictId.value) {
      const found = typeList.value.find((item) => item.dictId === activeDictId.value);
      if (found) {
        activeDictType.value = found;
      }
    }
  });
}

/** 搜索字典 */
function handleDictSearch() {
  // 搜索过滤由 computed 自动处理
}

/** 清除搜索 */
function handleDictClear() {
  dictSearch.value = "";
}

/** 选中字典类型 */
function selectDict(item) {
  activeDictId.value = item.dictId;
  activeDictType.value = item;
  dataQueryParams.value.pageNum = 1;
  dataQueryParams.value.params.dictLabel = undefined;
  dataQueryParams.value.params.status = undefined;
  dataShowSearch.value = true;
  getDataList();
}

/** 新增字典类型 */
function handleTypeAdd() {
  typeForm.value = { dictId: undefined, dictName: "", dictType: "", status: "0", remark: "" };
  typeTitle.value = "添加字典类型";
  typeOpen.value = true;
  nextTick(() => typeRef.value?.resetFields());
}

/** 修改字典类型 */
function handleTypeUpdate(item) {
  getType(item.dictId).then((res) => {
    typeForm.value = res.data;
    typeTitle.value = "修改字典类型";
    typeOpen.value = true;
  });
}

/** 删除字典类型 */
function handleTypeDelete(item) {
  ElMessageBox.confirm('是否确认删除字典"' + item.dictName + '"？', "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delType(item.dictId))
    .then(() => {
      ElMessage.success("删除成功");
      if (activeDictId.value === item.dictId) {
        activeDictId.value = null;
        activeDictType.value = null;
      }
      getTypeList();
    })
    .catch(() => {});
}

/** 提交字典类型表单 */
function submitTypeForm() {
  typeRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = typeForm.value.dictId ? updateType(typeForm.value) : addType(typeForm.value);
      apiCall.then(() => {
        ElMessage.success(typeForm.value.dictId ? "修改成功" : "新增成功");
        typeOpen.value = false;
        getTypeList();
      });
    }
  });
}

// ==================== 右侧：字典数据 ====================

const dataRef = ref(null);
const dataLoading = ref(false);
const dataOpen = ref(false);
const dataTitle = ref("");
const dataShowSearch = ref(true);
const dataTotal = ref(0);
const dataList = ref([]);
const dataFlatList = ref([]);
const dataTreeList = ref([]);
const treeSelectData = ref([]);
const dataIds = ref([]);
const dataSingle = ref(true);
const dataMultiple = ref(true);

const dataForm = ref({
  dictCode: undefined,
  dictLabel: "",
  dictValue: "",
  cssClass: "",
  listClass: "default",
  dictSort: 0,
  status: "0",
  remark: "",
  parentId: "0",
});

const dataRules = {
  dictLabel: [{ required: true, message: "数据标签不能为空", trigger: "blur" }],
  dictValue: [{ required: true, message: "数据键值不能为空", trigger: "blur" }],
  dictSort: [{ required: true, message: "数据顺序不能为空", trigger: "blur" }],
};

const listClassOptions = [
  { value: "default", label: "默认" },
  { value: "primary", label: "主要" },
  { value: "success", label: "成功" },
  { value: "info", label: "信息" },
  { value: "warning", label: "警告" },
  { value: "danger", label: "危险" },
];

const dataQueryConfig = [
  { label: "字典标签", prop: "dictLabel", type: "input", placeholder: "请输入字典标签" },
  { label: "状态", prop: "status", type: "select", placeholder: "数据状态", options: sys_normal_disable },
];

const dataQueryParams = ref({
  pageNum: 1,
  pageSize: 10,
  params: {
    dictLabel: undefined,
    status: undefined,
  },
});

/** 获取字典数据列表 */
function getDataList() {
  if (!activeDictType.value) return;
  dataLoading.value = true;
  const params = {
    ...dataQueryParams.value,
    params: { ...dataQueryParams.value.params, dictType: activeDictType.value.dictType },
  };
  listData(params).then((res) => {
    dataFlatList.value = res.rows;
    dataList.value = handleTree(res.rows, "dictDataId", "parentId", "children");
    dataTreeList.value = dataList.value;
    treeSelectData.value = [{ dictDataId: "0", dictLabel: "根节点", children: dataList.value }];
    dataTotal.value = res.total;
    dataLoading.value = false;
  });
}

/** 搜索 */
function handleDataQuery() {
  dataQueryParams.value.pageNum = 1;
  getDataList();
}

/** 重置 */
function resetDataQuery() {
  dataQueryParams.value.params.dictLabel = undefined;
  dataQueryParams.value.params.status = undefined;
  handleDataQuery();
}

/** 多选 */
function handleDataSelectionChange(selection) {
  dataIds.value = selection.map((item) => item.dictDataId);
  dataSingle.value = selection.length !== 1;
  dataMultiple.value = !selection.length;
}

/** 新增字典数据 */
function handleDataAdd() {
  dataForm.value = {
    dictCode: undefined,
    dictLabel: "",
    dictValue: "",
    cssClass: "",
    listClass: "default",
    dictSort: 0,
    status: "0",
    remark: "",
    parentId: "0",
  };
  dataTitle.value = "添加字典数据";
  dataOpen.value = true;
  nextTick(() => dataRef.value?.resetFields());
}

/** 修改字典数据 */
function handleDataUpdate(row) {
  const id = row ? row.dictDataId : dataIds.value[0];
  getData(id).then((res) => {
    dataForm.value = res.data;
    dataTitle.value = "修改字典数据";
    dataOpen.value = true;
  });
}

/** 删除字典数据 */
function handleDataDelete(row) {
  const dictDataIds = row ? [row.dictDataId] : dataIds.value;
  const names = dataFlatList.value.filter((p) => dictDataIds.includes(p.dictDataId)).map((p) => p.dictLabel);
  ElMessageBox.confirm("是否确认删除字典 " + names.join("，") + " ？", "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delData(dictDataIds.join(",")))
    .then(() => {
      ElMessage.success("删除成功");
      store.dispatch("dict/deleteDict", activeDictType.value.dictType);
      getDataList();
    })
    .catch(() => {});
}

/** 提交字典数据表单 */
function submitDataForm() {
  dataRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = dataForm.value.dictCode ? updateData(dataForm.value) : addData(dataForm.value);
      apiCall.then(() => {
        store.dispatch("dict/deleteDict", activeDictType.value.dictType);
        ElMessage.success(dataForm.value.dictCode ? "修改成功" : "新增成功");
        dataOpen.value = false;
        getDataList();
      });
    }
  });
}

// 初始化
getTypeList();
</script>

<style scoped>
.dict-plus-btn {
  font-size: 17px;
}

.dict-item {
  display: flex;
  align-items: center;
  padding: 10px 12px;
  cursor: pointer;
  border-left: 3px solid transparent;
  transition: all 0.2s;
  position: relative;
  min-height: 44px;
}

.dict-item:hover {
  background: var(--el-fill-color);
}

.dict-item.is-active {
  background: var(--el-color-primary-light-9);
  border-left-color: var(--el-color-primary);
}

.dict-item__info {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.dict-item__name {
  font-size: 14px;
  color: var(--el-text-color-primary);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.dict-item__type {
  font-size: 12px;
  color: var(--el-text-color-secondary);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.dict-item__dot {
  display: inline-block;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-left: 6px;
  vertical-align: middle;
}

.dict-item__dot.is-on {
  background-color: var(--el-color-success);
}

.dict-item__dot.is-off {
  background-color: var(--el-color-danger);
}

.dict-item__actions {
  display: flex;
  gap: 2px;
  margin-left: 8px;
  flex-shrink: 0;
}
</style>
