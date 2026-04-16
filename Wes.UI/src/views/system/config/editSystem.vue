<template>
  <div class="subsystem-container">
    <el-form-item label="系统列表">
      <el-button type="primary" plain icon="Plus" @click="handleAdd">新增</el-button>
    <el-table 
      :data="systemList" 
      border 
      stripe
      style="width: 100%"
      class="subsystem-table"
    >
      <el-table-column label="ID" align="left" header-align="left">
        <template #default="scope">
          <el-input v-model="scope.row.systemId" @change="handleChange"></el-input>
        </template>
      </el-table-column>
      <el-table-column label="系统名称" align="left" header-align="left">
        <template #default="scope">
          <el-input v-model="scope.row.systemName" @change="handleChange"></el-input>
        </template>
      </el-table-column>
      <el-table-column label="访问地址" align="left" header-align="left" :show-overflow-tooltip="true">
        <template #default="scope">
          <el-input v-model="scope.row.systemUrl" @change="handleChange"></el-input>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="80" align="center">
        <template #default="scope">
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.$index)"></el-button>
        </template>
      </el-table-column>
    </el-table>
    </el-form-item>

  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import { ElMessage } from "element-plus";

const props = defineProps({
  modelValue: String,
});

const emits = defineEmits(["update:modelValue"]);

const systemList = ref([]);

watch(
  () => props.modelValue,
  () => {
    if (!props.modelValue) {
      systemList.value = [];
      return;
    }
    try {
      systemList.value = JSON.parse(props.modelValue);
    } catch {
      systemList.value = [];
    }
  },
  { deep: true, immediate: true }
);

const handleChange = () => {
  emits("update:modelValue", JSON.stringify(systemList.value));
};

const handleAdd = () => {
  systemList.value.push({
    systemId: "",
    systemName: "",
    systemUrl: "",
  });
  handleChange();
};

const handleDelete = (index) => {
  systemList.value.splice(index, 1);
  handleChange();
};
</script>

<style scoped>
.subsystem-container {
  width: 100%;
}

.subsystem-table {
  width: 100%;
  margin-top: 12px;
}

/* .subsystem-table :deep(.el-table__cell) {
  padding: 4px 0;
  text-align: left;
}

.subsystem-table :deep(.el-table__header-cell) {
  text-align: left;
  padding: 8px 0;
}

.subsystem-table :deep(.el-input) {
  width: 100%;
}

.subsystem-table :deep(.el-input__wrapper) {
  padding: 0 8px;
}

.subsystem-table :deep(.el-table__cell.is-center) {
  text-align: center;
} */
</style>