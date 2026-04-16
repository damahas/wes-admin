<template>
  <div class="message-wrapper">
    <!-- <el-badge :value="unreadCount" :hidden="unreadCount === 0" class="message-badge"> -->
    <el-popover
      ref="popoverRef"
      placement="bottom-end"
      :width="450"
      popper-style="--el-popover-padding: 6px;"
      trigger="click"
      v-model:visible="popoverVisible"
    >
      <template #reference>
        <div class="message-trigger" @click="handleMessageClick">
          <i class="fa fa-bell"></i>
        </div>
      </template>

      <div class="message-content">
        <div class="message-header-row">
          <div class="header-title">
            <i class="fa fa-bell"></i>
            <span>{{ t("message.title") }}</span>
          </div>
          <el-dropdown :teleported="false" @command="handleMessageTypeChange">
            <span class="el-dropdown-link">
              {{ getMessageTypeLabel(queryParams.params.messageType) }}
              <i class="fa fa-chevron-down"></i>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="all">
                  {{ t("message.allType") }}
                </el-dropdown-item>
                <el-dropdown-item
                  v-for="item in sys_message_type"
                  :key="item.value"
                  :command="item.value"
                >
                  {{ item.label }}
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
        <div class="message-header-row">
          <div class="message-type-select">
            <div
              class="type-btn"
              :class="{ active: queryParams.params.isRead === -1 }"
              @click="handleTypeChange(-1)"
            >
              {{ t("message.all") }}
            </div>
            <div
              class="type-btn"
              :class="{ active: queryParams.params.isRead === 0 }"
              @click="handleTypeChange(0)"
            >
              {{ t("message.unread") }}
            </div>
            <div
              class="type-btn"
              :class="{ active: queryParams.params.isRead === 1 }"
              @click="handleTypeChange(1)"
            >
              {{ t("message.read") }}
            </div>
          </div>

          <el-input
            v-model="queryParams.params.messageTitle"
            :placeholder="t('message.search')"
            prefix-icon="Search"
            :style="{ width: '160px' }"
            size="small"
            clearable
            @clear="handleSearchClear"
            @input="debounceSearch"
          />
        </div>

        <div class="message-header-row message-actions">
          <el-checkbox v-model="selectAll" @change="handleSelectAll">
            {{ t("message.selectAll") }}
          </el-checkbox>
          <div class="action-right">
            <span
              class="action-btn"
              style="color: var(--el-color-primary)"
              @click="markAllRead"
            >
              {{ t("message.markAllRead") }}
            </span>
            <span class="action-btn" @click="markRead">{{ t("message.markRead") }}</span>
            <span class="action-btn" @click="deleteSelected">
              {{ t("message.delete") }}
            </span>
          </div>
        </div>

        <div class="message-list" @scroll="handleScroll">
          <div
            v-for="msg in messageList"
            :key="msg.messageId"
            class="message-item"
            :class="{ unread: !msg.isRead }"
            @click="handleClick(msg)"
          >
            <el-checkbox v-model="msg.selected" class="item-check" @click.stop />

            <div class="message-item-type-img" :style="getMessageTypeStyle(msg)">
              <i
                :style="getMessageIconStyle(msg)"
                :class="'fa ' + getMessageType(msg)?.elTagClass"
              ></i>
            </div>

            <div class="message-item-content">
              <div class="message-item-top">
                <div class="title-box-left">
                  <div v-if="!msg.isRead" class="message-item-read-dot"></div>
                  <div class="message-item-cat-name">
                    【{{ getMessageType(msg)?.label || msg.type }}】
                  </div>
                  <el-tag
                    v-if="msg.status"
                    :type="msg.statusType"
                    size="small"
                    style="margin-left: 4px"
                  >
                    {{ msg.status }}
                  </el-tag>
                </div>
                <div class="title-box-right">
                  <div class="message-item-time">{{ msg.time }}</div>
                </div>
              </div>

              <div class="message-item-title">{{ msg.messageTitle }}</div>
            </div>
          </div>

          <div v-if="messageList.length === 0" class="empty-state">
            <el-empty :image-size="80" :description="t('message.empty')" />
          </div>

          <div v-if="loading && messageList.length > 0" class="loading-more">
            <span>{{ t("message.loading") }}</span>
          </div>

          <div v-if="!hasMore && messageList.length > 0" class="no-more">
            <span>{{ t("message.noMore") }}</span>
          </div>
        </div>
      </div>
    </el-popover>
    <!-- </el-badge> -->
    <MessageDialog ref="messageDialogRef" />
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import { getDict } from "@/utils";
import {
  listMessage,
  readMessage,
  readAllMessage,
  readMessages,
  delMessage,
} from "@/api/system/message";
import MessageDialog from "./dialog.vue";

