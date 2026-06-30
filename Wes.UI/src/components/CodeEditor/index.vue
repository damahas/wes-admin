<template>
  <div
    class="code-editor-container"
    :class="{ fullscreen: fullscreen }"
    :style="containerStyle"
  >
    <!-- 编辑器工具栏 -->
    <div class="editor-toolbar">
      <div class="toolbar-left">
        <el-select
          v-model="currentLanguage"
          size="small"
          style="width: 120px"
          @change="handleLanguageChange"
        >
          <el-option label="SQL" value="sql" />
          <el-option label="JavaScript" value="javascript" />
          <el-option label="JSON" value="json" />
        </el-select>

        <el-button-group size="small" style="margin-left: 10px">
          <el-button @click="formatCode" :icon="MagicStick" title="格式化代码">
            格式化
          </el-button>
          <el-button @click="insertSnippet" :icon="DocumentAdd" title="插入代码片段">
            片段
          </el-button>
        </el-button-group>
      </div>

      <div class="toolbar-right">
        <el-button
          size="small"
          @click="toggleFullscreen"
          :icon="fullscreen ? 'FullScreen' : 'FullScreen'"
        >
          {{ fullscreen ? "退出全屏" : "全屏" }}
        </el-button>

        <el-tooltip
          :content="`行: ${cursorPosition.line}, 列: ${cursorPosition.column}`"
          placement="top"
        >
          <div class="cursor-info">
            行 {{ cursorPosition.line }}, 列 {{ cursorPosition.column }}
          </div>
        </el-tooltip>
      </div>
    </div>

    <!-- 编辑器主区域 - 简化版 -->
    <div class="editor-main-wrapper">
      <div class="editor-content">
        <textarea
          ref="textareaRef"
          v-model="codeValue"
          :placeholder="placeholder"
          class="editor-textarea"
          @input="handleInput"
          @keydown="handleKeyDown"
          @scroll="syncScroll"
          spellcheck="false"
        />

        <div class="editor-highlight" ref="highlightRef" :class="highlightStyleClass">
          <pre><code v-html="highlightedCode"></code></pre>
        </div>
      </div>

      <!-- 代码提示悬浮面板 -->
      <div
        v-if="showSuggestions && filteredSuggestions.length > 0"
        class="suggestion-dropdown"
        :style="suggestionPosition"
      >
        <div class="suggestion-list">
          <div
            v-for="(item, index) in filteredSuggestions"
            :key="index"
            class="suggestion-item"
            :class="{ active: activeSuggestionIndex === index }"
            @mousedown="insertSuggestion(item)"
            @mouseenter="activeSuggestionIndex = index"
          >
            <div class="suggestion-icon">
              <el-icon v-if="item.type === 'keyword'"><Key /></el-icon>
              <el-icon v-else-if="item.type === 'function'"><Position /></el-icon>
              <el-icon v-else-if="item.type === 'variable'"><DataLine /></el-icon>
              <el-icon v-else-if="item.type === 'custom'"><Star /></el-icon>
              <el-icon v-else><Document /></el-icon>
            </div>
            <div class="suggestion-text">
              <div class="suggestion-label">{{ item.label }}</div>
              <div class="suggestion-desc" v-if="item.description">
                {{ item.description }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 底部：状态栏 -->
    <div class="editor-statusbar">
      <div class="status-left">
        <el-tag
          size="small"
          :type="
            language === 'sql'
              ? 'success'
              : language === 'javascript'
              ? 'warning'
              : 'info'
          "
        >
          {{ language.toUpperCase() }}
        </el-tag>
        <span class="char-count">字符数: {{ charCount }}</span>
        <span class="line-count">行数: {{ lineCount }}</span>
      </div>
      <div class="status-right">
        <span v-if="hasUnsavedChanges" class="unsaved-status">未保存</span>
        <span v-else class="saved-status">已保存</span>
      </div>
    </div>

  <!-- 代码片段选择弹窗 -->
  <SnippetDialog
    v-model="snippetDialogVisible"
    :language="currentLanguage"
    :custom-sql-snippets="customSqlSnippets"
    :custom-js-snippets="customJsSnippets"
    :override-defaults="overrideSnippetDefaults"
    @insert="insertSelectedSnippet"
    @close="handleSnippetDialogClose"
  />
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted, onUnmounted, nextTick } from "vue";
import { useStore } from "vuex";
import {
  MagicStick,
  DocumentAdd,
  Key,
  Position,
  DataLine,
  Document,
  Star,
} from "@element-plus/icons-vue";
import { ElMessage } from "element-plus";
import hljs from "highlight.js/lib/core";
import sql from "highlight.js/lib/languages/sql";
import javascript from "highlight.js/lib/languages/javascript";
import json from "highlight.js/lib/languages/json";
import "highlight.js/styles/github.css";
import "highlight.js/styles/github-dark.css";
import SnippetDialog from "./SnippetDialog.vue";

