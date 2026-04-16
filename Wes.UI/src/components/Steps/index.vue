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
  min-height: 60px;
  padding: 10px 0;
}

.step-item-wrapper {
  display: flex;
  align-items: center;
}

.step-separator {
  margin: 0 6px;
  color: #dcdfe6;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
}

/* 步骤名称容器样式 */
.step-name-container {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 10px 0; /* 增加上下距离以改善垂直居中 */
  transition: all 0.3s ease;
  user-select: none;
  line-height: 1; /* 确保文字行高一致 */

  &.step-name-clickable {
    cursor: pointer;

    &:hover .step-name-text {
      color: #409eff;
    }
  }

  &.step-name-active .step-name-text {
    color: #409eff;
    font-weight: 500;
  }
}

/* 步骤名称文字样式 */
.step-name-text {
  font-size: 14px; /* 改回标准大小 */
  color: #606266; /* 灰色字体 */
  transition: color 0.3s ease;
  line-height: 1.5; /* 改善垂直对齐 */
}

/* 步骤标签样式 */
.step-name-tag {
  font-size: 12px;
  padding: 2px 6px;
  border-radius: 10px;
  background-color: #e6f7ff;
  color: #1890ff;
  border: 1px solid #91d5ff;
  line-height: 1.2; /* 改善标签垂直对齐 */
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
  background-color: #c0c4cc;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  margin-left: 4px;
  i {
    font-size: 12px;
  }

  &:hover {
    background-color: #409eff;
    transform: scale(1.1);
  }
}

.add-step-separator {
  display: flex;
  align-items: center;
  gap: 4px;
  color: #dcdfe6;
  font-size: 12px;
  margin-bottom: 4px;
}

.add-step-text {
  font-size: 12px;
  color: #909399;
  transition: all 0.3s ease;
}

/* 暗黑主题样式 */
:global(.dark) .step-separator {
  color: #434343;
}

:global(.dark) .step-separator .fa {
  color: #434343;
}

:global(.dark) .step-name-text {
  color: #bfbfbf;
}

:global(.dark) .step-name-container.step-name-clickable:hover .step-name-text {
  color: #69b1ff;
}

:global(.dark) .step-name-container.step-name-active .step-name-text {
  color: #69b1ff;
}

:global(.dark) .step-name-tag {
  background-color: #111d2c;
  color: #69b1ff;
  border-color: #153450;
}

:global(.dark) .add-step-icon {
  background-color: #434343;
  color: #bfbfbf;
}

:global(.dark) .add-step-icon .fa {
  color: #bfbfbf;
}

:global(.dark) .add-step-icon:hover {
  background-color: #409eff;
  color: #fff;
}

:global(.dark) .add-step-icon:hover .fa {
  color: #fff;
}

:global(.dark) .add-step-separator {
  color: #434343;
}

:global(.dark) .add-step-separator .fa {
  color: #434343;
}

:global(.dark) .add-step-text {
  color: #8c8c8c;
}
</style>
