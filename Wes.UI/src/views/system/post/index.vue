<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form
        :config="queryConfig"
        v-model:visible="showSearch"
        v-model="queryParams.params"
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
            v-hasPermi="['system:post:add']"
            >{{ t('common.add') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate"
            v-hasPermi="['system:post:edit']"
            >{{ t('common.edit') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
            v-hasPermi="['system:post:remove']"
            >{{ t('common.delete') }}</el-button
          >
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="warning"
            plain
            icon="Download"
            @click="handleExport"
            v-hasPermi="['system:post:export']"
            >{{ t('common.export') }}</el-button
          >
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table
        v-loading="loading"
        :data="postList"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('post.postCode')" align="center" prop="postCode" />
        <el-table-column :label="t('post.postName')" align="center" prop="postName" />
        <el-table-column :label="t('post.postSort')" align="center" prop="postSort" />
        <el-table-column :label="t('common.status')" align="center" prop="status">
          <template #default="scope">
            <dict-tag :options="sys_normal_disable" :value="scope.row.status" />
          </template>
        </el-table-column>
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" width="180">
          <template #default="scope">
            <span>{{ formatTime(scope.row.createTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          :label="t('common.actions')"
          width="180"
          align="center"
          class-name="small-padding fixed-width"
        >
          <template #default="scope">
            <el-button
              link
              type="primary"
              icon="Edit"
              @click="handleUpdate(scope.row)"
              v-hasPermi="['system:post:edit']"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              link
              type="danger"
              icon="Delete"
              @click="handleDelete(scope.row)"
              v-hasPermi="['system:post:remove']"
            >
              {{ t('common.delete') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination
        v-show="total > 0"
        :total="total"
        v-model:page="queryParams.pageNum"
        v-model:limit="queryParams.pageSize"
        @pagination="getList"
      />
    </div>

    <!-- 添加或修改岗位对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" append-to-body>
      <el-form ref="postRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item :label="t('post.postName')" prop="postName">
          <el-input v-model="form.postName" :placeholder="t('post.placeholder.postName')" />
        </el-form-item>
        <el-form-item :label="t('post.postCode')" prop="postCode">
          <el-input v-model="form.postCode" :placeholder="t('post.placeholder.postCode')" />
        </el-form-item>
        <el-form-item :label="t('post.postSort')" prop="postSort">
          <el-input-number v-model="form.postSort" controls-position="right" :min="0" />
        </el-form-item>
        <el-form-item :label="t('post.postStatus')" prop="status">
          <el-radio-group v-model="form.status">
            <el-radio
              v-for="dict in sys_normal_disable"
              :key="dict.value"
              :value="dict.value"
              >{{ dict.label }}</el-radio
            >
          </el-radio-group>
        </el-form-item>
        <el-form-item :label="t('common.remark')" prop="remark">
          <el-input v-model="form.remark" type="textarea" :placeholder="t('common.pleaseInput')" />
        </el-form-item>
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

<script setup name="Post">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import { download, getDict } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import { listPost, addPost, delPost, getPost, updatePost } from "@/api/system/post";

const { t } = useI18n();
const { sys_normal_disable } = getDict("sys_normal_disable");
const postRef = ref(null);

// 查询条件配置
const queryConfig = computed(() => [
  {
    label: t('post.postCode'),
    prop: "postCode",
    type: "input",
    placeholder: t('post.placeholder.postCode'),
  },
  {
    label: t('post.postName'),
    prop: "postName",
    type: "input",
    placeholder: t('post.placeholder.postName'),
  },
  {
    label: t('common.status'),
    prop: "status",
    type: "select",
    placeholder: t('post.placeholder.status'),
    options: sys_normal_disable,
  },
]);

const postList = ref([]);
const open = ref(false);
const loading = ref(true);
const showSearch = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      postCode: undefined,
      postName: undefined,
      status: undefined,
    },
  },
  rules: {
    postName: [{ required: true, message: computed(() => t('post.rules.postNameRequired')), trigger: "blur" }],
    postCode: [{ required: true, message: computed(() => t('post.rules.postCodeRequired')), trigger: "blur" }],
    postSort: [{ required: true, message: computed(() => t('post.rules.postSortRequired')), trigger: "blur" }],
  },
});

const { queryParams, form, rules } = toRefs(data);

/** 查询岗位列表 */
function getList() {
  loading.value = true;
  listPost(queryParams.value).then((response) => {
    postList.value = response.rows;
    total.value = response.total;
    loading.value = false;
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
    postId: undefined,
    postCode: undefined,
    postName: undefined,
    postSort: 0,
    status: "0",
    remark: undefined,
  };
  if (postRef.value) postRef.value.resetFields();
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  handleQuery();
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.postId);
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}

/** 新增按钮操作 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = t('post.addTitle');
}

/** 修改按钮操作 */
function handleUpdate(row) {
  reset();
  const postId = row.postId || ids.value;
  getPost(postId).then((response) => {
    form.value = response.data;
    open.value = true;
    title.value = t('post.editTitle');
  });
}

/** 提交按钮 */
function submitForm() {
  postRef.value?.validate((valid) => {
    if (valid) {
      const apiCall = form.value.postId ? updatePost(form.value) : addPost(form.value);
      apiCall.then(() => {
        ElMessage.success(form.value.postId ? t('common.editSuccess') : t('common.addSuccess'));
        open.value = false;
        getList();
      });
    }
  });
}

/** 删除按钮操作 */
function handleDelete(row) {
  const postIds = row.postId || ids.value;
  const formNames = postList.value
    .filter((p) => postIds.includes(p.postId))
    .map((p) => p.postName);
  ElMessageBox.confirm(t('post.confirm.delete', { names: formNames.join(", ") }), t('common.confirmTitle'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    confirmButtonType: "danger",
    type: "warning",
  })
    .then(() => delPost(postIds))
    .then(() => {
      getList();
      ElMessage.success(t('common.deleteSuccess'));
    })
    .catch(() => {});
}

/** 导出按钮操作 */
function handleExport() {
  download(
    "system/post/export",
    {
      ...queryParams.value,
    },
    `post_${new Date().getTime()}.xlsx`
  );
}

getList();
</script>
