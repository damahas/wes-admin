<template>
  <template v-if="schema">
    <template v-if="schema.type === 'table'">
      <el-form-item label="数据列表">
        <div style="width: 100%">
          <el-button type="primary" plain icon="Plus" @click="handleAddRow"
            >新增</el-button
          >
          <el-table
            :data="modelValue"
            border
            stripe
            style="width: 100%; margin-top: 12px"
          >
            <el-table-column
              v-for="col in schema.columns"
              :key="col.key"
              :label="col.label"
              :show-overflow-tooltip="true"
            >
              <template #default="{ row }">
                <el-input v-model="row[col.key]" @change="handleChange" />
              </template>
            </el-table-column>
            <el-table-column label="操作" width="80" align="center">
              <template #default="{ $index }">
                <el-button
                  link
                  type="danger"
                  icon="Delete"
                  @click="handleDeleteRow($index)"
                ></el-button>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </el-form-item>
    </template>
    <template v-else>
      <el-form-item
        v-for="field in schema.fields"
        :key="field.key"
        :label="field.label"
        :required="field.required"
      >
        <el-switch
          v-if="field.type === 'switch'"
          v-model="modelValue[field.key]"
          @change="handleChange"
        />
        <span v-else-if="field.button" style="display: flex; width: 100%">
          <el-input
            v-model="modelValue[field.key]"
            :type="field.type === 'password' ? 'password' : 'text'"
            :show-password="field.type === 'password'"
            style="flex: 1"
            @change="handleChange"
          >
            <template #append>
              <el-button
                :loading="loadingKeys[field.key]"
                @click="handleButtonClick(field)"
              >
                {{ field.button.title }}
              </el-button>
            </template>
          </el-input>
        </span>
        <el-input
          v-else
          v-model="modelValue[field.key]"
          :type="field.type === 'password' ? 'password' : 'text'"
          :show-password="field.type === 'password'"
          @change="handleChange"
        />
      </el-form-item>
    </template>
  </template>
</template>

<script setup>
import { ref } from "vue";

const props = defineProps({
  schema: {
    type: Object,
    default: null,
  },
  modelValue: {
    type: [Object, Array],
    default: () => ({}),
  },
});

const emit = defineEmits(["update:modelValue"]);

const loadingKeys = ref({});

function handleChange() {
  emit("update:modelValue", props.modelValue);
}

async function handleButtonClick(field) {
  loadingKeys.value[field.key] = true;
  try {
    const result = field.button.action();
    if (result && typeof result.then === "function") {
      await result;
    }
  } finally {
    loadingKeys.value[field.key] = false;
  }
}

function handleAddRow() {
  if (!props.schema?.columns) return;
  const row = {};
  props.schema.columns.forEach((col) => (row[col.key] = ""));
  const arr = Array.isArray(props.modelValue) ? [...props.modelValue, row] : [row];
  emit("update:modelValue", arr);
}

function handleDeleteRow(index) {
  const arr = [...props.modelValue];
  arr.splice(index, 1);
  emit("update:modelValue", arr);
}
</script>
