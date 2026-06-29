<template>
  <div class="home-container">
    <!-- 顶栏 -->
    <div class="home-header">
      <div class="header-left">
        <h2 class="page-title">工作台</h2>
        <p class="page-subtitle">{{ dateText }}，欢迎回来</p>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-row">
      <div class="stat-card">
        <div class="stat-icon users">
          <i class="fa fa-fw fa-user"></i>
        </div>
        <div class="stat-info">
          <span class="stat-label">用户总数</span>
          <span class="stat-value">128</span>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon roles">
          <i class="fa fa-fw fa-shield"></i>
        </div>
        <div class="stat-info">
          <span class="stat-label">角色数量</span>
          <span class="stat-value">12</span>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon depts">
          <i class="fa fa-fw fa-sitemap"></i>
        </div>
        <div class="stat-info">
          <span class="stat-label">部门数量</span>
          <span class="stat-value">8</span>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon flows">
          <i class="fa fa-fw fa-code-fork"></i>
        </div>
        <div class="stat-info">
          <span class="stat-label">流程模板</span>
          <span class="stat-value">5</span>
        </div>
      </div>
    </div>

    <!-- 快捷入口 -->
    <div class="section">
      <h3 class="section-title">快捷入口</h3>
      <div class="quick-links">
        <div class="quick-link" @click="$router.push('/system/user')">
          <i class="fa fa-user-plus"></i>
          <span>用户管理</span>
        </div>
        <div class="quick-link" @click="$router.push('/system/role')">
          <i class="fa fa-shield"></i>
          <span>角色管理</span>
        </div>
        <div class="quick-link" @click="$router.push('/system/menu')">
          <i class="fa fa-list"></i>
          <span>菜单管理</span>
        </div>
        <div class="quick-link" @click="$router.push('/system/dept')">
          <i class="fa fa-sitemap"></i>
          <span>部门管理</span>
        </div>
        <div class="quick-link" @click="$router.push('/flow/process')">
          <i class="fa fa-code-fork"></i>
          <span>流程设计</span>
        </div>
        <div class="quick-link" @click="$router.push('/system/config')">
          <i class="fa fa-cog"></i>
          <span>系统配置</span>
        </div>
      </div>
    </div>

    <!-- 底部两栏 -->
    <div class="bottom-row">
      <div class="info-card">
        <h3 class="section-title">系统信息</h3>
        <ul class="info-list">
          <li><span>系统版本</span><span>v2.0.0</span></li>
          <li><span>前端框架</span><span>Vue 3 + Element Plus</span></li>
          <li><span>后端框架</span><span>.NET 6 + SqlSugar</span></li>
          <li><span>数据库</span><span>MySQL</span></li>
        </ul>
      </div>
      <div class="info-card">
        <h3 class="section-title">操作指引</h3>
        <ul class="info-list">
          <li>
            <i class="fa fa-circle-thin"></i>
            <span>通过「系统管理」维护用户、角色、菜单等基础数据</span>
          </li>
          <li>
            <i class="fa fa-circle-thin"></i>
            <span>通过「流程管理」设计审批流程并跟踪流程实例</span>
          </li>
          <li>
            <i class="fa fa-circle-thin"></i>
            <span>分配角色时请确保赋予正确的菜单和数据权限</span>
          </li>
          <li>
            <i class="fa fa-circle-thin"></i>
            <span>系统配置支持动态开关验证码、日志级别等功能</span>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const dateText = computed(() => {
  const now = new Date()
  const h = now.getHours()
  const w = ['日', '一', '二', '三', '四', '五', '六']
  const prefix = h < 6 ? '夜深了' : h < 12 ? '上午好' : h < 18 ? '下午好' : '晚上好'
  return `${prefix}，今天是${now.getFullYear()}年${now.getMonth() + 1}月${now.getDate()}日 星期${w[now.getDay()]}`
})
</script>

<style scoped>
.home-container {
  padding: 24px;
  max-width: 1200px;
  margin: 0 auto;
}

/* ========== 顶栏 ========== */
.home-header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  margin-bottom: 24px;
}
.page-title {
  font-size: 22px;
  font-weight: 600;
  color: var(--text-title);
  margin: 0 0 6px;
}
.page-subtitle {
  font-size: 14px;
  color: var(--text-secondary);
  margin: 0;
}

/* ========== 统计卡片 ========== */
.stats-row {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 28px;
}
.stat-card {
  background: var(--bg-card);
  border-radius: 8px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
}
.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  flex-shrink: 0;
}
.stat-icon.users  { background: rgba(107, 163, 104, .12); color: var(--theme-color); }
.stat-icon.roles  { background: rgba(64, 158, 255, .12);  color: #409EFF; }
.stat-icon.depts  { background: rgba(230, 162, 60, .12);  color: #E6A23C; }
.stat-icon.flows  { background: rgba(103, 194, 58, .12);  color: #67C23A; }

.stat-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.stat-label {
  font-size: 14px;
  color: var(--text-secondary);
}
.stat-value {
  font-size: 28px;
  font-weight: 600;
  color: var(--text-title);
  line-height: 1;
}

/* ========== Section ========== */
.section {
  margin-bottom: 28px;
}
.section-title {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-title);
  margin: 0 0 14px;
}

/* ========== 快捷入口 ========== */
.quick-links {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  gap: 16px;
}
.quick-link {
  background: var(--bg-card);
  border-radius: 8px;
  padding: 22px 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  transition: transform .2s, box-shadow .2s;
}
.quick-link:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, .08);
}
.quick-link i {
  font-size: 26px;
  color: var(--theme-color);
}
.quick-link span {
  font-size: 14px;
  color: var(--text-primary);
}

/* ========== 底部两栏 ========== */
.bottom-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
.info-card {
  background: var(--bg-card);
  border-radius: 8px;
  padding: 20px 24px;
}
.info-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.info-list li {
  font-size: 14px;
  color: var(--text-secondary);
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}
.info-list li i {
  flex-shrink: 0;
  font-size: 10px;
  color: var(--theme-color);
  margin-right: 4px;
}
.info-list li span:last-child {
  text-align: right;
}
.info-card:last-child .info-list li {
  justify-content: flex-start;
}

/* ========== 响应式 ========== */
@media (max-width: 1100px) {
  .stats-row { grid-template-columns: repeat(2, 1fr); }
  .quick-links { grid-template-columns: repeat(3, 1fr); }
  .bottom-row { grid-template-columns: 1fr; }
}
@media (max-width: 640px) {
  .stats-row { grid-template-columns: 1fr; }
  .quick-links { grid-template-columns: repeat(2, 1fr); }
  .home-container { padding: 16px; }
}
</style>
