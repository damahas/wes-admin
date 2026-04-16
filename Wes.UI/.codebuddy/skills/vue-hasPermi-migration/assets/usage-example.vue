<template>
  <div class="permission-demo">
    <h2>权限控制示例</h2>

    <!-- 单个权限检查 -->
    <el-button v-hasPermi="['system:user:add']" type="primary">
      新增用户
    </el-button>

    <!-- 多个权限检查（满足任意一个即可） -->
    <el-button v-hasPermi="['system:user:edit', 'system:user:delete']" type="warning">
      编辑/删除用户
    </el-button>

    <!-- 超级管理员权限（*:*:* 会通过所有权限检查） -->
    <el-button v-hasPermi="['system:user:export']" type="success">
      导出用户
    </el-button>

    <!-- 无权限时按钮会被移除 -->
    <el-button v-hasPermi="['system:user:unauthorized']" type="danger">
      无权限按钮
    </el-button>
  </div>
</template>

<script setup>
// 当前用户权限示例
// permissions: ['system:user:add', 'system:user:edit', 'system:user:delete', '*:*:*']
</script>
