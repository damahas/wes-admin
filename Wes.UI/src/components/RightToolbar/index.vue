<template>
  <div class="top-right-btn" :style="style">
    <el-row>
      <el-tooltip
        class="item"
        effect="dark"
        :content="showSearch ? '隐藏搜索' : '显示搜索'"
        placement="top"
        v-if="search"
      >
        <el-button circle icon="Search" @click="toggleSearch()" />
      </el-tooltip>
      <el-tooltip class="item" effect="dark" content="刷新" placement="top">
        <el-button circle icon="Refresh" @click="refresh()" />
      </el-tooltip>
      <el-tooltip
        class="item"
        effect="dark"
        content="显隐列"
        placement="top"
        v-if="columns"
      >
        <!-- <el-button
          circle
          icon="Menu"
          @click="showColumn()"
          v-if="showColumnsType == 'transfer'"
        /> -->
        <el-dropdown
          trigger="click"
          :hide-on-click="false"
          style="padding-left: 12px"
          v-if="showColumnsType == 'checkbox'"
        >
          <el-button circle icon="Menu" />
          <template #dropdown>
            <el-dropdown-menu>
              <template v-for="item in columnArray" :key="item.key">
                <el-dropdown-item>
                  <el-checkbox
                    :checked="item.visible"
                    @change="checkboxChange($event, item.key)"
                    :label="item.label"
                  />
                </el-dropdown-item>
              </template>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </el-tooltip>
    </el-row>
    <!-- <el-dialog :title="title" v-model="open" append-to-body>
      <el-transfer
        :titles="['显示', '隐藏']"
        v-model="value"
        :data="columns"
        @change="dataChange"
      ></el-transfer>
    </el-dialog> -->
  </div>
</template>

<script setup>
import { ref, computed } from "vue";

const props = defineProps({
  showSearch: {
    type: Boolean,
    default: true,
  },
  columns: {
    type: Object,
  },
  search: {
    type: Boolean,
    default: true,
  },
  showColumnsType: {
    type: String,
    default: "checkbox",
  },
  gutter: {
    type: Number,
    default: 10,
  },
});

const emit = defineEmits(["update:showSearch", "queryTable"]);

const value = ref([]);
// const title = ref("显示/隐藏");
const open = ref(false);

const style = computed(() => {
  if (!props.gutter) return {};
  return { marginRight: `${props.gutter / 2}px` };
});

const columnArray = computed(() => {
  return Object.keys(props.columns).map((p) => ({ key: p, ...props.columns[p] }));
});

function toggleSearch() {
  emit("update:showSearch", !props.showSearch);
}

function refresh() {
  emit("queryTable");
}

function dataChange(data) {
  Object.values(props.columns).forEach((item) => {
    item.visible = !data.includes(item.key);
  });
}

function showColumn() {
  open.value = true;
}

function checkboxChange(event, key) {
  const column = props.columns[key];
  if (column) {
    column.visible = event;
  }
}

if (props.showColumnsType === "transfer" && props.columns) {
  Object.values(props.columns).forEach((item, index) => {
    if (item.visible === false) {
      value.value.push(index);
    }
  });
}
</script>

<style lang="scss" scoped>
:deep(.el-transfer__button) {
  border-radius: 50%;
  display: block;
  margin-left: 0px;
}
:deep(.el-transfer__button:first-child) {
  margin-bottom: 10px;
}
:deep(.el-dropdown-menu__item) {
  line-height: 30px;
  padding: 0 17px;
}

.top-right-btn {
  margin-left: auto;
}
</style>
