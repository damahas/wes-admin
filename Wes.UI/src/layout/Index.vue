<template>
  <div class="layout-container">
    <Sidebar :is-collapse="isCollapse" @toggle-collapse="toggleCollapse" />
    <el-container class="main-container">
      <Header :is-collapse="isCollapse" @toggle-collapse="toggleCollapse" />
      <el-main class="main">
        <router-view />
      </el-main>
    </el-container>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import Sidebar from './components/Sidebar'
import Header from './components/Header'

const isCollapse = ref(false)

const toggleCollapse = () => {
  isCollapse.value = !isCollapse.value
}
</script>

<style scoped>
.layout-container {
  display: flex;
  width: 100%;
  height: 100vh;
}

.main-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.main {
  flex: 1;
  padding: 10px;
  overflow-y: auto;
  box-sizing: border-box;
}
</style>