const DEBOUNCE_DELAY = 300;
let searchTimer = null;

const { t } = useI18n();
const router = useRouter();
const { sys_message_type } = getDict("sys_message_type");
const messageDialogRef = ref(null);
const popoverVisible = ref(false);

const queryParams = ref({
  pageNum: 1,
  pageSize: 6,
  params: {
    messageType: "",
    messageTitle: "",
    isRead: -1,
  },
});

const selectAll = ref(false);
const messageList = ref([]);
const loading = ref(false);
const hasMore = ref(true);

const getMessageType = (msg) => {
  return sys_message_type.value.find((p) => p.value === msg.messageType) ?? {};
};

const getMessageTypeLabel = (value) => {
  if (!value) {
    return t("message.allType");
  }
  const type = sys_message_type.value.find((p) => p.value === value);
  return type?.label || value;
};

const getMessageTypeStyle = (msg) => {
  const type = getMessageType(msg);
  const colorType = type?.elTagType || "primary";
  return {
    backgroundColor: `var(--el-color-${colorType}-light-9)`,
    borderColor: `var(--el-color-${colorType}-light-3)`,
  };
};

const getMessageIconStyle = (msg) => {
  const type = getMessageType(msg);
  const colorType = type?.elTagType || "primary";
  return {
    color: `var(--el-color-${colorType}-light-3)`,
  };
};

const getList = (append = false) => {
  if (loading.value) return;

  loading.value = true;
  listMessage(queryParams.value).then((res) => {
    if (append) {
      messageList.value = [...messageList.value, ...res.rows];
    } else {
      messageList.value = res.rows;
    }
    hasMore.value = res.rows.length >= queryParams.value.pageSize;
    loading.value = false;
  });
};

const handleMessageClick = () => {
  queryParams.value.pageNum = 1;
  messageList.value = [];
  hasMore.value = true;
  getList();
};

const debounceSearch = () => {
  if (searchTimer) {
    clearTimeout(searchTimer);
  }
  searchTimer = setTimeout(() => {
    queryParams.value.pageNum = 1;
    messageList.value = [];
    hasMore.value = true;
    getList();
  }, DEBOUNCE_DELAY);
};

const handleSearchClear = () => {
  queryParams.value.pageNum = 1;
  messageList.value = [];
  hasMore.value = true;
  getList();
};

const handleTypeChange = (type) => {
  queryParams.value.params.isRead = type;
  queryParams.value.pageNum = 1;
  selectAll.value = false;
  messageList.value = [];
  hasMore.value = true;
  getList();
};

const handleMessageTypeChange = (value) => {
  if (value == "all") {
    queryParams.value.params.messageType = undefined;
  } else {
    queryParams.value.params.messageType = value;
  }
  queryParams.value.pageNum = 1;
  messageList.value = [];
  hasMore.value = true;
  getList();
};

const handleSelectAll = (val) => {
  messageList.value.forEach((msg) => {
    msg.selected = val;
  });
};

const markAllRead = () => {
  readAllMessage().then(() => {
    ElMessage.success(t("message.allMarkedAsRead"));
    getList();
  });
};