const props = defineProps({
  modelValue: { type: String, default: "" },
  language: { type: String, default: "sql" },
  placeholder: { type: String, default: "请输入代码..." },
  readonly: { type: Boolean, default: false },
  height: { type: [String, Number], default: 400 },
  showLineNumbers: { type: Boolean, default: true },
  customSuggestions: { type: Array, default: () => [] },
  customSqlSnippets: { type: Array, default: () => [] },
  customJsSnippets: { type: Array, default: () => [] },
  overrideSnippetDefaults: { type: Boolean, default: false },
});

const emit = defineEmits(["update:modelValue", "update:language", "change", "save"]);

const store = useStore();
const isDark = computed(() => store.getters["system/isDark"]);

const codeValue = ref(props.modelValue);
const currentLanguage = ref(props.language);
const fullscreen = ref(false);
const textareaRef = ref(null);
const highlightRef = ref(null);
const cursorPosition = ref({ line: 1, column: 1 });
const showSuggestions = ref(false);
const activeSuggestionIndex = ref(0);
const snippetDialogVisible = ref(false);
const hasUnsavedChanges = ref(false);
const scrollbarWidth = ref(0);

const OFFSET_X = 10;
const OFFSET_Y = 20;
const SUGGESTION_ITEM_HEIGHT = 36;
const SUGGESTION_MAX_HEIGHT = 280;

const EDITOR_STYLES = {
  FONT_FAMILY: "'Courier New', monospace",
  FONT_SIZE: "14px",
  LINE_HEIGHT: 1.5,
  PADDING: 12,
  TAB_SIZE: 2,
};

const SUGGESTION_TYPES = {
  KEYWORD: "keyword",
  FUNCTION: "function",
  VARIABLE: "variable",
  CUSTOM: "custom",
};

const AUTO_SAVE_CONFIG = {
  ENABLED: true,
  DEBOUNCE_TIME: 2000,
  MIN_CHANGES: 1,
  SHOW_SUCCESS_MESSAGE: false, // 不显示保存成功消息，避免干扰
};

const REGEX = {
  WORD_CHAR: /[\w\.\(\)\[\]\{\}:]/,
  WORD_START: /\w/,
  TAB_REPLACEMENT: "  ",
};
const charCount = computed(() => codeValue.value.length);
const lineCount = computed(() => codeValue.value.split("\n").length);

const containerStyle = computed(() => ({
  "--editor-height": typeof props.height === "number" ? `${props.height}px` : props.height,
}));

const highlightStyleClass = computed(() => {
  return isDark.value ? "hljs-dark" : "hljs-light";
});

// 代码高亮（使用highlight.js）
const highlightedCode = computed(() => {
  if (!codeValue.value) return "";

  try {
    const languageMap = {
      sql: "sql",
      javascript: "javascript",
      json: "json",
    };

    const hljsLanguage = languageMap[currentLanguage.value] || "plaintext";

    if (hljsLanguage === "plaintext") {
      return escapeHtml(codeValue.value);
    }

    const result = hljs.highlight(codeValue.value, {
      language: hljsLanguage,
      ignoreIllegals: true,
    });

    return result.value;
  } catch (error) {
    console.error("代码高亮错误:", error);
    return escapeHtml(codeValue.value);
  }
});

function createSuggestionItem(label, type = SUGGESTION_TYPES.CUSTOM, description = "") {
  return { label, type, description };
}

const BASE_SUGGESTIONS = {
  sql: [
    createSuggestionItem("SELECT", SUGGESTION_TYPES.KEYWORD, "选择数据"),
    createSuggestionItem("FROM", SUGGESTION_TYPES.KEYWORD, "指定表名"),
    createSuggestionItem("WHERE", SUGGESTION_TYPES.KEYWORD, "条件筛选"),
    createSuggestionItem("INSERT INTO", SUGGESTION_TYPES.KEYWORD, "插入数据"),
    createSuggestionItem("UPDATE", SUGGESTION_TYPES.KEYWORD, "更新数据"),
    createSuggestionItem("DELETE FROM", SUGGESTION_TYPES.KEYWORD, "删除数据"),
    createSuggestionItem("JOIN", SUGGESTION_TYPES.KEYWORD, "表连接"),
    createSuggestionItem("ORDER BY", SUGGESTION_TYPES.KEYWORD, "排序"),
    createSuggestionItem("GROUP BY", SUGGESTION_TYPES.KEYWORD, "分组"),
    createSuggestionItem("LIMIT", SUGGESTION_TYPES.KEYWORD, "限制数量"),
    createSuggestionItem("COUNT()", SUGGESTION_TYPES.FUNCTION, "计数函数"),
    createSuggestionItem("SUM()", SUGGESTION_TYPES.FUNCTION, "求和函数"),
    createSuggestionItem("AVG()", SUGGESTION_TYPES.FUNCTION, "平均值函数"),
    createSuggestionItem("MAX()", SUGGESTION_TYPES.FUNCTION, "最大值函数"),
    createSuggestionItem("MIN()", SUGGESTION_TYPES.FUNCTION, "最小值函数"),
  ],
  javascript: [
    createSuggestionItem("function", SUGGESTION_TYPES.KEYWORD, "定义函数"),
    createSuggestionItem("const", SUGGESTION_TYPES.KEYWORD, "定义常量"),
    createSuggestionItem("let", SUGGESTION_TYPES.KEYWORD, "定义变量"),
    createSuggestionItem("if", SUGGESTION_TYPES.KEYWORD, "条件语句"),
    createSuggestionItem("for", SUGGESTION_TYPES.KEYWORD, "循环语句"),
    createSuggestionItem("while", SUGGESTION_TYPES.KEYWORD, "循环语句"),
    createSuggestionItem("return", SUGGESTION_TYPES.KEYWORD, "返回值"),
    createSuggestionItem("console.log()", SUGGESTION_TYPES.FUNCTION, "打印日志"),
    createSuggestionItem("JSON.stringify()", SUGGESTION_TYPES.FUNCTION, "对象转JSON字符串"),
    createSuggestionItem("JSON.parse()", SUGGESTION_TYPES.FUNCTION, "JSON字符串转对象"),
    createSuggestionItem("Array.map()", SUGGESTION_TYPES.FUNCTION, "数组映射"),
    createSuggestionItem("Array.filter()", SUGGESTION_TYPES.FUNCTION, "数组过滤"),
    createSuggestionItem("Array.reduce()", SUGGESTION_TYPES.FUNCTION, "数组归约"),
    createSuggestionItem("params", SUGGESTION_TYPES.VARIABLE, "传入参数对象"),
    createSuggestionItem("context", SUGGESTION_TYPES.VARIABLE, "执行上下文对象"),
  ],
};

