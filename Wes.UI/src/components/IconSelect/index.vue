<template>
  <div class="icon-body">
    <el-input
      v-model="iconName"
      class="icon-search"
      clearable
      placeholder="请输入图标名称"
    >
      <template #suffix><i class="el-icon-search el-input__icon" /></template>
    </el-input>
    <div class="icon-list">
      <div class="list-container">
        <div
          v-for="(item, index) in iconList"
          class="icon-item-wrapper"
          :key="index"
          @click="selectedIcon(item)"
        >
          <div :class="['icon-item', { active: activeIcon === item }]">
            <i :class="'fa fa-fw fa-' + item" />
            <span>{{ item }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";

const props = defineProps({
  activeIcon: {
    type: String,
  },
});

const iconName = ref("");
const allIcons = ref([]);
const emit = defineEmits(["selected"]);

// 常用 FontAwesome 图标列表
const commonIcons = [
  "home", "user", "users", "cog", "cogs", "edit", "trash", "plus", "minus",
  "check", "times", "search", "arrow-left", "arrow-right", "arrow-up", "arrow-down",
  "file", "folder", "folder-open", "download", "upload", "save", "print", "copy",
  "cut", "paste", "undo", "redo", "refresh", "sync", "clock", "calendar", "bell",
  "envelope", "phone", "comment", "comments", "share", "link", "external-link",
  "star", "star-o", "heart", "heart-o", "bookmark", "bookmark-o", "flag",
  "camera", "image", "film", "music", "play", "pause", "stop", "volume-up",
  "volume-down", "volume-off", "eye", "eye-slash", "lock", "unlock", "key",
  "shield", "certificate", "warning", "info-circle", "question-circle", "check-circle",
  "times-circle", "exclamation-triangle", "ban", "power-off", "sign-out", "sign-in",
  "desktop", "laptop", "tablet", "mobile", "cloud", "database", "server", "wifi",
  "bluetooth", "usb", "battery-full", "battery-half", "battery-quarter", "battery-empty",
  "shopping-cart", "credit-card", "paypal", "bitcoin", "dollar", "euro", "yen",
  "pound", "rupee", "map-marker", "location-arrow", "globe", "plane", "car",
  "bus", "train", "ship", "bicycle", "wrench", "hammer", "tools", "cog",
  "cogs", "build", "rocket", "fire", "bolt", "moon", "sun", "cloud", "rain",
  "snowflake", "umbrella", "glass", "glass-half", "utensils", "coffee", "pizza",
  "birthday-cake", "leaf", "tree", "flower", "bug", "paw", "cat", "dog",
  "horse", "dove", "crow", "fish", "spider", "dragon", "ghost", "skull",
  "align-left", "align-center", "align-right", "align-justify", "indent", "outdent",
  "list-ul", "list-ol", "bold", "italic", "underline", "strikethrough", "subscript",
  "superscript", "code", "terminal", "file-code", "file-excel", "file-pdf",
  "file-word", "file-archive", "file-audio", "file-video", "file-image",
  "bars", "navicon", "ellipsis-h", "ellipsis-v", "th", "th-list", "square",
  "square-o", "circle", "circle-o", "check-square", "check-square-o", "remove",
  "chevron-left", "chevron-right", "chevron-up", "chevron-down", "angle-left",
  "angle-right", "angle-up", "angle-down", "double-angle-left", "double-angle-right",
  "double-angle-up", "double-angle-down", "caret-left", "caret-right", "caret-up",
  "caret-down", "expand", "compress", "expand-o", "compress-o", "step-forward",
  "step-backward", "fast-forward", "fast-backward", "backward", "forward"
];

allIcons.value = commonIcons.sort();

const iconList = computed(() => {
  if (!iconName.value) return allIcons.value;
  return allIcons.value.filter(icon =>
    icon.toLowerCase().includes(iconName.value.toLowerCase())
  );
});

// function filterIcons() {
//   iconList.value = icons;
//   if (iconName.value) {
//     iconList.value = icons.filter(
//       (item) => item.indexOf(iconName.value) !== -1
//     );
//   }
// }

function selectedIcon(name) {
  emit("selected", name);
  document.body.click();
}

function reset() {
  iconName.value = "";
}

defineExpose({
  reset,
});
</script>

<style lang='scss' scoped>
.icon-body {
  width: 100%;
  padding: 10px;
  .icon-search {
    position: relative;
    margin-bottom: 5px;
  }
  .icon-list {
    height: 200px;
    overflow: auto;
    .list-container {
      display: flex;
      flex-wrap: wrap;
      .icon-item-wrapper {
        flex: 0 0 33.333%;
        max-width: 33.333%;
        box-sizing: border-box;
        padding: 2px;
        cursor: pointer;
        .icon-item {
          display: flex;
          align-items: center;
          gap: 3px;
          padding: 3px;
          border-radius: 3px;
          &:hover {
            background: #ececec;
          }
          i {
            flex-shrink: 0;
            width: 12px;
            text-align: center;
            font-size: 12px;
            margin-right: 4px;
          }
          span {
            flex: 1;
            min-width: 0;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            font-size: 12px;
          }
        }
        .icon-item.active {
          background: #ececec;
        }
      }
    }
  }
}
</style>