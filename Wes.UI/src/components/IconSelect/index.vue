<template>
  <div class="icon-body">
    <el-input
      v-model="iconName"
      class="icon-search"
      clearable
      :placeholder="t('common.searchIcon')"
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
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = defineProps({
  activeIcon: {
    type: String,
  },
});

const iconName = ref("");
const allIcons = ref([]);
const emit = defineEmits(["selected"]);

// 常用 FontAwesome 7 图标列表
const commonIcons = [
  // ── 导航 & 布局 ──
  "home", "bars", "ellipsis-h", "ellipsis-vertical", "th", "th-large", "th-list",
  "sitemap", "stream", "grip-horizontal", "grip-vertical", "grip-lines", "columns",

  // ── 箭头 & 方向 ──
  "arrow-left", "arrow-right", "arrow-up", "arrow-down",
  "arrow-trend-up", "arrow-trend-down",
  "chevron-left", "chevron-right", "chevron-up", "chevron-down",
  "angle-left", "angle-right", "angle-up", "angle-down",
  "angles-left", "angles-right", "angles-up", "angles-down",
  "caret-left", "caret-right", "caret-up", "caret-down",
  "location-arrow", "reply", "reply-all", "share", "share-nodes",
  "external-link", "link", "unlink", "up-right-from-square",

  // ── 通用操作 ──
  "plus", "plus-circle", "circle-plus",
  "minus", "minus-circle", "circle-minus",
  "xmark", "circle-xmark", "check", "circle-check",
  "edit", "pen-to-square", "trash", "trash-can",
  "copy", "paste", "clipboard", "scissors",
  "download", "upload", "print", "save", "floppy-disk",
  "undo", "redo", "sync", "refresh", "repeat", "rotate",
  "expand", "compress", "maximize", "minimize",
  "up-right-and-down-left-from-center", "down-left-and-up-right-to-center",
  "power-off", "sign-out", "sign-in", "delete-left", "backspace",

  // ── 用户 & 人员 ──
  "user", "user-large", "users", "user-plus", "user-minus",
  "user-check", "user-xmark", "user-lock", "user-gear", "user-group",
  "user-secret", "user-tie", "user-graduate", "user-md", "user-nurse", "user-injured",
  "person", "address-book", "id-card", "id-badge", "id-card-clip",

  // ── 文件 & 文档 ──
  "file", "file-lines", "folder", "folder-open", "folder-plus", "folder-minus",
  "folder-tree", "file-code", "file-pdf", "file-excel", "file-word",
  "file-powerpoint", "file-image", "file-video", "file-audio", "file-zipper",
  "file-csv", "file-import", "file-export", "file-download", "file-upload",
  "clipboard", "clipboard-check", "clipboard-list",
  "paperclip", "note-sticky", "bookmark",

  // ── 状态 & 反馈 ──
  "info-circle", "circle-info", "question-circle", "circle-question",
  "exclamation-circle", "circle-exclamation", "triangle-exclamation",
  "check-circle", "circle-check", "times-circle", "circle-xmark",
  "ban", "warning", "circle-notch", "spinner",

  // ── 通信 ──
  "envelope", "envelope-open", "phone", "phone-flip",
  "comment", "comments", "message", "bell", "bell-slash", "paper-plane",
  "bullhorn", "envelope-circle-check",

  // ── 业务 & 财务 ──
  "shopping-cart", "cart-plus", "cart-shopping",
  "credit-card", "wallet", "piggy-bank", "receipt", "sack-dollar",
  "tag", "tags", "bookmark", "trophy",
  "star", "star-half", "star-half-stroke",
  "heart", "certificate", "award", "medal", "crown", "gem", "gift",

  // ── 安全 & 权限 ──
  "lock", "lock-open", "unlock", "key",
  "shield", "shield-halved", "eye", "eye-slash", "fingerprint", "user-shield",

  // ── 时间 & 日期 ──
  "clock", "calendar", "calendar-days",
  "calendar-check", "calendar-plus", "calendar-minus", "calendar-xmark",
  "alarm-clock", "stopwatch", "hourglass", "history",

  // ── 设备 & 技术 ──
  "desktop", "laptop", "tablet", "mobile", "mobile-screen-button",
  "server", "database", "cloud", "hard-drive",
  "wifi", "bluetooth", "usb", "microchip", "memory",
  "code", "code-branch", "code-commit", "code-merge", "code-fork",
  "terminal", "network-wired",
  "sd-card", "sim-card", "signal", "broadcast-tower", "satellite", "satellite-dish",

  // ── 媒体 ──
  "image", "images", "camera", "video", "film",
  "music", "headphones",
  "play", "pause", "stop", "eject",
  "backward", "forward", "step-backward", "step-forward",
  "fast-backward", "fast-forward",
  "volume-up", "volume-down", "volume-off", "volume-xmark",
  "microphone", "microphone-slash", "photo-film",

  // ── 文本 & 编辑器 ──
  "bold", "italic", "underline", "strikethrough",
  "subscript", "superscript", "paragraph", "heading",
  "font", "text-height", "text-width", "text-slash",
  "align-left", "align-center", "align-right", "align-justify",
  "indent", "outdent", "list", "list-ol", "list-ul",
  "table", "table-columns", "spell-check", "highlighter", "eraser",

  // ── 图形 & UI ──
  "square", "circle", "dot-circle", "square-full",
  "square-check", "square-plus", "square-minus",
  "toggle-on", "toggle-off",
  "magnifying-glass", "search-plus", "search-minus",
  "filter", "sliders", "crosshairs",
  "palette", "brush", "paintbrush",
  "gear", "gears", "wrench", "screwdriver", "tools", "toolbox",

  // ── 图表 & 数据 ──
  "chart-bar", "chart-pie", "chart-line", "chart-area", "chart-simple",
  "chart-column", "chart-gantt", "chart-diagram",
  "table-list", "calculator",

  // ── 地图 & 位置 ──
  "map-marker", "map-pin", "map", "map-location-dot",
  "globe", "compass", "flag", "flag-checkered",

  // ── 交通 ──
  "car", "bus", "train", "plane", "ship", "bicycle", "motorcycle",
  "truck", "truck-fast", "taxi", "rocket", "helicopter",

  // ── 天气 & 自然 ──
  "sun", "moon", "cloud", "cloud-sun", "cloud-moon", "cloud-rain", "cloud-bolt",
  "bolt", "fire", "leaf", "tree", "snowflake",
  "water", "wind", "umbrella", "umbrella-beach", "rainbow", "mountain",

  // ── 购物 & 支付 ──
  "shop", "store", "basket-shopping", "shopping-bag",
  "dollar-sign", "euro-sign", "sterling-sign", "yen-sign", "rupee-sign", "btc",
  "paypal", "money-bill", "money-bill-wave", "money-check", "coins",

  // ── 健康 & 医疗 ──
  "heartbeat", "stethoscope", "hospital", "syringe", "pills", "capsules",
  "kit-medical", "bandage", "x-ray", "tooth", "brain", "dna", "lungs", "virus",

  // ── 食物 & 饮品 ──
  "mug-saucer", "wine-glass", "beer-mug-empty",
  "birthday-cake", "pizza-slice", "hamburger",
  "cookie", "cookie-bite", "ice-cream", "candy-cane",
  "lemon", "carrot", "egg", "cheese", "bread-slice", "hotdog",

  // ── 趣味 & 其他 ──
  "bug", "skull", "ghost", "spider", "dragon", "crow",
  "fish", "cat", "dog", "horse", "paw", "dove", "hippo", "frog", "otter",
  "feather", "bone", "hat-wizard", "mask", "wand-magic-sparkles",
  "dice", "chess", "gamepad", "puzzle-piece",
  "lightbulb", "magnet", "anchor", "life-ring", "binoculars",
  "bomb", "poo", "fire-extinguisher",

  // ── 品牌 (常用) ──
  "github", "gitlab", "git-alt", "bitbucket", "docker",
  "chrome", "firefox", "safari", "edge",
  "apple", "android", "linux", "windows",
  "html5", "css3-alt", "js", "python", "java", "php",
  "react", "vuejs", "angular", "node-js", "npm",
  "sass", "less", "bootstrap", "markdown",
  "figma", "sketch", "yarn",
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