// 代码提示列表（合并内置和自定义）
const suggestions = computed(() => {
  const baseSuggestions = BASE_SUGGESTIONS[currentLanguage.value] || [];

  // 合并自定义提示词
  const customSuggestions = props.customSuggestions.map((item) => {
    if (typeof item === "string") {
      return createSuggestionItem(item, SUGGESTION_TYPES.CUSTOM, "自定义提示词");
    }
    return createSuggestionItem(
      item.label,
      item.type || SUGGESTION_TYPES.CUSTOM,
      item.description || ""
    );
  });

  return [...baseSuggestions, ...customSuggestions];
});

// 过滤后的代码提示
const filteredSuggestions = computed(() => {
  const currentWord = getCurrentWord();
  if (!currentWord) return suggestions.value.slice(0, 10);

  const currentWordLower = currentWord.toLowerCase();

  // 首先匹配以当前单词开头的建议
  const startsWithMatches = suggestions.value.filter((item) =>
    item.label.toLowerCase().startsWith(currentWordLower)
  );

  // 然后匹配包含当前单词的建议
  const containsMatches = suggestions.value.filter(
    (item) =>
      !item.label.toLowerCase().startsWith(currentWordLower) &&
      item.label.toLowerCase().includes(currentWordLower)
  );

  // 合并结果，优先显示开头匹配的
  return [...startsWithMatches, ...containsMatches].slice(0, 10);
});

function calculateCursorPosition(textarea, text) {
  const cursorPos = textarea.selectionStart;
  const textBeforeCursor = text.substring(0, cursorPos);
  const lines = textBeforeCursor.split("\n");
  const lineIndex = lines.length - 1;
  const columnIndex = lines[lines.length - 1].length;

  // 测量实际的字符宽度和行高
  const tempSpan = document.createElement("span");
  tempSpan.style.position = "absolute";
  tempSpan.style.visibility = "hidden";
  tempSpan.style.whiteSpace = "pre";
  tempSpan.style.fontFamily = EDITOR_STYLES.FONT_FAMILY;
  tempSpan.style.fontSize = EDITOR_STYLES.FONT_SIZE;
  tempSpan.style.lineHeight = EDITOR_STYLES.LINE_HEIGHT.toString();
  tempSpan.textContent = "X";

  document.body.appendChild(tempSpan);
  const charWidth = tempSpan.offsetWidth;
  const lineHeight = parseInt(window.getComputedStyle(tempSpan).lineHeight) || 21;
  document.body.removeChild(tempSpan);

  // 计算光标在编辑器内容区域中的位置
  const cursorTopInContent = lineIndex * lineHeight + EDITOR_STYLES.PADDING;
  const cursorLeftInContent = columnIndex * charWidth + EDITOR_STYLES.PADDING;

  // 获取textarea的滚动位置
  const scrollTop = textarea.scrollTop;
  const scrollLeft = textarea.scrollLeft;

  // 计算光标相对于编辑器内容区域可见部分的位置
  const cursorTopVisible = cursorTopInContent - scrollTop;
  const cursorLeftVisible = cursorLeftInContent - scrollLeft;

  // 获取编辑器内容区域在视口中的位置
  const editorContent = textarea.closest(".editor-content");
  if (!editorContent) {
    return { cursorTopVisible, cursorLeftVisible };
  }

  const contentRect = editorContent.getBoundingClientRect();

  // 计算光标在视口中的绝对位置
  const cursorTopInViewport = contentRect.top + cursorTopVisible;
  const cursorLeftInViewport = contentRect.left + cursorLeftVisible;

  return { 
    cursorTopVisible, 
    cursorLeftVisible,
    cursorTopInViewport,
    cursorLeftInViewport 
  };
};

