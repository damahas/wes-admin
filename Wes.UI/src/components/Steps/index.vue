<template>
  <div class="simple-steps-container">
    <div class="steps-wrapper">
      <!-- 遍历所有步骤 -->
      <div v-for="(step, index) in steps" :key="index" class="step-item-wrapper">

        <!-- 步骤名称和标签容器 -->
        <div
          class="step-name-container"
          :class="{
            'step-name-active': index === activeIndex,
            'step-name-clickable': clickable,
          }"
          @click="handleStepClick(index)"
        >
          <!-- 步骤名称 -->
          <span class="step-name-text">{{ step.name }}</span>
          
          <!-- 如果有标签则显示 -->
          <span v-if="step.tag" class="step-name-tag">{{ step.tag }}</span>
        </div>

        <!-- 可新增区域（在步骤之间，除了最后一步） -->
        <div class="step-separator" v-if="canAddNew && index < steps.length">
          <i class="fa fa-angle-right"></i>
        </div>
        <div
          class="add-step-icon"
          v-if="canAddNew && index < steps.length"
          @click="handleAddClick(index)"
        >
          <i class="fa fa-plus"></i>
        </div>
        <!-- 步骤之间的分隔线（除了第一个步骤前） -->
        <div v-if="index < steps.length - 1" class="step-separator">
          <i class="fa fa-angle-right"></i>
        </div>
      </div>

      <!-- 最后的可新增区域（在最后一个步骤后） -->
      <!-- <div v-if="canAddNew" class="add-step-area" @click="handleAddClick(steps.length)">
        <div class="add-step-separator">
          <i class="fa fa-angle-right"></i>
          <div class="add-step-icon">
            <i class="fa fa-plus"></i>
          </div>
        </div>
      </div> -->
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits } from "vue";

const props = defineProps({
  // 步骤数组
  steps: {
    type: Array,
    required: true,
    default: () => [],
  },
  // 当前激活的步骤索引
  activeIndex: {
    type: Number,
    default: 0,
  },
  // 是否可点击步骤
  clickable: {
    type: Boolean,
    default: true,
  },
  // 是否可以新增步骤
  canAddNew: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["step-click", "step-add"]);

// 处理步骤点击
const handleStepClick = (index) => {
  if (props.clickable) {
    emit("step-click", index);
  }
};

// 处理新增点击
const handleAddClick = (position) => {
  if (props.canAddNew) {
    emit("step-add", position);
  }
};
</script>

<style lang="scss" scoped>
.simple-steps-container {
  width: 100%;
  overflow-x: auto;
}

.steps-wrapper {
  display: flex;
  align-items: center;
}

.step-item-wrapper {
  display: flex;
  align-items: center;
}

.step-separator {
  margin: 0 6px;
  color: var(--text-placeholder);
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
}

.step-name-container {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 10px 0;
  transition: all 0.3s ease;
  user-select: none;
  line-height: 1;

  &.step-name-clickable {
    cursor: pointer;

    &:hover .step-name-text {
      color: var(--theme-color);
    }
  }

  &.step-name-active .step-name-text {
    color: var(--theme-color);
    font-weight: 500;
  }
}

.step-name-text {
  font-size: 14px;
  color: var(--text-secondary);
  transition: color 0.3s ease;
  line-height: 1.5;
}

.step-name-tag {
  font-size: 12px;
  padding: 2px 6px;
  border-radius: 10px;
  background: var(--bg-active);
  color: var(--theme-color);
  border: 1px solid var(--theme-color);
  line-height: 1.2;
  display: inline-flex;
  align-items: center;
}

.add-step-icon {
  cursor: pointer;
  padding: 0 8px;
  transition: all 0.3s ease;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background-color: var(--text-placeholder);
  display: flex;
  align-items: center;
  justify-content: center;
  color: #141614;
  margin-left: 4px;
  i {
    font-size: 12px;
  }

  &:hover {
    background-color: var(--theme-color);
    color: #141614;
    transform: scale(1.1);
  }
}

.add-step-separator {
  display: flex;
  align-items: center;
  gap: 4px;
  color: var(--text-placeholder);
  font-size: 12px;
  margin-bottom: 4px;
}

.add-step-text {
  font-size: 12px;
  color: var(--text-secondary);
  transition: all 0.3s ease;
}
</style>
