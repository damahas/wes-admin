<template>
  <el-form
    :model="modelValue"
    ref="formRef"
    :inline="true"
    v-show="visible"
    :label-width="labelWidth"
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
      <el-button type="primary" icon="Search" @click="handleQuery">{{ t('common.search') }}</el-button>
      <el-button icon="Refresh" @click="handleReset">{{ t('common.reset') }}</el-button>
    </el-form-item>
  </el-form>
</template>

<script setup>
import { ref, reactive, defineEmits, isRef } from "vue";
import { useI18n } from "vue-i18n";
import { ElInput, ElSelect, ElDatePicker } from "element-plus";

const { t } = useI18n();

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
  labelWidth: {
    type: [String, Number],
    default: "auto",
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
    style: { width: item.width || '212px' },
  };

  switch (item.type) {
    case 'input':
      return {
        ...baseProps,
        placeholder: item.placeholder || t('common.input') + item.label,
      };
    case 'select':
      return {
        ...baseProps,
        placeholder: item.placeholder || t('common.select') + item.label,
      };
    case 'daterange':
      return {
        ...baseProps,
        'value-format': 'YYYY-MM-DD',
        type: 'daterange',
        'range-separator': '-',
        'start-placeholder': item.startPlaceholder || t('common.startDate'),
        'end-placeholder': item.endPlaceholder || t('common.endDate'),
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
