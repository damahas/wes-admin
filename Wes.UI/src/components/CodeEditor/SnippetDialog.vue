<template>
  <el-dialog
    v-model="visible"
    :title="`${currentLanguageName}代码片段`"
    width="600px"
    :close-on-click-modal="false"
    custom-class="snippet-dialog"
  >
    <div class="snippet-list">
      <!-- 显示当前语言的片段 -->
      <div
        v-for="(snippet, index) in currentSnippets"
        :key="index"
        class="snippet-item"
        @click="handleSnippetClick(snippet)"
      >
        <h5>{{ snippet.name }}</h5>
        <p>{{ snippet.description }}</p>
        <pre class="snippet-code" v-html="highlightSnippetCode(snippet.code, currentLanguage)"></pre>
      </div>
      
      <!-- 如果没有片段 -->
      <div v-if="currentSnippets.length === 0" class="snippet-empty">
        <el-empty description="当前语言暂无代码片段" :image-size="80" />
      </div>
    </div>

    <template #footer>
      <el-button @click="close">取消</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, computed } from "vue";
import hljs from "highlight.js/lib/core";
import sql from "highlight.js/lib/languages/sql";
import javascript from "highlight.js/lib/languages/javascript";
import json from "highlight.js/lib/languages/json";

const props = defineProps({
  // 是否显示弹窗
  modelValue: {
    type: Boolean,
    default: false,
  },
  // 当前语言
  language: {
    type: String,
    default: "sql",
  },
  // 自定义SQL代码片段（可选）
  customSqlSnippets: {
    type: Array,
    default: () => [],
  },
  // 自定义JavaScript代码片段（可选）
  customJsSnippets: {
    type: Array,
    default: () => [],
  },
  // 是否覆盖默认片段（默认为false，即合并自定义和默认片段）
  overrideDefaults: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["update:modelValue", "insert", "close"]);

const visible = computed({
  get() {
    return props.modelValue;
  },
  set(value) {
    emit("update:modelValue", value);
  },
});

const currentLanguage = ref(props.language);

// 默认SQL代码片段数据
const defaultSqlSnippets = [
  {
    name: "基础SELECT查询",
    description: "简单的SELECT查询语句",
    code: "SELECT * FROM table_name WHERE condition = :param ORDER BY id DESC LIMIT 10",
    keywords: ["select", "from", "where", "order by", "limit"],
  },
  {
    name: "INSERT语句",
    description: "插入数据语句",
    code: "INSERT INTO table_name (column1, column2) VALUES (:value1, :value2)",
    keywords: ["insert", "into", "values"],
  },
  {
    name: "UPDATE语句",
    description: "更新数据语句",
    code: "UPDATE table_name SET column1 = :value1 WHERE id = :id",
    keywords: ["update", "set", "where"],
  },
  {
    name: "DELETE语句",
    description: "删除数据语句",
    code: "DELETE FROM table_name WHERE id = :id",
    keywords: ["delete", "from", "where"],
  },
  {
    name: "JOIN查询",
    description: "表连接查询",
    code: "SELECT a.*, b.name FROM table_a a JOIN table_b b ON a.b_id = b.id WHERE a.status = 1",
    keywords: ["select", "join", "on", "where"],
  },
  {
    name: "子查询",
    description: "嵌套子查询",
    code: "SELECT * FROM table1 WHERE id IN (SELECT id FROM table2 WHERE status = 1)",
    keywords: ["select", "from", "where", "in", "subquery"],
  },
  {
    name: "分组统计",
    description: "GROUP BY分组统计",
    code: "SELECT department, COUNT(*) as count, AVG(salary) as avg_salary FROM employees GROUP BY department HAVING COUNT(*) > 5",
    keywords: ["select", "from", "group by", "having", "count", "avg"],
  },
];

// 默认JavaScript代码片段数据
const defaultJsSnippets = [
  {
    name: "参数处理",
    description: "处理传入参数",
    code: '// 获取参数\nconst { param1, param2 } = params\n\n// 参数验证\nif (!param1) {\n  throw new Error("参数param1不能为空")\n}\n\n// 返回结果\nreturn { success: true, data: param1 }',
    keywords: ["params", "参数", "validation", "验证"],
  },
  {
    name: "数组操作",
    description: "常见的数组操作",
    code: "// 数组映射\nconst result = data.map(item => ({\n  id: item.id,\n  name: item.name\n}))\n\n// 数组过滤\nconst filtered = data.filter(item => item.status === 1)\n\n// 数组查找\nconst found = data.find(item => item.id === targetId)\n\nreturn result",
    keywords: ["array", "数组", "map", "filter", "find"],
  },
  {
    name: "异步操作",
    description: "异步函数处理",
    code: "async function processData() {\n  // 模拟异步操作\n  const result = await someAsyncFunction(params)\n  \n  // 数据处理\n  const processed = result.map(item => {\n    return { ...item, processed: true }\n  })\n  \n  return processed\n}\n\nreturn await processData()",
    keywords: ["async", "异步", "await", "promise"],
  },
  {
    name: "错误处理",
    description: "try-catch错误处理",
    code: 'try {\n  // 业务逻辑\n  const result = process(params)\n  \n  // 返回成功结果\n  return { \n    code: 200, \n    message: "操作成功", \n    data: result \n  }\n} catch (error) {\n  console.error("处理失败:", error)\n  \n  // 返回错误结果\n  return { \n    code: 500, \n    message: error.message || "处理失败", \n    data: null \n  }\n}',
    keywords: ["try", "catch", "错误处理", "error"],
  },
  {
    name: "对象操作",
    description: "对象属性和方法",
    code: "// 对象解构\nconst { name, age, ...rest } = user\n\n// 对象合并\nconst merged = { ...obj1, ...obj2 }\n\n// 对象遍历\nObject.keys(obj).forEach(key => {\n  console.log(key, obj[key])\n})\n\n// 返回对象\nreturn { success: true, data: merged }",
    keywords: ["object", "对象", "destructuring", "解构", "spread"],
  },
  {
    name: "函数定义",
    description: "函数定义和使用",
    code: "// 函数定义\nfunction calculateSum(a, b) {\n  return a + b\n}\n\n// 箭头函数\nconst multiply = (a, b) => a * b\n\n// 默认参数\nfunction greet(name = 'Guest') {\n  return `Hello, ${name}!`\n}\n\n// 调用函数\nconst result = calculateSum(5, 3)",
    keywords: ["function", "函数", "arrow function", "箭头函数"],
  },
];

// 获取当前语言名称
const currentLanguageName = computed(() => {
  const langMap = {
    'sql': 'SQL',
    'javascript': 'JavaScript',
    'json': 'JSON'
  };
  return langMap[currentLanguage.value] || currentLanguage.value;
});

// 根据语言获取合并后的片段数据
const getMergedSnippets = (language) => {
  if (language === 'sql') {
    if (props.overrideDefaults) {
      // 覆盖模式：只使用自定义片段
      return props.customSqlSnippets;
    } else {
      // 合并模式：合并默认和自定义片段，自定义片段在前
      return [...props.customSqlSnippets, ...defaultSqlSnippets];
    }
  } else if (language === 'javascript') {
    if (props.overrideDefaults) {
      // 覆盖模式：只使用自定义片段
      return props.customJsSnippets;
    } else {
      // 合并模式：合并默认和自定义片段，自定义片段在前
      return [...props.customJsSnippets, ...defaultJsSnippets];
    }
  } else {
    // 对于其他语言，返回空数组
    return [];
  }
};

// 根据当前语言获取对应的片段
const currentSnippets = computed(() => {
  return getMergedSnippets(currentLanguage.value);
});

// 监听语言变化
const updateLanguage = (lang) => {
  currentLanguage.value = lang;
};

// 暴露方法供外部调用
const getSnippetsByLanguage = (language) => {
  return getMergedSnippets(language);
};

// 转义HTML
function escapeHtml(text) {
  const div = document.createElement("div");
  div.textContent = text;
  return div.innerHTML;
}

// 高亮显示代码片段
const highlightSnippetCode = (code, language) => {
  try {
    // 格式化代码：添加适当的换行
    let formattedCode = code;
    
    // 根据语言进行基本格式化
    if (language === 'sql') {
      // SQL格式化：关键字前加换行
      const sqlKeywords = ['SELECT', 'FROM', 'WHERE', 'INSERT', 'UPDATE', 'DELETE', 'CREATE', 'ALTER', 'DROP', 'JOIN'];
      sqlKeywords.forEach(keyword => {
        const regex = new RegExp(`\\b${keyword}\\b`, 'gi');
        formattedCode = formattedCode.replace(regex, `\n${keyword}`);
      });
      // 清理多余的换行
      formattedCode = formattedCode.replace(/\n\s*\n/g, '\n').trim();
    } else if (language === 'javascript') {
      // JavaScript格式化：缩进处理
      const lines = code.split('\n');
      let indentLevel = 0;
      formattedCode = lines.map(line => {
        const trimmed = line.trim();
        
        // 减少缩进
        if (trimmed.startsWith('}') || trimmed.startsWith(']') || trimmed.includes('} else')) {
          indentLevel = Math.max(0, indentLevel - 1);
        }
        
        const indented = '  '.repeat(indentLevel) + trimmed;
        
        // 增加缩进
        if (trimmed.endsWith('{') || trimmed.endsWith('[') || trimmed.includes('if (') || 
            trimmed.includes('for (') || trimmed.includes('while (')) {
          indentLevel++;
        }
        
        return indented;
      }).join('\n');
    }
    
    // 使用highlight.js进行语法高亮
    const result = hljs.highlight(formattedCode, {
      language: language === 'json' ? 'json' : language,
      ignoreIllegals: true,
    });
    
    return result.value;
  } catch (error) {
    console.error('代码片段高亮错误:', error);
    return escapeHtml(code);
  }
};

// 处理片段点击
const handleSnippetClick = (snippet) => {
  emit("insert", snippet);
};

// 关闭弹窗
const close = () => {
  emit("close");
  emit("update:modelValue", false);
};

// 注册highlight.js语言
hljs.registerLanguage("sql", sql);
hljs.registerLanguage("javascript", javascript);
hljs.registerLanguage("json", json);

// 暴露方法供父组件调用
defineExpose({
  updateLanguage,
  getSnippetsByLanguage,
  defaultSqlSnippets,
  defaultJsSnippets,
});
</script>

<style scoped>
.snippet-empty {
  text-align: center;
  padding: 40px 20px;
}

.snippet-item {
  padding: 12px;
  margin-bottom: 8px;
  border: 1px solid #e8e8e8;
  border-radius: 4px;
  cursor: pointer;
  transition: border-color 0.2s;
}

.snippet-item:hover {
  border-color: #409eff;
  background-color: #f0f7ff;
}

.snippet-item h5 {
  margin: 0 0 8px 0;
  font-size: 14px;
  color: #333;
}

.snippet-item p {
  margin: 0 0 8px 0;
  font-size: 12px;
  color: #666;
}

.snippet-code {
  margin: 0;
  padding: 8px;
  background-color: #f6f8fa;
  border-radius: 3px;
  font-size: 12px;
  font-family: "Courier New", monospace;
  overflow-wrap: break-word;
  white-space: pre-wrap;
  word-break: break-word;
}

/* 代码片段中的语法高亮样式 */
.snippet-code :deep(.hljs) {
  background: transparent !important;
  padding: 0 !important;
  font-size: 12px !important;
  line-height: 1.4 !important;
}

.snippet-code :deep(.hljs-keyword),
.snippet-code :deep(.hljs-selector-tag),
.snippet-code :deep(.hljs-literal) {
  color: #d73a49 !important;
}

.snippet-code :deep(.hljs-string),
.snippet-code :deep(.hljs-doctag) {
  color: #032f62 !important;
}

.snippet-code :deep(.hljs-title),
.snippet-code :deep(.hljs-section) {
  color: #6f42c1 !important;
}

.snippet-code :deep(.hljs-comment) {
  color: #6a737d !important;
}

/* 暗黑主题样式 */
:global(.dark) .snippet-item {
  border-color: #434343;
  background-color: #1f1f1f;
}

:global(.dark) .snippet-item:hover {
  border-color: #409eff;
  background-color: #262626;
}

:global(.dark) .snippet-item h5 {
  color: #e8e8e8;
}

:global(.dark) .snippet-item p {
  color: #8c8c8c;
}

:global(.dark) .snippet-code {
  background-color: #262626;
  color: #e8e8e8;
}
</style>