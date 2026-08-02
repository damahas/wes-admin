<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form
        :config="queryConfig"
        v-model:visible="showSearch"
        v-model="queryParams"
        @query="handleQuery"
        @reset="resetQuery"
      />

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button
            type="primary"
            plain
            icon="Plus"
            @click="handleAdd"
            v-hasPermi="['system:menu:add']"
            >{{ t('common.add') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button type="info" plain icon="Sort" @click="toggleExpandAll"
            >{{ t('common.expand') }}</el-button
          >
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table
        v-if="refreshTable"
        v-loading="loading"
        :data="menuList"
        row-key="menuId"
        :default-expand-all="isExpandAll"
        :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
      >
        <el-table-column
          prop="menuName"
          :label="t('menuManage.menuName')"
          :show-overflow-tooltip="true"
          min-width="160"
        ></el-table-column>
        <el-table-column prop="icon" :label="t('menuManage.icon')" align="center" width="100">
          <template #default="scope">
            <i :class="'fa fa-' + scope.row.icon" style="font-size: 13px" />
          </template>
        </el-table-column>
        <el-table-column prop="orderNum" :label="t('common.sort')" width="80"></el-table-column>
        <el-table-column
          prop="perms"
          :label="t('menuManage.perms')"
          :show-overflow-tooltip="true"
        ></el-table-column>
        <el-table-column
          prop="component"
          :label="t('menuManage.component')"
          :show-overflow-tooltip="true"
        ></el-table-column>
        <el-table-column prop="status" :label="t('common.status')" width="80">
          <template #default="scope">
            <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
          </template>
        </el-table-column>
        <el-table-column :label="t('common.createTime')" align="center" width="170" prop="createTime">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="t('common.actions')"
          align="center"
          width="220"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              @click="handleUpdate(scope.row)"
              v-hasPermi="['system:menu:edit']"
              >{{ t('common.edit') }}</el-button
            >
            <el-button
              link
              type="primary"
              icon="Plus"
              @click="handleAdd(scope.row)"
              v-hasPermi="['system:menu:add']"
              >{{ t('common.add') }}</el-button
            >
            <el-button
              link
              type="danger"
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['system:menu:remove']"
              >{{ t('common.delete') }}</el-button
            >
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- 添加或修改菜单对话框 -->
    <el-dialog :title="title" v-model="open" width="900px" append-to-body>
      <el-form ref="menuRef" :model="form" :rules="rules" label-width="140px">
        <el-row>
          <el-col :span="24">
            <el-form-item :label="t('menuManage.parentMenu')">
              <el-tree-select
                v-model="form.parentId"
                :data="menuOptions"
                :props="{
                  value: 'menuId',
                  label: 'menuName',
                  children: 'children',
                }"
                value-key="menuId"
                :placeholder="t('menuManage.placeholder.parentMenu')"
                check-strictly
              />
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item :label="t('menuManage.menuType')" prop="menuType">
              <el-radio-group v-model="form.menuType">
                <el-radio value="M">{{ t('menuManage.type.dir') }}</el-radio>
                <el-radio value="C">{{ t('menuManage.type.menu') }}</el-radio>
                <el-radio value="F">{{ t('menuManage.type.button') }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType != 'F'">
            <el-form-item :label="t('menuManage.menuIcon')" prop="icon">
              <el-popover placement="bottom-start" :width="540" trigger="click">
                <template #reference>
                  <el-input
                    v-model="form.icon"
                    :placeholder="t('menuManage.placeholder.icon')"
                    @blur="showSelectIcon"
                    readonly
                  >
                    <template #prefix>
                      <i v-if="form.icon" :class="'fa fa-' + form.icon"></i>
                      <el-icon v-else style="height: 32px; width: 16px">
                        <search />
                      </el-icon>
                    </template>
                  </el-input>
                </template>
                <icon-select
                  ref="iconSelectRef"
                  @selected="selected"
                  :active-icon="form.icon"
                />
              </el-popover>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="t('menuManage.displayOrder')" prop="orderNum">
              <el-input-number
                v-model="form.orderNum"
                controls-position="right"
                :min="0"
              />
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <template v-if="form.menuType != 'F'">
              <el-form-item :label="t('menuManage.menuName')" class="i18n-inline-form-item">
                <div class="i18n-inline">
                  <div class="i18n-tags">
                    <span
                      :class="['i18n-tag', 'i18n-tag-default', { active: currentTab === 'default', filled: form.menuName }]"
                      @click="currentTab = 'default'"
                    >
                      Default
                    </span>
                    <span
                      v-for="item in langList"
                      :key="item.langCode"
                      :class="['i18n-tag', { active: currentTab === item.langCode, filled: i18nValues[item.langCode] }]"
                      @click="currentTab = item.langCode"
                    >
                      {{ item.langName }}
                    </span>
                  </div>
                  <el-input
                    v-model="activeInputValue"
                    :placeholder="t('menuManage.placeholder.menuName')"
                  />
                </div>
              </el-form-item>
            </template>
            <el-form-item v-else :label="t('menuManage.menuName')" prop="menuName">
              <el-input v-model="form.menuName" :placeholder="t('menuManage.placeholder.menuName')" />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType == 'C'">
            <el-form-item prop="routeName">
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.routeName')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('menuManage.routeName') }}
                </span>
              </template>
              <el-input v-model="form.routeName" :placeholder="t('menuManage.placeholder.routeName')" />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType != 'F'">
            <el-form-item>
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.isFrame')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon> </el-tooltip
                  >{{ t('menuManage.isFrame') }}
                </span>
              </template>
              <el-radio-group v-model="form.isFrame">
                <el-radio value="0">{{ t('common.yes') }}</el-radio>
                <el-radio value="1">{{ t('common.no') }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType != 'F'">
            <el-form-item prop="path">
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.routePath')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('menuManage.routePath') }}
                </span>
              </template>
              <el-input v-model="form.path" :placeholder="t('menuManage.placeholder.routePath')" />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType == 'C'">
            <el-form-item prop="component">
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.component')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('menuManage.component') }}
                </span>
              </template>
              <el-input v-model="form.component" :placeholder="t('menuManage.placeholder.component')" />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType != 'M'">
            <el-form-item>
              <el-input
                v-model="form.perms"
                :placeholder="t('menuManage.placeholder.perms')"
                maxlength="100"
              />
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.perms')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('menuManage.perms') }}
                </span>
              </template>
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType == 'C'">
            <el-form-item>
              <el-input
                v-model="form.query"
                :placeholder="t('menuManage.placeholder.query')"
                maxlength="255"
              />
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.query')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('menuManage.query') }}
                </span>
              </template>
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType == 'C'">
            <el-form-item>
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.isCache')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('menuManage.isCache') }}
                </span>
              </template>
              <el-radio-group v-model="form.isCache">
                <el-radio value="0">{{ t('menuManage.type.cache') }}</el-radio>
                <el-radio value="1">{{ t('menuManage.type.noCache') }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="form.menuType != 'F'">
            <el-form-item>
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.visible')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('menuManage.visible') }}
                </span>
              </template>
              <el-radio-group v-model="form.visible">
                <el-radio
                  v-for="dict in sys_show_hide"
                  :key="dict.value"
                  :value="dict.value"
                  >{{ dict.label }}</el-radio
                >
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item>
              <template #label>
                <span>
                  <el-tooltip
                    :content="t('menuManage.placeholder.status')"
                    placement="top"
                  >
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ t('menuManage.menuStatus') }}
                </span>
              </template>
              <el-radio-group v-model="form.status">
                <el-radio
                  v-for="dict in sys_normal_disable"
                  :key="dict.value"
                  :value="dict.value"
                  >{{ dict.label }}</el-radio
                >
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
          <el-button @click="cancel">{{ t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="Menu">
import { ref, nextTick, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { getDict, handleTree } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import IconSelect from "@/components/IconSelect";
import { addMenu, delMenu, getMenu, listMenu, updateMenu } from "@/api/system/menu";
import { addI18n, updateI18n, listI18n, refreshI18nCache } from "@/api/system/i18n";

const { t } = useI18n();
const store = useStore();

const { sys_show_hide, sys_normal_disable } = getDict(
  "sys_show_hide",
  "sys_normal_disable"
);

const menuRef = ref(null);
const queryRef = ref(null);
const iconSelectRef = ref(null);

const menuList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const title = ref("");
const menuOptions = ref([]);
const isExpandAll = ref(false);
const refreshTable = ref(true);

// 查询条件配置
const queryConfig = computed(() => [
  {
    label: t('menuManage.menuName'),
    prop: "menuName",
    type: "input",
    placeholder: t('menuManage.placeholder.menuName'),
  },
  {
    label: t('common.status'),
    prop: "status",
    type: "select",
    placeholder: t('menuManage.placeholder.status'),
    options: sys_normal_disable,
  },
]);

const queryParams = ref({
  menuName: undefined,
  status: undefined,
});

const form = ref({});

// 多语言 - 标签切换
const langList = computed(() => store.state.system.langList);
const currentTab = ref('default');
const i18nValues = ref({});

/** 当前标签对应的输入值 */
const activeInputValue = computed({
  get: () => currentTab.value === 'default' ? (form.value.menuName || '') : (i18nValues.value[currentTab.value] || ''),
  set: (val) => {
    if (currentTab.value === 'default') {
      form.value.menuName = val;
    } else {
      i18nValues.value[currentTab.value] = val;
    }
  }
});

const rules = computed(() => ({
  menuName: [{ required: true, message: t('menuManage.rules.menuNameRequired'), trigger: "blur" }],
  orderNum: [{ required: true, message: t('menuManage.rules.orderNumRequired'), trigger: "blur" }],
  path: [{ required: true, message: t('menuManage.rules.pathRequired'), trigger: "blur" }],
}));

/** 查询菜单列表 */
function getList() {
  loading.value = true;
  listMenu(queryParams.value).then((response) => {
    menuList.value = handleTree(response.data, "menuId");
    loading.value = false;
  });
}

/** 搜索按钮操作 */
function handleQuery() {
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  if (queryRef.value) queryRef.value.resetFields();
  handleQuery();
}

/** 查询菜单下拉树结构 */
function getTreeselect() {
  menuOptions.value = [];
  listMenu().then((response) => {
    const menu = { menuId: 0, menuName: t('menuManage.parentMenu'), children: [] };
    menu.children = handleTree(response.data, "menuId");
    menuOptions.value.push(menu);
  });
}

/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

/** 表单重置 */
function reset() {
  form.value = {
    menuId: undefined,
    parentId: 0,
    menuName: undefined,
    icon: undefined,
    menuType: "M",
    orderNum: undefined,
    isFrame: "1",
    isCache: "0",
    visible: "0",
    status: "0",
  };
  currentTab.value = 'default';
  i18nValues.value = {};
  if (menuRef.value) menuRef.value.resetFields();
}

/** 展示下拉图标 */
function showSelectIcon() {
  iconSelectRef.value.reset();
}

/** 选择图标 */
function selected(name) {
  form.value.icon = name;
}

/** 新增按钮操作 */
function handleAdd(row) {
  reset();
  getTreeselect();
  if (row != null && row.menuId) {
    form.value.parentId = row.menuId;
  } else {
    form.value.parentId = 0;
  }
  open.value = true;
  title.value = t('menuManage.addTitle');
}

/** 展开/折叠操作 */
function toggleExpandAll() {
  refreshTable.value = false;
  isExpandAll.value = !isExpandAll.value;
  nextTick(() => {
    refreshTable.value = true;
  });
}

/** 修改按钮操作 */
async function handleUpdate(row) {
  reset();
  getTreeselect();
  try {
    const response = await getMenu(row.menuId);
    form.value = response.data;
    await nextTick();
    // 加载已有的多语言翻译
    await loadMenuTranslations(row.menuId);
    open.value = true;
    title.value = t('menuManage.editTitle');
  } catch (error) {
    console.error('获取菜单详情失败:', error);
  }
}

/** 提交按钮 */
function submitForm() {
  menuRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = form.value.menuId ? updateMenu(form.value) : addMenu(form.value);
      apiCall.then((response) => {
        const menuId = response.data?.menuId || form.value.menuId;
        if (menuId) saveTranslations(menuId);
        ElMessage.success(form.value.menuId ? t('common.editSuccess') : t('common.addSuccess'));
        open.value = false;
        getList();
      });
    }
  });
}

/** 加载菜单已有的多语言翻译 */
async function loadMenuTranslations(menuId) {
  currentTab.value = 'default';
  i18nValues.value = {};
  if (!menuId) return;
  try {
    const res = await listI18n({ params: { i18nKey: `sys.menu.${menuId}` } });
    const rows = res.rows || [];
    rows.forEach(row => {
      i18nValues.value[row.lang] = row.i18nValue;
    });
  } catch { /* ignore */ }
}

/** 保存单条翻译（自动判断新增或更新） */
async function upsertTranslation(menuId, lang, value) {
  if (!value) return;
  const key = `sys.menu.${menuId}`;
  try {
    const res = await listI18n({ params: { i18nKey: key, lang } });
    const rows = res.rows || [];
    const existing = rows.find(r => r.lang === lang);
    if (existing?.i18nId) {
      await updateI18n({ i18nId: existing.i18nId, i18nKey: key, lang, i18nValue: value });
    } else {
      await addI18n({ i18nKey: key, lang, i18nValue: value });
    }
  } catch (e) {
    console.error("保存翻译失败:", e);
  }
}

/** 保存菜单的多语言翻译 */
async function saveTranslations(menuId) {
  const promises = langList.value.map(l =>
    upsertTranslation(menuId, l.langCode, i18nValues.value[l.langCode])
  );
  await Promise.all(promises);
  refreshI18nCache().catch(() => {});
}

/** 删除按钮操作 */
function handleDelete(row) {
  ElMessageBox.confirm(t('menuManage.confirm.delete', { name: row.menuName }), t('common.confirmTitle'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delMenu(row.menuId))
    .then(() => {
      getList();
      ElMessage.success(t('common.deleteSuccess'));
    })
    .catch(() => {});
}

getList();
</script>

<style lang="scss" scoped>

:deep(.el-form-item__label) {
  white-space: normal;
  line-height: 1.5;
  word-break: break-word;
  display: flex;
  align-items: center;

  .el-icon {
    vertical-align: middle;
  }
}

/* 多语言紧凑标签 */
.i18n-inline-form-item :deep(.el-form-item__content) {
  flex-wrap: nowrap;
}
.i18n-inline {
  display: flex;
  flex-direction: column;
  gap: 8px;
  width: 100%;
}
.i18n-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
}
.i18n-tag {
  display: inline-flex;
  align-items: center;
  gap: 3px;
  padding: 2px 8px;
  border-radius: 3px;
  cursor: pointer;
  font-size: 12px;
  line-height: 1.4;
  transition: all 0.2s;
  user-select: none;

  // 未填 — 淡化
  color: #c0c4cc;
  border: 1px dashed #dcdfe6;
  background: #fff;
  opacity: 0.65;

  &:hover {
    border-color: #c0c4cc;
    color: #909399;
    opacity: 1;
  }

  &.active {
    color: #409eff;
    border: 1px solid #409eff;
    background: #ecf5ff;
    opacity: 1;
  }

  &.filled {
    color: #fff;
    border: 1px solid #67c23a;
    background: #67c23a;
    opacity: 1;
  }

  &.active.filled {
    color: #fff;
    border-color: #409eff;
    background: #409eff;
  }
}
.i18n-tag-default {
  margin-right: 8px;
  position: relative;
  &::after {
    content: '';
    position: absolute;
    right: -5px;
    top: 2px;
    bottom: 2px;
    width: 1px;
    background: #dcdfe6;
  }
}
.i18n-tag.filled::after {
  content: '✓';
  font-size: 10px;
  font-weight: bold;
  margin-left: 2px;
}
</style>