function clampPosition(value, min, max) {
  return Math.max(min, Math.min(max, value));
}

function calculateBestHorizontalPositionInViewport(cursorLeftInViewport, viewportWidth, suggestionWidth) {
  const preferredLeft = cursorLeftInViewport + OFFSET_X;
  
  if (preferredLeft + suggestionWidth <= viewportWidth) {
    return preferredLeft;
  }

  const leftPosition = cursorLeftInViewport - suggestionWidth - OFFSET_X;
  if (leftPosition >= 0) {
    return leftPosition;
  }

  return Math.max(0, viewportWidth - suggestionWidth);
};

const calculateBestVerticalPositionInViewport = (cursorTopInViewport, viewportHeight, suggestionHeight) => {
  const preferredTop = cursorTopInViewport + OFFSET_Y;
  
  if (preferredTop + suggestionHeight <= viewportHeight) {
    return preferredTop;
  }

  const topPosition = cursorTopInViewport - suggestionHeight - OFFSET_Y;
  if (topPosition >= 0) {
    return topPosition;
  }

  return Math.max(0, viewportHeight - suggestionHeight);
};

const suggestionPosition = ref({});

function updateSuggestionPosition() {
  if (!textareaRef.value || !showSuggestions.value) {
    suggestionPosition.value = {};
    return;
  }

  const textarea = textareaRef.value;
  const text = codeValue.value;
  const { cursorTopInViewport, cursorLeftInViewport } = calculateCursorPosition(textarea, text);

  const editorContent = textarea.closest(".editor-content");
  if (!editorContent) {
    console.error("找不到 .editor-content 容器");
    suggestionPosition.value = {};
    return;
  }

  const viewportHeight = window.innerHeight;
  const viewportWidth = window.innerWidth;
  const suggestionHeight = Math.min(
    filteredSuggestions.value.length * SUGGESTION_ITEM_HEIGHT,
    SUGGESTION_MAX_HEIGHT
  );
  const suggestionWidth = Math.min(Math.max(220, viewportWidth * 0.4), 400);

  let bestLeft = calculateBestHorizontalPositionInViewport(
    cursorLeftInViewport,
    viewportWidth,
    suggestionWidth
  );

  let bestTop = calculateBestVerticalPositionInViewport(
    cursorTopInViewport,
    viewportHeight,
    suggestionHeight
  );

  bestTop = clampPosition(bestTop, 0, viewportHeight - Math.min(suggestionHeight, 100));
  bestLeft = clampPosition(bestLeft, 0, viewportWidth - Math.min(suggestionWidth, 100));

  suggestionPosition.value = {
    position: "fixed",
    top: `${bestTop}px`,
    left: `${bestLeft}px`,
    maxHeight: `${SUGGESTION_MAX_HEIGHT}px`,
    zIndex: 99999,
  };
};

// 监听代码提示显示状态变化
watch([showSuggestions, filteredSuggestions], () => {
  if (showSuggestions.value && filteredSuggestions.value.length > 0) {
    nextTick(() => {
      updateSuggestionPosition();
    });
  }
});

function escapeHtml(text) {
  const div = document.createElement("div");
  div.textContent = text;
  return div.innerHTML;
}

function getCurrentWord() {
  if (!textareaRef.value) return "";

  const text = codeValue.value;
  const cursorPos = textareaRef.value.selectionStart;
  let start = cursorPos;
  let end = cursorPos;

  // 向前查找单词开始位置
  while (start > 0) {
    const char = text[start - 1];
    if (REGEX.WORD_CHAR.test(char)) {
      start--;
    } else {
      break;
    }
  }

  // 向后查找单词结束位置
  while (end < text.length) {
    const char = text[end];
    if (REGEX.WORD_CHAR.test(char)) {
      end++;
    } else {
      break;
    }
  }

  return text.substring(start, end);
};

const updateCodeSuggestions = () => {
  const word = getCurrentWord();
  if (word.length >= 2) {
    showSuggestions.value = true;
    activeSuggestionIndex.value = 0;
    nextTick(() => updateSuggestionPosition());
  } else {
    showSuggestions.value = false;
  }
};

const syncHighlightAndScroll = () => {
  nextTick(() => {
    syncScroll();
    if (highlightRef.value) {
      void highlightRef.value.offsetHeight; // 触发重排
    }
  });
};

// 计算滚动条宽度
const getScrollbarWidth = () => {
  const div = document.createElement("div");
  div.style.width = "100px";
  div.style.height = "100px";
  div.style.overflow = "scroll";
  div.style.position = "absolute";
  div.style.top = "-9999px";
  document.body.appendChild(div);
  const scrollbarWidth = div.offsetWidth - div.clientWidth;
  document.body.removeChild(div);
  return scrollbarWidth;
};

let saveTimeout = null;
let changeCount = 0;

