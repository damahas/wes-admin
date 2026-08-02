// 把常见的 mermaid 图表语法（xychart-beta 的 line/bar、pie）转换为 ECharts option。
// 模型有时坚持用 mermaid 语法画图（且不走 echart 通道），这里在 markdown 渲染阶段就
// 直接转换为 ECharts JSON，避免引入 mermaid 运行时依赖，保证图能稳定渲染。
export function mermaidToEcharts(src) {
  const text = src.trim()
  // —— xychart-beta：折线图 / 柱状图 ——
  if (/xychart-beta/i.test(text)) {
    const titleM = text.match(/title\s+"([^"]+)"/i)
    const xM = text.match(/x-axis\s*\[([^\]]*)\]/i)
    const yM = text.match(/y-axis\s+"([^"]*)"/i)
    const lineM = text.match(/\bline\s*\[([^\]]*)\]/i)
    const barM = text.match(/\bbar\s*\[([^\]]*)\]/i)
    if (!xM) return null
    const categories = xM[1]
      .split(',')
      .map(s => s.trim().replace(/^["']|["']$/g, ''))
      .filter(s => s !== '')
    const seriesM = lineM || barM
    if (!seriesM) return null
    const data = seriesM[1]
      .split(',')
      .map(s => {
        const n = parseFloat(s)
        return isNaN(n) ? s.trim().replace(/^["']|["']$/g, '') : n
      })
    const isLine = !!lineM
    return {
      title: { text: titleM ? titleM[1] : '' },
      tooltip: { trigger: 'axis' },
      xAxis: { type: 'category', data: categories, boundaryGap: !isLine },
      yAxis: { type: 'value', name: yM ? yM[1] : '' },
      series: [{ type: isLine ? 'line' : 'bar', data, smooth: isLine }]
    }
  }
  // —— pie：饼图 ——
  if (/^\s*pie\b/i.test(text)) {
    const titleM = text.match(/pie\s+title\s+"?([^\n"]+)"?/i)
    const data = []
    const re = /"([^"]+)"\s*:\s*(\d+(?:\.\d+)?)/g
    let m
    while ((m = re.exec(text)) !== null) {
      // 模型常把次数写在名称里（如"登录成功 (22次)"），清理掉避免 tooltip/图例重复
      const name = m[1].replace(/\s*\(\d+次\)\s*$/, '').trim()
      data.push({ name, value: parseFloat(m[2]) })
    }
    if (!data.length) return null
    return {
      title: { text: titleM ? titleM[1].trim() : '占比' },
      tooltip: { trigger: 'item', formatter: '{b}: {c}次 ({d}%)' },
      legend: {
        orient: 'horizontal',
        bottom: 8,
        left: 'center',
        itemWidth: 12,
        itemHeight: 12,
        textStyle: { fontSize: 12 }
      },
      series: [{
        type: 'pie',
        radius: ['42%', '58%'],
        center: ['50%', '50%'],
        avoidLabelOverlap: true,
        itemStyle: { borderRadius: 6, borderColor: 'transparent', borderWidth: 2 },
        label: {
          show: true,
          position: 'outside',
          formatter: '{d}%',
          fontSize: 12,
          lineHeight: 16
        },
        labelLine: { show: true, length: 10, length2: 6 },
        emphasis: {
          label: { show: true, fontSize: 13, fontWeight: 'bold' },
          itemStyle: { shadowBlur: 8, shadowOffsetX: 0, shadowColor: 'rgba(0,0,0,0.2)' }
        },
        data
      }]
    }
  }
  return null
}
