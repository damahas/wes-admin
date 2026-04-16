<template>
  <div>
    <el-button
      :type="buttonType"
      :plain="plain"
      :icon="icon"
      @click="handleImport"
      v-if="showButton"
    >
      {{ buttonText }}
    </el-button>
    <el-dialog :title="title" v-model="visible" width="400px" append-to-body>
      <el-upload
        ref="uploadRef"
        :limit="1"
        accept=".xlsx, .xls"
        :headers="headers"
        :action="uploadUrl + '?updateSupport=' + updateSupport"
        :disabled="isUploading"
        :on-progress="handleFileUploadProgress"
        :on-success="handleFileSuccess"
        :on-error="handleFileError"
        :auto-upload="false"
        drag
      >
        <el-icon class="el-icon--upload"><upload-filled /></el-icon>
        <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
        <template #tip>
          <div class="el-upload__tip text-center">
            <div class="el-upload__tip" v-if="showUpdateSupport">
              <el-checkbox v-model="updateSupport" />是否更新已经存在的数据
            </div>
            <span v-if="acceptTip">{{ acceptTip }}</span>
            <el-link
              v-if="templateUrl"
              type="primary"
              :underline="false"
              style="font-size: 12px; vertical-align: baseline"
              @click="handleDownloadTemplate"
            >
              下载模板
            </el-link>
          </div>
        </template>
      </el-upload>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="handleUpload">确 定</el-button>
          <el-button @click="handleCancel">取 消</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import store from "@/store";
import { UploadFilled } from "@element-plus/icons-vue";
import { download } from "@/utils";

const props = defineProps({
  // 按钮配置
  buttonText: {
    type: String,
    default: "导入",
  },
  buttonType: {
    type: String,
    default: "info",
  },
  plain: {
    type: Boolean,
    default: true,
  },
  icon: {
    type: String,
    default: "Upload",
  },
  showButton: {
    type: Boolean,
    default: true,
  },
  // 上传配置
  uploadUrl: {
    type: String,
    required: true,
  },
  templateUrl: {
    type: String,
    default: "",
  },
  title: {
    type: String,
    default: "导入",
  },
  acceptTip: {
    type: String,
    default: "仅允许导入xls、xlsx格式文件。",
  },
  showUpdateSupport: {
    type: Boolean,
    default: false,
  },
  // 自定义请求头
  customHeaders: {
    type: Object,
    default: null,
  },
});

const emit = defineEmits(["success", "error"]);

const uploadRef = ref(null);
const visible = ref(false);
const isUploading = ref(false);
const updateSupport = ref(0);

const headers = computed(() => {
  if (props.customHeaders) {
    return props.customHeaders;
  }
  return { Authorization: "Bearer " + store.getters["user/accessToken"] };
});

function handleImport() {
  visible.value = true;
  updateSupport.value = 0;
}

function handleFileUploadProgress() {
  isUploading.value = true;
}

function handleFileSuccess(response) {
  visible.value = false;
  isUploading.value = false;
  uploadRef.value?.clearFiles();
  emit("success", response);
}

function handleFileError(error) {
  visible.value = false;
  isUploading.value = false;
  uploadRef.value?.clearFiles();
  emit("error", error);
}

function handleUpload() {
  uploadRef.value?.submit();
}

function handleCancel() {
  visible.value = false;
}

function handleDownloadTemplate() {
  if (props.templateUrl) {
    download(props.templateUrl, {}, `user_template_${new Date().getTime()}.xlsx`);
  }
}

defineExpose({
  handleImport,
});
</script>

<style scoped>
.text-center {
  text-align: center;
}
</style>