function triggerAutoSave() {
  if (!AUTO_SAVE_CONFIG.ENABLED) return;
  
  // 只要有变化就触发保存，不检查hasUnsavedChanges状态
  if (changeCount >= AUTO_SAVE_CONFIG.MIN_CHANGES) {
    emit("save", codeValue.value);
    hasUnsavedChanges.value = false;
    changeCount = 0;
    
    // 可选的保存成功提示（默认不显示）
    if (AUTO_SAVE_CONFIG.SHOW_SUCCESS_MESSAGE) {
      ElMessage.success("代码已自动保存");
    }
  }
}

function resetSaveTimer() {
  if (saveTimeout) clearTimeout(saveTimeout);
  
  if (AUTO_SAVE_CONFIG.ENABLED) {
    saveTimeout = setTimeout(triggerAutoSave, AUTO_SAVE_CONFIG.DEBOUNCE_TIME);
  }
}

function handleInput() {
  emit("update:modelValue", codeValue.value);
  emit("change", codeValue.value);
  hasUnsavedChanges.value = true;
  changeCount++;

  resetSaveTimer();
  updateCodeSuggestions();
  updateCursorPosition();
  syncHighlightAndScroll();
}

function handleSuggestionNavigation(event) {
  if (!showSuggestions.value) return false;

  const suggestionCount = filteredSuggestions.value.length;

  switch (event.key) {
    case "ArrowDown":
      event.preventDefault();
      activeSuggestionIndex.value = (activeSuggestionIndex.value + 1) % suggestionCount;
      return true;
    case "ArrowUp":
      event.preventDefault();
      activeSuggestionIndex.value = (activeSuggestionIndex.value - 1 + suggestionCount) % suggestionCount;
      return true;
    case "Enter":
      if (suggestionCount > 0) {
        event.preventDefault();
        insertSuggestion(filteredSuggestions.value[activeSuggestionIndex.value]);
        return true;
      }
      break;
    case "Escape":
      event.preventDefault();
      showSuggestions.value = false;
      return true;
  }

  return false;
}

function handleKeyDown(event) {
  // Tab键处理（插入空格）
  if (event.key === "Tab") {
    event.preventDefault();
    insertText(REGEX.TAB_REPLACEMENT);
    return;
  }

  // 方向键导航代码提示
  if (handleSuggestionNavigation(event)) {
    return;
  }

  // 更新光标位置
  updateCursorPosition();
};

function syncScroll() {
  if (textareaRef.value && highlightRef.value) {
    const scrollTop = textareaRef.value.scrollTop;
    const scrollLeft = textareaRef.value.scrollLeft;

    highlightRef.value.scrollTop = scrollTop;
    highlightRef.value.scrollLeft = scrollLeft;

    if (showSuggestions.value) {
      nextTick(() => updateSuggestionPosition());
    }
  }
}

function updateCursorPosition() {
  if (!textareaRef.value) return;

  const text = codeValue.value;
  const cursorPos = textareaRef.value.selectionStart;
  const textBeforeCursor = text.substring(0, cursorPos);
  const lines = textBeforeCursor.split("\n");
  const line = lines.length;
  const column = lines[lines.length - 1].length + 1;

  cursorPosition.value = { line, column };
}

function replaceSelectedText(text) {
  if (!textareaRef.value) return;

  const start = textareaRef.value.selectionStart;
  const end = textareaRef.value.selectionEnd;
  const before = codeValue.value.substring(0, start);
  const after = codeValue.value.substring(end);

  codeValue.value = before + text + after;

  nextTick(() => {
    textareaRef.value.selectionStart = textareaRef.value.selectionEnd = start + text.length;
    textareaRef.value.focus();
  });

  handleInput();
}

function insertText(text) {
  replaceSelectedText(text);
}

function findWordBoundaries(text, cursorPos) {
  let start = cursorPos;
  let end = cursorPos;

  while (start > 0 && REGEX.WORD_START.test(text[start - 1])) {
    start--;
  }

  while (end < text.length && REGEX.WORD_START.test(text[end])) {
    end++;
  }

  return { start, end };
}

function replaceWordAtCursor(textarea, text, suggestionLabel) {
  const cursorPos = textarea.selectionStart;
  const { start, end } = findWordBoundaries(text, cursorPos);
  const before = text.substring(0, start);
  const after = text.substring(end);

  codeValue.value = before + suggestionLabel + after;

  nextTick(() => {
    textarea.selectionStart = textarea.selectionEnd = start + suggestionLabel.length;
    textarea.focus();
  });
}

function insertSuggestion(suggestion) {
  if (!textareaRef.value) return;

  const textarea = textareaRef.value;
  const text = codeValue.value;
  const currentWord = getCurrentWord();

  if (!currentWord) {
    insertText(suggestion.label);
  } else {
    const suggestionLower = suggestion.label.toLowerCase();
    const currentWordLower = currentWord.toLowerCase();

    if (suggestionLower.startsWith(currentWordLower)) {
      replaceWordAtCursor(textarea, text, suggestion.label);
    } else {
      insertText(suggestion.label);
    }
  }

  showSuggestions.value = false;
  emit("update:modelValue", codeValue.value);
  emit("change", codeValue.value);
  hasUnsavedChanges.value = true;
  updateCursorPosition();
}

