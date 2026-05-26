<template>
  <teleport to="body">
    <transition name="context-menu-fade">
      <div
        v-if="visible"
        ref="menuRef"
        class="context-menu"
        :style="menuStyle"
        @click.stop
      >
        <slot>
          <div
            v-for="item in menus"
            :key="item.key"
            class="context-menu-item"
            :class="{ 'is-disabled': item.disabled }"
            @click="handleClick(item)"
          >
            <i v-if="item.icon" :class="item.icon"></i>
            <span>{{ item.label }}</span>
          </div>
        </slot>
      </div>
    </transition>
  </teleport>
</template>

<script setup>
import { ref, watch, nextTick, onMounted, onBeforeUnmount } from "vue";

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  menus: {
    type: Array,
    default: () => [],
  },
  x: {
    type: Number,
    default: 0,
  },
  y: {
    type: Number,
    default: 0,
  },
  minWidth: {
    type: Number,
    default: 160,
  },
});

const emit = defineEmits(["select", "update:visible"]);

const menuRef = ref(null);
const menuStyle = ref({});

function updatePosition() {
  if (!props.visible) return;

  const padding = 8;
  let left = props.x;
  let top = props.y;

  // 获取菜单尺寸
  nextTick(() => {
    if (menuRef.value) {
      const menuWidth = menuRef.value.offsetWidth || props.minWidth;
      const menuHeight = menuRef.value.offsetHeight || 40;

      // 边界检测
      if (left + menuWidth > window.innerWidth - padding) {
        left = window.innerWidth - menuWidth - padding;
      }
      if (top + menuHeight > window.innerHeight - padding) {
        top = window.innerHeight - menuHeight - padding;
      }

      menuStyle.value = {
        left: left + "px",
        top: top + "px",
        minWidth: props.minWidth + "px",
      };
    }
  });
}

function handleClick(item) {
  if (item.disabled) return;
  emit("select", item);
  emit("update:visible", false);
}

function handleClickOutside(e) {
  if (menuRef.value && !menuRef.value.contains(e.target)) {
    emit("update:visible", false);
  }
}

watch(
  () => props.visible,
  (val) => {
    if (val) {
      updatePosition();
      nextTick(() => {
        document.addEventListener("click", handleClickOutside);
      });
    } else {
      document.removeEventListener("click", handleClickOutside);
    }
  }
);

watch(
  () => [props.x, props.y],
  () => {
    if (props.visible) {
      updatePosition();
    }
  }
);

onBeforeUnmount(() => {
  document.removeEventListener("click", handleClickOutside);
});
</script>

<style lang="scss" scoped>
.context-menu {
  position: fixed;
  z-index: 9999;
  background: #fff;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  padding: 4px 0;

  .context-menu-item {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 8px 16px;
    cursor: pointer;
    font-size: 14px;
    color: #606266;
    transition: all 0.2s;
    min-width: 100px;

    &:hover:not(.is-disabled) {
      background-color: #f5f7fa;
      color: #409eff;
    }

    &.is-disabled {
      color: #c0c4cc;
      cursor: not-allowed;
    }

    i {
      font-size: 14px;
      width: 16px;
      text-align: center;
    }
  }
}

.context-menu-fade-enter-active,
.context-menu-fade-leave-active {
  transition: opacity 0.15s, transform 0.15s;
}

.context-menu-fade-enter-from,
.context-menu-fade-leave-to {
  opacity: 0;
  transform: scale(0.95);
}
</style>
