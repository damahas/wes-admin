<template>
  <el-dialog
    v-model="dialogVisible"
    :title="title"
    :fullscreen="true"
    :close-on-click-modal="false"
    :close-on-press-escape="false"
    @closed="handleDialogClosed"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ title }}</span>
        <div class="dialog-header-actions">
          <el-button
            type="primary"
            plain
            icon="ArrowLeft"
            @click="prevStep"
            :disabled="currentStep === 0"
          >
            上一步
          </el-button>

          <el-button
            type="danger"
            plain
            icon="Delete"
            @click="removeCurrentStep"
            :disabled="currentStep === 0 || currentStep > sortedNodes.length"
          >
            删除当前步骤
          </el-button>

          <el-button
            type="primary"
            plain
            icon="ArrowRight"
            @click="nextStep"
            :disabled="currentStep > sortedNodes.length"
          >
            下一步
          </el-button>

          <el-button @click="handleCancel">取消</el-button>
          <el-button type="primary" @click="submitForm">保存</el-button>
        </div>
      </div>
    </template>

    <div class="dialog-content">
      <!-- 步骤条 -->
      <div class="step-container">
        <Steps
          :activeIndex="currentStep"
          :canAddNew="true"
          :steps="steps"
          @step-click="handleStepClick"
          @step-add="openAddStepDialog"
        ></Steps>
      </div>

      <!-- 步骤内容区域 -->
      <div class="step-content">
        <!-- 步骤1: 基础信息 -->
        <div v-show="currentStep === 0" class="step-panel">
          <basic-info-step
            ref="basicInfoRef"
            :form-data="form"
            :category-options="categoryOptions"
            :status-options="sys_normal_disable"
            @validate="handleBasicInfoValidate"
          />
        </div>

        <!-- 步骤配置 -->
        <div
          v-for="(node, index) in sortedNodes"
          :key="index"
          v-show="currentStep === index + 1"
          class="step-panel"
        >
          <!-- SQL步骤 -->
          <sql-step v-if="node.partType === 0" :node="node" :form-data="form" />
          <!-- JS步骤 -->
          <js-step v-else-if="node.partType === 1" :node="node" />
        </div>
      </div>
    </div>

    <!-- 添加步骤弹窗（独立组件） -->
    <add-step-dialog
      v-model:visible="addStepDialogVisible"
      :default-values="newStepForm"
      @confirm="handleAddStepConfirm"
    />
  </el-dialog>
</template>

<script setup name="DataServiceEdit">
import { ref, computed, watch } from "vue";
import { ElMessage } from "element-plus";
import { getDict } from "@/utils";
import Steps from "@/components/Steps";
import BasicInfoStep from "./components/BasicInfoStep.vue";
import SqlStep from "./components/SqlStep.vue";
import JsStep from "./components/JsStep.vue";
import AddStepDialog from "./components/AddStepDialog.vue";
import {
  getDataService,
  addDataService,
  updateDataService,
} from "@/api/system/dataService";

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  editId: {
    type: [String, Number],
    default: undefined
  }
})

const emit = defineEmits(['update:visible', 'success', 'cancel'])

const dialogVisible = ref(false)
const serviceRef = ref(null);
const basicInfoRef = ref(null);
// 获取字典数据
const { sys_data_service_category, sys_normal_disable } = getDict(
  "sys_data_service_category",
  "sys_normal_disable"
);

const title = ref("");
const currentStep = ref(0);

// 添加步骤弹窗相关状态
const addStepDialogVisible = ref(false);
const newStepForm = ref({
  partName: "",
  varName: "",
  partType: 0, // 0: SQL, 1: JS
});

// 监听visible变化
watch(() => props.visible, (newVal) => {
  dialogVisible.value = newVal
  if (newVal) {
    loadData()
  }
})

// 监听dialogVisible变化
watch(() => dialogVisible.value, (newVal) => {
  emit('update:visible', newVal)
  if (!newVal) {
    emit('cancel')
  }
})

// 监听editId变化
watch(() => props.editId, () => {
  if (dialogVisible.value) {
    loadData()
  }
}, { immediate: true })

// Steps 组件需要的步骤数据
const steps = computed(() => {
  const stepList = [{ name: "基础信息" }];

  // 添加动态步骤
  sortedNodes.value.forEach((node, index) => {
    let tag = "";
    switch (node.partType) {
      case 0:
        tag = "SQL";
        break;
      case 1:
        tag = "JS";
        break;
      case 2:
        tag = "API";
        break;
    }

    stepList.push({
      name: node.partName || `步骤${index + 1}`,
      tag: tag,
    });
  });

  return stepList;
});

const form = ref({
  dsId: undefined,
  serviceCode: "",
  serviceName: "",
  category: "",
  paramConfig: "",
  serviceType: "",
  isEnabled: 1,
  remark: "",
  nodes: [],
});

// 使用 computed 自动响应字典数据变化
const categoryOptions = computed(() => {
  const dictArray = sys_data_service_category?.value;
  return Array.isArray(dictArray) && dictArray.length > 0 ? dictArray : [];
});

// 按 sortBy 排序的 nodes
const sortedNodes = computed(() => {
  return [...form.value.nodes].sort((a, b) => (a.sortBy || 0) - (b.sortBy || 0));
});

