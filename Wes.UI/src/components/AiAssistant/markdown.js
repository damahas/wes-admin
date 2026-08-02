/**
 * Markdown 渲染（前端 AI 回复）：
 * - 抽出围栏代码块并用 highlight.js 语法高亮（SQL 等）
 * - 代码块按语言自动格式化：SQL（sql-formatter）、JSON、XML/HTML，可读性更好
 * - 代码块带语言标签与复制按钮（复制逻辑在 AiChat.vue 中事件委托）
 * - 代码块主题跟随应用亮/暗模式（亮色用浅色主题，暗色用深色主题），高亮配色在 AiChat.vue 中定义
 * - 支持表格 / 行内代码 / 加粗 / 斜体 / 链接 / 标题
 * - 关键点：代码块先抽成占位符，最后再还原，避免内部换行被转成 <br> 导致双重空行
 */
import hljs from 'highlight.js/lib/common'
import { format as formatSql } from 'sql-formatter'
import { mermaidToEcharts } from './chartConvert'

// sql-formatter 支持的方言，映射到其 language 选项
const SQL_DIALECTS = {
  sql: 'sql', mysql: 'mysql', mariadb: 'mariadb', postgresql: 'postgresql',
  postgres: 'postgresql', pgsql: 'postgresql', plsql: 'plsql', sqlite: 'sqlite',
  tsql: 'tsql', 'transact-sql': 'tsql', bigquery: 'bigquery', redshift: 'redshift',
  spark: 'spark', db2: 'db2', hive: 'hive', trino: 'trino'
}

// 简单的 XML/HTML 缩进格式化：将标签拆分到独立行并按层级缩进
function formatXml(code) {
  const normalized = code.replace(/>\s*</g, '><').trim()
  const tokens = normalized.replace(/></g, '>\n<').split('\n')
  let indent = 0
  const pad = '  '
  return tokens.map(raw => {
    const line = raw.trim()
    if (!line) return ''
    // 闭合标签或自闭合标签：先减一级缩进
    if (/^<\/\w/.test(line)) indent = Math.max(indent - 1, 0)
    const result = pad.repeat(indent) + line
    // 开标签（非自闭合、非声明、非闭合、内部不含闭合标签）：下一行加一级缩进
    if (/^<\w[^>]*[^/]>$/.test(line) && !/^<(\?|!)/.test(line) && !line.includes('</')) indent++
    return result
  }).filter(l => l !== '').join('\n')
}

// 按语言对代码块做格式化，失败时回退原文本。
function formatCode(code, lang) {
  try {
    const l = (lang || '').toLowerCase()
    if (SQL_DIALECTS[l]) {
      return formatSql(code, { language: SQL_DIALECTS[l], keywordCase: 'upper', tabWidth: 2 })
    }
    if (l === 'json' || l === 'json5') {
      return JSON.stringify(JSON.parse(code), null, 2)
    }
    if (l === 'xml' || l === 'html' || l === 'svg' || l === 'xhtml') {
      return formatXml(code)
    }
    return code
  } catch {
    return code
  }
}

const CODE_OPEN = '@@CODE'
const CODE_CLOSE = '@@'

function escapeHtml(s) {
  return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
}

function highlight(code, lang) {
  try {
    if (lang && hljs.getLanguage(lang)) {
      return hljs.highlight(code, { language: lang }).value
    }
    return hljs.highlightAuto(code).value
  } catch {
    return escapeHtml(code)
  }
}

// ==================== 表格（GFM） ====================
function splitRow(line) {
  let s = line.trim()
  if (s.startsWith('|')) s = s.slice(1)
  if (s.endsWith('|')) s = s.slice(0, -1)
  return s.split('|').map(c => c.trim())
}
function isTableRow(line) {
  const t = line.trim()
  return t.includes('|') && t.length > 1
}
function isDelimiterRow(line) {
  if (!line.trim().includes('|')) return false
  return splitRow(line).every(c => /^:?-+:?$/.test(c))
}
function renderTables(text) {
  const lines = text.split('\n')
  const out = []
  let i = 0
  while (i < lines.length) {
    if (isTableRow(lines[i]) && i + 1 < lines.length && isDelimiterRow(lines[i + 1])) {
      const header = splitRow(lines[i])
      const aligns = splitRow(lines[i + 1]).map(c => {
        const l = c.startsWith(':')
        const r = c.endsWith(':')
        return l && r ? 'center' : r ? 'right' : l ? 'left' : ''
      })
      const body = []
      i += 2
      while (i < lines.length && isTableRow(lines[i])) {
        body.push(splitRow(lines[i]))
        i++
      }
      let html = '<div class="ai-table-wrap"><table class="ai-table"><thead><tr>'
      header.forEach((h, idx) => {
        const a = aligns[idx] ? ` style="text-align:${aligns[idx]}"` : ''
        html += `<th${a}>${h}</th>`
      })
      html += '</tr></thead><tbody>'
      body.forEach(row => {
        html += '<tr>'
        row.forEach((c, idx) => {
          const a = aligns[idx] ? ` style="text-align:${aligns[idx]}"` : ''
          html += `<td${a}>${c}</td>`
        })
        html += '</tr>'
      })
      html += '</tbody></table></div>'
      out.push(html)
      continue
    }
    out.push(lines[i])
    i++
  }
  return out.join('\n')
}

