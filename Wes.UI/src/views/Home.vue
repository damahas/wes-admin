<template>
  <div class="home-container">
    <!-- 欢迎横幅 -->
    <div class="welcome-banner">
      <div class="banner-content">
        <div class="banner-text">
          <h1 class="banner-greeting">{{ greeting }}</h1>
          <p class="banner-date">{{ dateText }}</p>
          <p class="banner-desc">{{ t('home.monitorStatus') }}</p>
        </div>
        <div class="banner-illustration">
          <i class="fa fa-th-large"></i>
        </div>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-row">
      <div class="stat-card users">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-user"></i>
          </div>
          <span class="stat-badge">{{ t('common.user') }}</span>
        </div>
        <div class="stat-number">128</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.totalUsers') }}</span>
          <span class="stat-trend up"><i class="fa fa-arrow-up"></i> 12%</span>
        </div>
      </div>
      <div class="stat-card roles">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-shield"></i>
          </div>
          <span class="stat-badge">{{ t('home.role') }}</span>
        </div>
        <div class="stat-number">12</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.configuredRoles') }}</span>
          <span class="stat-trend up"><i class="fa fa-arrow-up"></i> 5%</span>
        </div>
      </div>
      <div class="stat-card depts">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-sitemap"></i>
          </div>
          <span class="stat-badge">{{ t('home.dept') }}</span>
        </div>
        <div class="stat-number">8</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.orgStructure') }}</span>
          <span class="stat-trend flat"><i class="fa fa-minus"></i> {{ t('home.flat') }}</span>
        </div>
      </div>
      <div class="stat-card flows">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-code-fork"></i>
          </div>
          <span class="stat-badge">{{ t('home.flow') }}</span>
        </div>
        <div class="stat-number">5</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.flowTemplates') }}</span>
          <span class="stat-trend up"><i class="fa fa-arrow-up"></i> 8%</span>
        </div>
      </div>
    </div>

    <!-- 快捷入口 + 公告 -->
    <div class="middle-row">
      <div class="quick-section">
        <h3 class="section-title">{{ t('home.quickEntry') }}</h3>
        <div class="quick-links">
          <div class="quick-link" @click="$router.push('/system/user')">
            <div class="ql-icon user"><i class="fa fa-user-plus"></i></div>
            <div class="ql-text">
              <span class="ql-name">{{ t('home.userManagement') }}</span>
              <span class="ql-desc">{{ t('home.userDesc') }}</span>
            </div>
          </div>
          <div class="quick-link" @click="$router.push('/system/role')">
            <div class="ql-icon role"><i class="fa fa-shield"></i></div>
            <div class="ql-text">
              <span class="ql-name">{{ t('home.roleManagement') }}</span>
              <span class="ql-desc">{{ t('home.roleDesc') }}</span>
            </div>
          </div>
          <div class="quick-link" @click="$router.push('/system/menu')">
            <div class="ql-icon menu"><i class="fa fa-list"></i></div>
            <div class="ql-text">
              <span class="ql-name">{{ t('home.menuManagement') }}</span>
              <span class="ql-desc">{{ t('home.menuDesc') }}</span>
            </div>
          </div>
          <div class="quick-link" @click="$router.push('/system/dept')">
            <div class="ql-icon dept"><i class="fa fa-sitemap"></i></div>
            <div class="ql-text">
              <span class="ql-name">{{ t('home.deptManagement') }}</span>
              <span class="ql-desc">{{ t('home.deptDesc') }}</span>
            </div>
          </div>
          <div class="quick-link" @click="$router.push('/flow/process')">
            <div class="ql-icon flow"><i class="fa fa-code-fork"></i></div>
            <div class="ql-text">
              <span class="ql-name">{{ t('home.flowDesign') }}</span>
              <span class="ql-desc">{{ t('home.flowDesc') }}</span>
            </div>
          </div>
        </div>
      </div>
      <div class="notice-section">
        <h3 class="section-title">{{ t('home.notice') }}</h3>
        <div class="notice-card">
          <div class="notice-item">
            <div class="notice-dot"></div>
            <div class="notice-content">
              <span class="notice-title">系统 v2.0.0 已正式发布</span>
              <span class="notice-time">2024-06-15</span>
            </div>
          </div>
          <div class="notice-item">
            <div class="notice-dot blue"></div>
            <div class="notice-content">
              <span class="notice-title">新增流程引擎模块，支持自定义审批流</span>
              <span class="notice-time">2024-06-10</span>
            </div>
          </div>
          <div class="notice-item">
            <div class="notice-dot orange"></div>
            <div class="notice-content">
              <span class="notice-title">安全提醒：请定期修改登录密码</span>
              <span class="notice-time">2024-06-01</span>
            </div>
          </div>
          <div class="notice-item">
            <div class="notice-dot gray"></div>
            <div class="notice-content">
              <span class="notice-title">数据备份将于每周日凌晨自动执行</span>
              <span class="notice-time">2024-05-28</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 底部三栏 -->
    <div class="bottom-row">
      <div class="info-card">
        <h3 class="section-title">
          <i class="fa fa-server"></i> {{ t('home.systemInfo') }}
        </h3>
        <div class="info-grid">
          <div class="info-item">
            <span class="info-key">{{ t('home.systemVersion') }}</span>
            <span class="info-val">v2.0.0</span>
          </div>
          <div class="info-item">
            <span class="info-key">{{ t('home.frontendFramework') }}</span>
            <span class="info-val">{{ t('home.vueFramework') }}</span>
          </div>
          <div class="info-item">
            <span class="info-key">{{ t('home.backendFramework') }}</span>
            <span class="info-val">{{ t('home.dotnetFramework') }}</span>
          </div>
          <div class="info-item">
            <span class="info-key">{{ t('home.database') }}</span>
            <span class="info-val">{{ t('home.mysql') }}</span>
          </div>
        </div>
      </div>
      <div class="info-card">
        <h3 class="section-title">
          <i class="fa fa-lightbulb-o"></i> {{ t('home.guideTitle') }}
        </h3>
        <ul class="guide-list">
          <li>
            <span class="guide-step">1</span>
            <span>{{ t('home.guideStep1') }}</span>
          </li>
          <li>
            <span class="guide-step">2</span>
            <span>{{ t('home.guideStep2') }}</span>
          </li>
          <li>
            <span class="guide-step">3</span>
            <span>{{ t('home.guideStep3') }}</span>
          </li>
          <li>
            <span class="guide-step">4</span>
            <span>{{ t('home.guideStep4') }}</span>
          </li>
        </ul>
      </div>
      <div class="info-card">
        <h3 class="section-title">
          <i class="fa fa-clock-o"></i> {{ t('home.recentVisit') }}
        </h3>
        <div class="recent-list">
          <div class="recent-item" @click="$router.push('/system/user')">
            <i class="fa fa-circle"></i>
            <span>{{ t('home.userManagement') }}</span>
            <span class="recent-time">10{{ t('home.minutesAgo') }}</span>
          </div>
          <div class="recent-item" @click="$router.push('/system/role')">
            <i class="fa fa-circle"></i>
            <span>{{ t('home.roleManagement') }}</span>
            <span class="recent-time">30{{ t('home.minutesAgo') }}</span>
          </div>
          <div class="recent-item" @click="$router.push('/system/menu')">
            <i class="fa fa-circle"></i>
            <span>{{ t('home.menuManagement') }}</span>
            <span class="recent-time">1{{ t('home.hoursAgo') }}</span>
          </div>
          <div class="recent-item" @click="$router.push('/system/dept')">
            <i class="fa fa-circle"></i>
            <span>{{ t('home.deptManagement') }}</span>
            <span class="recent-time">{{ t('home.yesterday') }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const greeting = computed(() => {
  const h = new Date().getHours()
  if (h < 6) return t('home.lateNight')
  if (h < 9) return t('home.morning')
  if (h < 12) return t('home.forenoon')
  if (h < 14) return t('home.noon')
  if (h < 18) return t('home.afternoon')
  return t('home.evening')
})

const dateText = computed(() => {
  const now = new Date()
  const weekdays = [t('home.sunday'), t('home.monday'), t('home.tuesday'), t('home.wednesday'), t('home.thursday'), t('home.friday'), t('home.saturday')]
  return t('home.dateFormat', {
    year: now.getFullYear(),
    month: now.getMonth() + 1,
    day: now.getDate(),
    weekday: weekdays[now.getDay()]
  })
})
</script>

<style scoped>
.home-container {
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;
}

/* ========== 欢迎横幅 ========== */
.welcome-banner {
  background: linear-gradient(135deg, #8bc48a 0%, #7ab878 50%, #6ba368 100%);
  border-radius: 12px;
  padding: 28px 32px;
  margin-bottom: 24px;
  position: relative;
  overflow: hidden;
}
.welcome-banner::after {
  content: '';
  position: absolute;
  right: -40px;
  top: -40px;
  width: 200px;
  height: 200px;
  border-radius: 50%;
  background: rgba(255,255,255,.06);
}
.welcome-banner::before {
  content: '';
  position: absolute;
  right: 80px;
  bottom: -60px;
  width: 160px;
  height: 160px;
  border-radius: 50%;
  background: rgba(255,255,255,.04);
}
.banner-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: relative;
  z-index: 1;
}
.banner-text {
  color: #fff;
}
.banner-greeting {
  font-size: 24px;
  font-weight: 700;
  margin: 0 0 8px;
}
.banner-date {
  font-size: 14px;
  opacity: .85;
  margin: 0 0 6px;
}
.banner-desc {
  font-size: 13px;
  opacity: .7;
  margin: 0;
}
.banner-illustration {
  font-size: 72px;
  color: rgba(255,255,255,.15);
  line-height: 1;
}

/* ========== 统计卡片 ========== */
.stats-row {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}
.stat-card {
  background: var(--bg-card);
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid var(--border-color);
  transition: box-shadow .2s;
}
.stat-card:hover {
  box-shadow: 0 2px 12px rgba(0,0,0,.06);
}
.stat-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}
.stat-icon-wrap {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
}
.stat-card.users .stat-icon-wrap  { background: rgba(107, 163, 104, .12); color: #6BA368; }
.stat-card.roles .stat-icon-wrap  { background: rgba(64, 158, 255, .12);  color: #409EFF; }
.stat-card.depts .stat-icon-wrap  { background: rgba(230, 162, 60, .12);  color: #E6A23C; }
.stat-card.flows .stat-icon-wrap  { background: rgba(103, 194, 58, .12);  color: #67C23A; }

.stat-badge {
  font-size: 12px;
  padding: 2px 10px;
  border-radius: 10px;
  font-weight: 500;
}
.stat-card.users .stat-badge  { background: rgba(107,163,104,.1);  color: #6BA368; }
.stat-card.roles .stat-badge  { background: rgba(64,158,255,.1);   color: #409EFF; }
.stat-card.depts .stat-badge  { background: rgba(230,162,60,.1);   color: #E6A23C; }
.stat-card.flows .stat-badge  { background: rgba(103,194,58,.1);   color: #67C23A; }

.stat-number {
  font-size: 32px;
  font-weight: 700;
  color: var(--text-title);
  line-height: 1;
  margin-bottom: 12px;
  font-variant-numeric: tabular-nums;
}
.stat-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.stat-label {
  font-size: 13px;
  color: var(--text-secondary);
}
.stat-trend {
  font-size: 12px;
  font-weight: 500;
}
.stat-trend.up   { color: #67C23A; }
.stat-trend.flat { color: var(--text-secondary); }

/* ========== 中部两栏 ========== */
.middle-row {
  display: grid;
  grid-template-columns: 1fr 360px;
  gap: 16px;
  margin-bottom: 24px;
}

/* ========== Section Title ========== */
.section-title {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-title);
  margin: 0 0 14px;
  display: flex;
  align-items: center;
  gap: 8px;
}
.section-title i {
  color: var(--theme-color);
}

/* ========== 快捷入口 ========== */
.quick-section {
  background: var(--bg-card);
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid var(--border-color);
}
.quick-links {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.quick-link {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 12px 14px;
  border-radius: 8px;
  cursor: pointer;
  transition: background .15s;
}
.quick-link:hover {
  background: var(--bg-page);
}
.ql-icon {
  width: 38px;
  height: 38px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  flex-shrink: 0;
}
.ql-icon.user { background: rgba(107,163,104,.1);  color: #6BA368; }
.ql-icon.role { background: rgba(64,158,255,.1);   color: #409EFF; }
.ql-icon.menu { background: rgba(230,162,60,.1);   color: #E6A23C; }
.ql-icon.dept { background: rgba(103,194,58,.1);   color: #67C23A; }
.ql-icon.flow { background: rgba(144,147,153,.1);  color: #909399; }

.ql-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.ql-name {
  font-size: 14px;
  font-weight: 500;
  color: var(--text-primary);
}
.ql-desc {
  font-size: 12px;
  color: var(--text-secondary);
}

/* ========== 公告区域 ========== */
.notice-section {
  background: var(--bg-card);
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid var(--border-color);
}
.notice-card {
  display: flex;
  flex-direction: column;
  gap: 0;
}
.notice-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 12px 0;
  border-bottom: 1px solid var(--border-color);
}
.notice-item:last-child {
  border-bottom: none;
  padding-bottom: 0;
}
.notice-item:first-child {
  padding-top: 0;
}
.notice-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #67C23A;
  margin-top: 5px;
  flex-shrink: 0;
}
.notice-dot.blue   { background: #409EFF; }
.notice-dot.orange { background: #E6A23C; }
.notice-dot.gray   { background: #C0C4CC; }

.notice-content {
  display: flex;
  flex-direction: column;
  gap: 4px;
  min-width: 0;
}
.notice-title {
  font-size: 14px;
  color: var(--text-primary);
}
.notice-time {
  font-size: 12px;
  color: var(--text-secondary);
}

/* ========== 底部三栏 ========== */
.bottom-row {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 16px;
}
.info-card {
  background: var(--bg-card);
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid var(--border-color);
}

/* 系统信息 */
.info-grid {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.info-key {
  font-size: 14px;
  color: var(--text-secondary);
}
.info-val {
  font-size: 14px;
  color: var(--text-primary);
  font-weight: 500;
}

/* 操作指引 */
.guide-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.guide-list li {
  font-size: 13px;
  color: var(--text-secondary);
  display: flex;
  align-items: flex-start;
  gap: 10px;
  line-height: 1.5;
}
.guide-step {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: var(--theme-color);
  color: #fff;
  font-size: 11px;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  margin-top: 1px;
}

/* 最近访问 */
.recent-list {
  display: flex;
  flex-direction: column;
  gap: 0;
}
.recent-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 0;
  border-bottom: 1px solid var(--border-color);
  cursor: pointer;
  transition: color .15s;
}
.recent-item:last-child {
  border-bottom: none;
  padding-bottom: 0;
}
.recent-item:first-child {
  padding-top: 0;
}
.recent-item:hover {
  color: var(--theme-color);
}
.recent-item i {
  font-size: 8px;
  color: var(--theme-color);
  flex-shrink: 0;
}
.recent-item span:first-of-type {
  font-size: 14px;
  color: var(--text-primary);
  flex: 1;
}
.recent-time {
  font-size: 12px !important;
  color: var(--text-secondary) !important;
}

/* ========== 响应式 ========== */
@media (max-width: 1280px) {
  .middle-row { grid-template-columns: 1fr; }
  .bottom-row { grid-template-columns: 1fr 1fr; }
}
@media (max-width: 900px) {
  .stats-row { grid-template-columns: repeat(2, 1fr); }
  .bottom-row { grid-template-columns: 1fr; }
}
@media (max-width: 600px) {
  .stats-row { grid-template-columns: 1fr; }
  .home-container { padding: 16px; }
  .welcome-banner { padding: 20px; }
  .banner-greeting { font-size: 20px; }
  .banner-illustration { display: none; }
}
</style>
