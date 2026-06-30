<template>
  <el-dialog 
    v-model="isShow" 
    append-to-body 
    :title="message.messageTitle"
    width="600px"
    class="msg-detail-dialog"
  >
    <div class="msg-detail-content">
      <div class="msg-body">{{ message.messageBody }}</div>

      <div class="msg-footer">
        <div class="msg-sender-info">
          <div class="msg-sender-name">
            <!-- {{ message.sendUser?.account }}/ -->
            {{ message.sendUser?.userName }}
          </div>
          <div class="msg-send-time">{{ formatTime(message.createTime) }}</div>
        </div>
      </div>
    </div>
  </el-dialog>
</template>

<script setup>
import { ref } from "vue";
import { getMessage } from "@/api/system/message";

const isShow = ref(false);
const message = ref({});

const showDialog = (id) => {
  getMessage(id).then((res) => {
    message.value = res.data;
    isShow.value = true;
  });
};

defineExpose({ showDialog });
</script>

<style lang="scss" scoped>
.msg-detail-dialog {
  :deep(.el-dialog__body) {
    padding: 24px;
  }
}

.msg-detail-content {
  display: flex;
  flex-direction: column;
//   min-height: 200px;
}

.msg-body {
  font-size: 16px;
  line-height: 1.8;
  color: var(--text-primary);
  margin-bottom: 30px;
  white-space: pre-wrap;
  word-break: break-word;
}

.msg-footer {
  display: flex;
  justify-content: flex-end;
  align-items: flex-end;
}

.msg-sender-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  min-width: 200px;
  padding: 12px 16px;
//   background-color: var(--el-fill-color-light);
  border-radius: 4px;
}

.msg-sender-name {
  font-size: 13px;
  color: var(--text-regular);
  margin-bottom: 8px;
}

.msg-send-time {
  font-size: 12px;
  color: var(--text-secondary);
}
</style>