export function renderMarkdown(text) {
  if (!text) return ''

  // 1. 抽出围栏代码块（多行），避免后续处理破坏其内部内容
  const codeBlocks = []
  let src = text.replace(/```(\w*)\r?\n([\s\S]*?)```/g, (_, lang, code) => {
    const idx = codeBlocks.length
    codeBlocks.push({ lang: (lang || '').toLowerCase(), code: code.replace(/\n$/, '') })
    return `${CODE_OPEN}${idx}${CODE_CLOSE}`
  })

  // 2. 转义其余文本
  src = escapeHtml(src)

  // 3. 表格（需在换行处理前完成）
  src = renderTables(src)

  // 4. 行内元素
  src = src
    .replace(/`([^`\n]+)`/g, '<code class="ai-inline-code">$1</code>')
    .replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>')
    .replace(/\*([^*\n]+)\*/g, '<em>$1</em>')
    .replace(/\[([^\]]+)\]\((https?:\/\/[^\s)]+)\)/g, '<a class="ai-link" href="$2" target="_blank" rel="noopener">$1</a>')
    .replace(/^###\s+(.*)$/gm, '<h4 class="ai-h">$1</h4>')
    .replace(/^##\s+(.*)$/gm, '<h3 class="ai-h">$1</h3>')
    .replace(/^#\s+(.*)$/gm, '<h2 class="ai-h">$1</h2>')

  // 5. 换行 -> <br>（代码块此时已是占位符，不受影响）
  src = src.replace(/\n/g, '<br>')

  // 6. 还原代码块（按语言自动格式化 + 语法高亮）
  src = src.replace(new RegExp(`${CODE_OPEN}(\\d+)${CODE_CLOSE}`, 'g'), (_, i) => {
    const { lang, code } = codeBlocks[+i]
    // ECharts 图表块：输出占位 div，由 AiChat.vue 扫描后挂载 echarts 实例渲染
    if (lang === 'echart' || lang === 'echarts') {
      let json = code
      try { json = JSON.stringify(JSON.parse(code)) } catch { /* JSON 不规范时用原始文本 */ }
      const encoded = encodeURIComponent(json)
      return '<div class="ai-chart" data-option="' + encoded + '"><div class="ai-chart-loading">图表渲染中…</div></div>'
    }
    // mermaid 图表块：不再依赖 mermaid 运行时，直接在渲染阶段转换为 ECharts。
    // 1) 内容本身就是合法 JSON（模型把 ECharts option 写进 mermaid 块）→ 按 echart 处理；
    // 2) 内容是 mermaid 语法 → 尝试转换为 ECharts option；
    // 3) 都不行 → 降级为普通代码块展示，不报错。
    if (lang === 'mermaid') {
      let option = null
      try { option = JSON.parse(code) } catch { /* 不是 JSON，往下走 mermaid 转换 */ }
      if (!option) option = mermaidToEcharts(code)
      if (option) {
        const encoded = encodeURIComponent(JSON.stringify(option))
        return '<div class="ai-chart" data-option="' + encoded + '"><div class="ai-chart-loading">图表渲染中…</div></div>'
      }
      const html = highlight(code, 'mermaid')
      return (
        '<div class="ai-code-block">' +
        '<div class="ai-code-head"><span class="ai-code-lang">mermaid</span>' +
        '<button class="ai-copy-btn" type="button" data-copy>复制</button></div>' +
        '<pre class="ai-code"><code class="hljs">' + html + '</code></pre></div>'
      )
    }
    const display = formatCode(code, lang)
    const html = highlight(display, lang)
    const label = lang || 'code'
    return (
      '<div class="ai-code-block">' +
      '<div class="ai-code-head"><span class="ai-code-lang">' + label + '</span>' +
      '<button class="ai-copy-btn" type="button" data-copy>复制</button></div>' +
      '<pre class="ai-code"><code class="hljs">' + html + '</code></pre></div>'
    )
  })

  return src
}
