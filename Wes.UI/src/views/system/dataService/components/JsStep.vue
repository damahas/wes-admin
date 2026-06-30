<template>
  <div class="step-wrapper">
    <div class="step-header">
      <div class="header-left">
        <div class="header-icon js-icon">
          <svg
            width="18"
            height="18"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
          >
            <path d="M12 2L2 7l10 5 10-5-10-5z" />
            <path d="M2 17l10 5 10-5" />
            <path d="M2 12l10 5 10-5" />
          </svg>
        </div>
        <div class="header-text">
          <h3>JavaScript配置</h3>
          <p>编写JavaScript代码处理数据和逻辑</p>
        </div>
      </div>
      <el-button type="danger" plain @click="emit('delete')"> 删除 </el-button>
    </div>

    <div class="step-body">
      <div class="form-group flex-1">
        <label class="form-label">JavaScript代码</label>
        <CodeEditor
          :model-value="localPartConfig"
          @update:model-value="updatePartConfig"
          language="javascript"
          placeholder="请输入JavaScript代码"
          height="calc(100vh - 220px)"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import CodeEditor from "@/components/CodeEditor";

const props = defineProps({
  node: {
    type: Object,
    required: true,
  },
});

const emit = defineEmits(["update:node", "delete"]);

// 本地副本用于编辑
const localPartConfig = ref(props.node.partConfig || "");

// 监听props.node.partConfig变化
watch(
  () => props.node.partConfig,
  (newPartConfig) => {
    if (newPartConfig !== localPartConfig.value) {
      localPartConfig.value = newPartConfig || "";
    }
  }
);

// 更新partConfig
function updatePartConfig(newValue) {
  localPartConfig.value = newValue;
  emit("update:node", {
    ...props.node,
    partConfig: newValue,
  });
}
</script>

<style scoped>
.step-wrapper {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.step-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  background: var(--bg-hover);
  border-radius: 12px 12px 0 0;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.header-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.js-icon {
  background: linear-gradient(135deg, #D4A853 0%, #C49A3C 100%);
  color: #141614;
}

.header-text h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: var(--el-text-color-primary);
}

.header-text p {
  margin: 2px 0 0;
  font-size: 12px;
  color: var(--el-text-color-secondary);
}

.step-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
  background: linear-gradient(135deg, #D4A853 0%, #C49A3C 100%);
  color: #141614;
}

.delete-btn {
  color: var(--color-danger);
  border-color: var(--color-danger);
  background: transparent;
}

.delete-btn:hover {
  color: #141614;
  background: var(--color-danger);
  border-color: var(--color-danger);
}

.step-body {
  flex: 1;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  background: var(--bg-card);
  border-radius: 0 0 12px 12px;
  border: 1px solid var(--border-color);
  border-top: none;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.flex-1 {
  flex: 1;
  min-height: 0;
}

.form-label {
  margin-bottom: 8px;
  font-size: 13px;
  font-weight: 500;
  color: var(--el-text-color-regular);
}
</style>
