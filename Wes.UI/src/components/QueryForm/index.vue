<template>
  <el-form
    :model="modelValue"
    ref="formRef"
    :inline="true"
    v-show="visible"
    label-width="68px"
  >
    <template v-for="item in queryConfig" :key="item.prop">
      <el-form-item :label="item.label" :prop="item.prop">
        <component
          :is="getComponent(item.type)"
          v-model="modelValue[item.prop]"
          v-bind="getComponentProps(item)"
        >
          <template v-if="item.type === 'select'">
            <el-option
              v-for="option in isRef(item.options) ? item.options.value : item.options"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </template>
        </component>
      </el-form-item>
    </template>
    <el-form-item>
      <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
      <el-button icon="Refresh" @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>

<script setup>
import { ref, reactive, defineEmits, isRef } from "vue";
import { ElInput, ElSelect, ElDatePicker } from "element-plus";

const props = defineProps({
  config: {
    type: Array,
    required: true,
    default: () => [],
  },
  visible: {
    type: Boolean,
    default: true,
  },
  modelValue: {
    type: Object,
    required: true,
  },
});

const emit = defineEmits(["query", "reset", "update:modelValue"]);

const formRef = ref(null);

// 根据配置初始化日期范围
const queryConfig = props.config || [];

// 组件映射
const componentMap = {
  input: ElInput,
  select: ElSelect,
  daterange: ElDatePicker,
};

// 获取组件
function getComponent(type) {
  return componentMap[type] || ElInput;
}

// 获取组件属性
function getComponentProps(item) {
  const baseProps = {
    clearable: true,
    style: { width: item.width || '240px' },
  };

  switch (item.type) {
    case 'input':
      return {
        ...baseProps,
        placeholder: item.placeholder || '请输入' + item.label,
      };
    case 'select':
      return {
        ...baseProps,
        placeholder: item.placeholder || '请选择' + item.label,
      };
    case 'daterange':
      return {
        ...baseProps,
        'value-format': 'YYYY-MM-DD',
        type: 'daterange',
        'range-separator': '-',
        'start-placeholder': item.startPlaceholder || '开始日期',
        'end-placeholder': item.endPlaceholder || '结束日期',
      };
    default:
      return baseProps;
  }
}

// 获取表单数据和日期范围
function getFormData() {
  const result = { ...props.modelValue };
  return result;
}

// 查询按钮
function handleQuery() {
  emit("query", props.modelValue);
}

// 重置按钮
function handleReset() {
  formRef.value?.resetFields();
  emit("reset");
}

// 暴露方法给父组件
defineExpose({
  getFormData,
  resetFields: () => {
    formRef.value?.resetFields();
  },
});
</script>

<style scoped></style>