function formatJsonCode() {
  try {
    const obj = JSON.parse(codeValue.value);
    codeValue.value = JSON.stringify(obj, null, 2);
    ElMessage.success("JSON格式化成功");
    return true;
  } catch (e) {
    ElMessage.error("JSON格式错误，无法格式化");
    return false;
  }
}

function formatJavaScriptCode() {
  const lines = codeValue.value.split("\n");
  let indentLevel = 0;
  const formattedLines = lines.map((line) => {
    const trimmed = line.trim();

    if (trimmed.startsWith("}") || trimmed.startsWith("]") || trimmed.includes("} else")) {
      indentLevel = Math.max(0, indentLevel - 1);
    }

    const indented = "  ".repeat(indentLevel) + trimmed;

    if (trimmed.endsWith("{") || trimmed.endsWith("[") || trimmed.endsWith("(") ||
        trimmed.includes("if (") || trimmed.includes("for (") || trimmed.includes("while (")) {
      indentLevel++;
    }

    return indented;
  });

  codeValue.value = formattedLines.join("\n");
  ElMessage.success("JavaScript格式化完成");
  return true;
}

function formatSqlCode() {
  const sqlKeywords = [
    "SELECT", "FROM", "WHERE", "INSERT", "UPDATE", "DELETE", "CREATE", "ALTER", "DROP",
    "JOIN", "LEFT", "RIGHT", "INNER", "OUTER", "ON", "AS", "AND", "OR", "NOT", "IN",
    "LIKE", "BETWEEN", "IS", "NULL", "ORDER", "BY", "GROUP", "HAVING", "LIMIT", "OFFSET",
    "UNION", "ALL", "DISTINCT", "VALUES", "SET",
  ];

  const keywordPattern = new RegExp(`\\b(${sqlKeywords.join("|")})\\b`, "gi");
  const formatted = codeValue.value
    .replace(keywordPattern, "\n$1")
    .replace(/,/g, ",\n  ")
    .replace(/\n\s*\n/g, "\n")
    .trim();

  codeValue.value = formatted;
  ElMessage.success("SQL格式化完成");
  return true;
}

function formatCode() {
  let success = false;

  switch (currentLanguage.value) {
    case "json":
      success = formatJsonCode();
      break;
    case "javascript":
      success = formatJavaScriptCode();
      break;
    case "sql":
      success = formatSqlCode();
      break;
    default:
      ElMessage.warning(`暂不支持${currentLanguage.value}语言的格式化`);
      return;
  }

  if (success) {
    handleInput();
  }
}

function toggleFullscreen() {
  fullscreen.value = !fullscreen.value;
  document.body.style.overflow = fullscreen.value ? "hidden" : "";
}

function handleLanguageChange(newLang) {
  currentLanguage.value = newLang;
  emit("update:language", newLang);
}

function insertSnippet() {
  snippetDialogVisible.value = true;
}

function insertSelectedSnippet(snippet) {
  insertText(snippet.code);
  snippetDialogVisible.value = false;
  ElMessage.success(`已插入${snippet.name}片段`);
}

function handleSnippetDialogClose() {
  snippetDialogVisible.value = false;
}

watch(() => props.modelValue, (newVal) => {
  if (newVal !== codeValue.value) {
    codeValue.value = newVal;
    hasUnsavedChanges.value = false;
  }
});

watch(() => props.language, (newVal) => {
  currentLanguage.value = newVal;
});





hljs.registerLanguage("sql", sql);
hljs.registerLanguage("javascript", javascript);
hljs.registerLanguage("json", json);

onMounted(() => {
  updateCursorPosition();
  scrollbarWidth.value = getScrollbarWidth();

  const handleGlobalClick = (event) => {
    if (showSuggestions.value && textareaRef.value) {
      const suggestionDropdown = document.querySelector(".suggestion-dropdown");
      const isClickInsideSuggestion = suggestionDropdown && suggestionDropdown.contains(event.target);
      const isClickInsideTextarea = textareaRef.value.contains(event.target);

      if (!isClickInsideSuggestion && !isClickInsideTextarea) {
        showSuggestions.value = false;
      }
    }
  };

  const handleResize = () => {
    if (showSuggestions.value) {
      nextTick(() => updateSuggestionPosition());
    }
  };

  document.addEventListener("click", handleGlobalClick);
  window.addEventListener("resize", handleResize);

  window._codeEditorGlobalClickHandler = handleGlobalClick;
  window._codeEditorResizeHandler = handleResize;

  nextTick(() => {
    syncScroll();
    
    if (highlightRef.value) {
      highlightRef.value.style.visibility = "hidden";
      void highlightRef.value.offsetHeight;
      highlightRef.value.style.visibility = "";
    }

    if (textareaRef.value) {
      textareaRef.value.scrollTop = 0;
      textareaRef.value.scrollLeft = 0;
    }
    syncScroll();
  });
});

