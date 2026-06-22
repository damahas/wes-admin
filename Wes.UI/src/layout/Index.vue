<template>
  <div class="layout-container">
    <Header />
    <div class="layout-body">
      <Sidebar :is-collapse="isCollapse" @toggle-collapse="toggleCollapse" />
      <el-main class="main">
        <router-view />
      </el-main>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import Header from './components/Header'
import Sidebar from './components/Sidebar'

const isCollapse = ref(false)

const toggleCollapse = () => {
  isCollapse.value = !isCollapse.value
}
</script>

<style scoped>
.layout-container {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100vh;
}

.layout-body {
  flex: 1;
  display: flex;
  overflow: hidden;
}

.main {
  flex: 1;
  padding: 10px;
  overflow-y: auto;
  box-sizing: border-box;
  background-color: var(--bg-primary);
}
</style>