// 打开添加步骤弹窗
function openAddStepDialog() {
  let insertIndex = currentStep.value;
  if (currentStep.value === 0 && form.value.nodes.length === 0) {
    insertIndex = 0;
  } else if (currentStep.value > sortedNodes.value.length) {
    insertIndex = sortedNodes.value.length;
  }
  
  // 设置默认值
  newStepForm.value = {
    partName: `步骤${insertIndex + 1}`,
    varName: `step${insertIndex + 1}`,
    partType: 0, // 默认SQL类型
  };
  
  addStepDialogVisible.value = true;
}





// 删除当前步骤
function removeCurrentStep() {
  if (currentStep.value > 0 && currentStep.value <= sortedNodes.value.length) {
    const nodeIndex = currentStep.value - 1;
    form.value.nodes.splice(nodeIndex, 1);
    reorderNodes();

    // 如果删除的是当前步骤，跳转到上一步
    if (currentStep.value > sortedNodes.value.length) {
      currentStep.value = sortedNodes.value.length;
    }
  }
}

// 重新排序 nodes
function reorderNodes() {
  form.value.nodes.forEach((node, index) => {
    node.sortBy = index;
  });
}

// 上一步
function prevStep() {
  if (currentStep.value > 0) {
    currentStep.value--;
  }
}

// 下一步
function nextStep() {
  if (currentStep.value < sortedNodes.value.length) {
    currentStep.value++;
  }
}

// 处理步骤点击
function handleStepClick(index) {
  currentStep.value = index;
}

// 处理基础信息验证
function handleBasicInfoValidate(isValid) {
  // 这里可以处理验证结果，如果需要的话
  console.log("基础信息验证结果:", isValid);
}

/** 取消 */
function handleCancel() {
  dialogVisible.value = false;
}

/** 重置表单 */
function reset() {
  form.value = {
    dsId: undefined,
    serviceCode: "",
    serviceName: "",
    category: "",
    paramConfig: "",
    serviceType: "",
    isEnabled: 1,
    remark: "",
    nodes: [],
  };
  currentStep.value = 0;
  if (serviceRef.value) serviceRef.value.resetFields();
}

/** 加载数据 */
function loadData() {
  if (props.editId) {
    title.value = "修改数据服务";
    getDataService(props.editId).then((response) => {
      form.value = response.data;
      // 确保 nodes 是数组
      if (!Array.isArray(form.value.nodes)) {
        form.value.nodes = [];
      }
    });
  } else {
    title.value = "添加数据服务";
    reset();
  }
}

/** 提交表单 */
function submitForm() {
  // 先验证基础信息表单
  if (basicInfoRef.value) {
    basicInfoRef.value.validateForm().then((isValid) => {
      if (isValid) {
        submitData();
      } else {
        ElMessage.error("请填写完整的基础信息");
      }
    });
  } else {
    // 如果没有基础信息组件引用，直接提交
    submitData();
  }
}

// 提交数据
function submitData() {
  const dataToSubmit = { ...form.value };

  const apiCall = dataToSubmit.dsId
    ? updateDataService(dataToSubmit)
    : addDataService(dataToSubmit);

  apiCall
    .then(() => {
      ElMessage.success(dataToSubmit.dsId ? "修改成功" : "新增成功");
      dialogVisible.value = false;
      emit('success')
    })
    .catch((error) => {
      console.error("提交失败:", error);
      ElMessage.error("提交失败，请检查数据格式");
    });
}

// 处理添加步骤弹窗确认
function handleAddStepConfirm(data) {
  let insertIndex = currentStep.value;
  if (currentStep.value === 0 && form.value.nodes.length === 0) {
    insertIndex = 0;
  } else if (currentStep.value > sortedNodes.value.length) {
    insertIndex = sortedNodes.value.length;
  }
  
  // 创建新节点
  const newNode = {
    partName: data.partName,
    varName: data.varName,
    partConfig: "",
    sortBy: insertIndex,
    partType: data.partType,
    isDel: 0,
  };

  // 插入到正确位置并重新排序
  form.value.nodes.splice(insertIndex, 0, newNode);
  reorderNodes();

  // 跳转到新步骤
  currentStep.value = insertIndex + 1;
  
  ElMessage.success('步骤添加成功');
}

// 弹窗关闭时的处理
function handleDialogClosed() {
  // 重置状态
  reset()
}


</script>

<style scoped>
.dialog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  padding-right: 20px;
}

.dialog-title {
  font-size: 16px;
  font-weight: 500;
}

.dialog-header-actions {
  display: flex;
  align-items: center;
  gap: 10px;
}

.dialog-content {
  height: 100%;
  padding: 20px;
}

.step-container {
  margin-bottom: 20px;
}

.step-content {
  min-height: 500px;
}

.step-panel {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

:deep(.el-dialog__header) {
  padding: 16px 20px;
  border-bottom: 1px solid #e8e8e8;
  flex-shrink: 0;
}

:deep(.el-dialog__body) {
  padding: 0;
  flex: 1;
  overflow-y: auto;
}

:deep(.el-dialog) {
  margin: 0 !important;
  max-height: 100vh;
  max-width: 100vw;
  display: flex;
  flex-direction: column;
}

:deep(.el-dialog__wrapper) {
  overflow: hidden;
}
</style>