// 暴露方法供外部调用
defineExpose({
  insertText,
  getValue: () => modelValue.value,
  setValue: (val) => {
    modelValue.value = val;
  },
});


onUnmounted(() => {
  if (fullscreen.value) {
    document.body.style.overflow = "";
  }

  if (saveTimeout) {
    clearTimeout(saveTimeout);
    saveTimeout = null;
  }

  if (window._codeEditorGlobalClickHandler) {
    document.removeEventListener("click", window._codeEditorGlobalClickHandler);
    delete window._codeEditorGlobalClickHandler;
  }

  if (window._codeEditorResizeHandler) {
    window.removeEventListener("resize", window._codeEditorResizeHandler);
    delete window._codeEditorResizeHandler;
  }
});
</script>

<style scoped>
.code-editor-container {
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background-color: var(--bg-card);
  overflow: hidden;
  position: relative;
  display: flex;
  flex-direction: column;
  height: var(--editor-height, 400px);
}

.code-editor-container.fullscreen {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 9999;
  background: var(--bg-card);
  border: none;
  border-radius: 0;
  height: 100vh !important;
  --editor-height: 100vh !important;
  max-height: 100vh !important;
  overflow: hidden;
}

.code-editor-container.fullscreen .editor-main-wrapper {
  flex: 1 1 auto;
  min-height: 0;
}

.code-editor-container.fullscreen .editor-content {
  flex: 1;
  min-height: 0;
}

.code-editor-container.fullscreen .editor-toolbar,
.code-editor-container.fullscreen .editor-statusbar {
  flex-shrink: 0;
}

/* 工具栏 */
.editor-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background-color: var(--bg-hover);
  border-bottom: 1px solid var(--border-color);
  flex-shrink: 0;
}

.toolbar-left,
.toolbar-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

.cursor-info {
  padding: 2px 8px;
  background-color: var(--theme-color-light);
  border-radius: 3px;
  font-size: 12px;
  color: var(--theme-color);
  font-family: "Courier New", monospace;
}

/* 编辑器主区域 */
.editor-main-wrapper {
  position: relative;
  flex: 1;
  overflow: hidden;
  display: flex;
}

.editor-content {
  flex: 1;
  position: relative;
  overflow: auto;
}

/* 文本输入区域 */
.editor-textarea {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  margin: 0;
  padding: 12px;
  font-family: "Courier New", monospace;
  font-size: 14px;
  line-height: 1.5;
  tab-size: 2;
  white-space: pre;
  overflow-wrap: normal;
  border: none;
  outline: none;
  resize: none;
  background: transparent;
  box-sizing: border-box;
  color: transparent;
  caret-color: var(--text-primary);
  z-index: 2;
  -webkit-text-fill-color: transparent;
}

/* 高亮显示区域 */
.editor-highlight {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  margin: 0;
  padding: 12px;
  font-family: "Courier New", monospace;
  font-size: 14px;
  line-height: 1.5;
  box-sizing: border-box;
  pointer-events: none;
  z-index: 1;
  user-select: none;
  overflow: hidden;
}

.editor-highlight pre {
  margin: 0;
  padding: 0;
  overflow: visible;
}

.editor-highlight code {
  display: block;
  font-family: "Courier New", monospace;
  font-size: 14px;
  line-height: 1.5;
  white-space: pre;
}

/* 高亮样式覆盖 */
.editor-highlight :deep(.hljs) {
  background: transparent !important;
  padding: 0 !important;
}

/* 选中文本样式 */
.editor-textarea::selection {
  background: rgba(64, 158, 255, 0.3);
}

/* 代码提示悬浮下拉框 */
.suggestion-dropdown {
  position: fixed;
  background: var(--bg-card);
  backdrop-filter: blur(10px);
  border: 1px solid var(--border-color-light);
  border-radius: 8px;
  box-shadow: 0 8px 32px var(--shadow-color), 0 2px 8px var(--shadow-color);
  z-index: 99999;
  max-height: 280px;
  overflow-y: auto;
  overflow-x: auto;
  min-width: 220px;
  max-width: 400px;
  width: auto;
}

.suggestion-dropdown::-webkit-scrollbar {
  width: 6px;
}

.suggestion-dropdown::-webkit-scrollbar-track {
  background: var(--scrollbar-track);
  border-radius: 3px;
}

.suggestion-dropdown::-webkit-scrollbar-thumb {
  background: var(--scrollbar-thumb);
  border-radius: 3px;
}

.suggestion-dropdown::-webkit-scrollbar-thumb:hover {
  background: var(--text-placeholder);
}

.suggestion-list {
  padding: 6px 0;
}

.suggestion-item {
  display: flex;
  align-items: center;
  padding: 6px 12px;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 12px;
  border-radius: 3px;
  margin: 0 4px;
}

.suggestion-item:hover,
.suggestion-item.active {
  background-color: var(--bg-active);
  box-shadow: inset 0 0 0 1px var(--theme-color);
}