const markRead = () => {
  const selected = messageList.value.filter((msg) => msg.selected);
  if (selected.length === 0) {
    ElMessage.warning(t("message.pleaseSelect"));
    return;
  }
  readMessages(selected.map((p) => p.messageId)).then(() => {
    getList();
    ElMessage.success(t("message.markedAsRead"));
  });
};

const deleteSelected = () => {
  const selected = messageList.value.filter((msg) => msg.selected);
  if (selected.length === 0) {
    ElMessage.warning(t("message.pleaseSelect"));
    return;
  }
  delMessage(selected.map((p) => p.messageId).join(",")).then(() => {
    getList();
    ElMessage.success(t("message.deleted"));
  });
};

const handleScroll = (e) => {
  const { scrollTop, scrollHeight, clientHeight } = e.target;
  const scrollBottom = scrollHeight - scrollTop - clientHeight;

  if (scrollBottom < 50 && hasMore.value) {
    queryParams.value.pageNum++;
    getList(true);
  }
};

const handleClick = (row) => {
  readMessage(row.messageId);
  row.isRead = 1;
  if (row.openType === "dialog") {
    messageDialogRef.value.showDialog(row.messageId);
    popoverVisible.value = false;
    return;
  }
  if (row.openType === "out") {
    window.open(row.messageBody);
    popoverVisible.value = false;
    return;
  }
  if (row.openType === "in") {
    router.push(row.messageBody);
    popoverVisible.value = false;
    return;
  }
};

onMounted(() => {
  getList();
});
</script>

<style lang="scss" scoped>
.message-wrapper {
  display: flex;
  align-items: center;

  i {
    font-size: 15px;
    color: var(--menu-item-icon-color);
  }
}

.message-badge {
  display: flex;
  align-items: center;
}

.message-trigger {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  cursor: pointer;
  transition: background-color 0.3s;
}

.message-trigger:hover {
  background-color: var(--el-fill-color-light);
}

.message-content {
  padding: 0;
}

.message-header-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 6px 13px;

  i {
    font-size: 15px;
    color: var(--menu-item-icon-color);
  }
}

.header-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
}

.message-type-select {
  display: flex;
  align-items: center;
  gap: 8px;
}

.type-btn {
  padding: 4px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.3s;
  color: var(--text-regular);
}

.type-btn:hover {
  background-color: var(--el-fill-color-light);
}

.type-btn.active {
  background-color: var(--theme-color);
  color: white;
}

.message-actions {
  background-color: var(--el-fill-color-light);
  border-radius: 4px;

  .action-right {
    display: flex;
    align-items: center;
    gap: 16px;
  }
  .action-btn {
    cursor: pointer;
    font-size: 13px;
    color: var(--widgets-button-backgroundColor-ma-col);
    transition: color 0.3s;
  }

  .action-btn:hover {
    color: var(--theme-color);
  }
}

.message-list {
  max-height: 400px;
  overflow-y: auto;
}

.message-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 12px;
  border-bottom: 1px solid var(--el-border-color-lighter);
  transition: background-color 0.3s;
  background-color: var(--el-bg-color);
  cursor: pointer;
}
.message-item:hover {
  background-color: var(--el-fill-color-light);
}

.item-check {
  margin-top: 4px;
}
.message-item-type-img {
  border-radius: 50%;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.message-item-content {
  flex: 1;
  min-width: 0;
}

.message-item-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 4px;
}

.title-box-left {
  display: flex;
  align-items: center;
  gap: 4px;
}

.message-item-cat-name {
  font-size: 13px;
  color: var(--text-regular);
}

.message-item-read-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background-color: #fe8522;
}

.title-box-right {
  display: flex;
  align-items: center;
}

.message-item-time {
  font-size: 12px;
  color: var(--text-secondary);
}

.message-item-title {
  margin: 4px 0;
  font-size: 14px;
  color: var(--text-primary);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.loading-more,
.no-more {
  padding: 12px;
  text-align: center;
  font-size: 13px;
  color: var(--text-secondary);
}

.loading-more span,
.no-more span {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}
</style>