.suggestion-icon {
  margin-right: 8px;
  color: var(--text-primary);
  font-size: 12px;
  flex-shrink: 0;
  opacity: 0.9;
}

.suggestion-text {
  flex: 1;
  min-width: 0;
}

.suggestion-label {
  font-weight: 500;
  color: var(--text-title);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  font-size: 12px;
  line-height: 1.4;
}

.suggestion-desc {
  font-size: 10px;
  color: var(--text-secondary);
  margin-top: 1px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.3;
  opacity: 0.7;
}

/* 状态栏 */
.editor-statusbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 6px 12px;
  background-color: var(--bg-hover);
  border-top: 1px solid var(--border-color);
  font-size: 12px;
  color: var(--text-secondary);
  flex-shrink: 0;
}

.status-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.status-right .saved-status {
  color: var(--color-success);
}

.status-right .unsaved-status {
  color: var(--color-warning);
}











/* 滚动条样式 */
.editor-content::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

.editor-content::-webkit-scrollbar-track {
  background: transparent;
}

.editor-content::-webkit-scrollbar-thumb {
  background-color: var(--scrollbar-thumb);
  border-radius: 4px;
}

/* 响应式调整 */
@media (max-width: 768px) {
  .editor-main-wrapper {
    flex-direction: column;
  }

  .suggestion-dropdown {
    max-width: calc(100% - 20px);
    left: 10px !important;
    right: 10px;
  }

  .editor-toolbar {
    flex-direction: column;
    gap: 8px;
    align-items: stretch;
  }

  .toolbar-left,
  .toolbar-right {
    justify-content: space-between;
  }
}

/* 暗黑主题样式 */
:global(.dark) .code-editor-container {
  border-color: var(--border-color);
  background-color: var(--bg-card);
}

:global(.dark) .code-editor-container.fullscreen {
  background: var(--bg-card);
}

:global(.dark) .editor-toolbar {
  background-color: var(--bg-hover);
  border-bottom-color: var(--border-color);
}

:global(.dark) .cursor-info {
  background-color: var(--theme-color-light);
  color: var(--theme-color);
}

:global(.dark) .editor-textarea {
  caret-color: var(--text-primary);
}

:global(.dark) .editor-textarea::selection {
  background: rgba(64, 158, 255, 0.5);
}

:global(.dark) .suggestion-dropdown {
  background: var(--bg-card);
  border-color: var(--border-color-light);
  box-shadow: 0 8px 32px var(--shadow-color), 0 2px 8px var(--shadow-color);
}

:global(.dark) .suggestion-dropdown::-webkit-scrollbar-track {
  background: var(--scrollbar-track);
}

:global(.dark) .suggestion-dropdown::-webkit-scrollbar-thumb {
  background: var(--scrollbar-thumb);
}

:global(.dark) .suggestion-dropdown::-webkit-scrollbar-thumb:hover {
  background: var(--text-placeholder);
}

:global(.dark) .suggestion-item:hover,
:global(.dark) .suggestion-item.active {
  background-color: var(--bg-active);
  box-shadow: inset 0 0 0 1px var(--theme-color);
}

:global(.dark) .suggestion-icon {
  color: var(--text-primary);
}

:global(.dark) .suggestion-label {
  color: var(--text-title);
}

:global(.dark) .suggestion-desc {
  color: var(--text-primary);
}

:global(.dark) .editor-statusbar {
  background-color: var(--bg-hover);
  border-top-color: var(--border-color);
  color: var(--text-secondary);
}

:global(.dark) .status-right .saved-status {
  color: var(--color-success);
}

:global(.dark) .status-right .unsaved-status {
  color: var(--color-warning);
}



:global(.dark) .editor-content::-webkit-scrollbar-thumb {
  background-color: var(--scrollbar-thumb);
}

/* highlight.js 暗黑主题支持 */
.editor-highlight.hljs-dark :deep(.hljs) {
  background: transparent !important;
  color: #c9d1d9 !important;
}

.editor-highlight.hljs-dark :deep(.hljs-keyword),
.editor-highlight.hljs-dark :deep(.hljs-selector-tag),
.editor-highlight.hljs-dark :deep(.hljs-literal) {
  color: #ff7b72 !important;
}

.editor-highlight.hljs-dark :deep(.hljs-string),
.editor-highlight.hljs-dark :deep(.hljs-doctag) {
  color: #a5d6ff !important;
}

.editor-highlight.hljs-dark :deep(.hljs-title),
.editor-highlight.hljs-dark :deep(.hljs-section),
.editor-highlight.hljs-dark :deep(.hljs-type) {
  color: #d2a8ff !important;
}

.editor-highlight.hljs-dark :deep(.hljs-number),
.editor-highlight.hljs-dark :deep(.hljs-params) {
  color: #79c0ff !important;
}

.editor-highlight.hljs-dark :deep(.hljs-comment) {
  color: #8b949e !important;
}

.editor-highlight.hljs-dark :deep(.hljs-built_in),
.editor-highlight.hljs-dark :deep(.hljs-builtin-name) {
  color: #ffa657 !important;
}
</style>
