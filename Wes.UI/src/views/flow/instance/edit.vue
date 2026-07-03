<template>
  <div class="app-container">
    <!-- 添加或修改数据对话框 -->
    <el-dialog :title="title" v-model="open" width="800px" append-to-body>
      <el-descriptions border>
        <el-descriptions-item :label="t('flow.instance.businessId')">
          {{ form.businessId }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('flow.instance.businessCode')" :span="2">
          {{ form.businessCode }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('flow.instance.flowName')">
          {{ form.process?.processName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('flow.instance.flowVersion')">
          {{ form.version?.version }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('flow.instance.initiator')">
          {{ getName(form.createUser) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('flow.instance.flowStatus')">
          <status-tag :status="form.instanceStatus" />
        </el-descriptions-item>
        <el-descriptions-item :label="t('flow.instance.isUrgent')">
          {{ form.isUrgent > 0 ? t('flow.instance.urgentYes') : t('flow.instance.normal') }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('flow.instance.createTime')">
          {{ form.createTime }}
        </el-descriptions-item>
      </el-descriptions>

      <div style="margin-top: 30px">
        <el-timeline>
          <el-timeline-item
            v-for="node in form.nodes"
            :key="node.instanceNodeId"
            :icon="getNodeIcon(node.nodeResult)"
            :color="getNodeColor(node.nodeResult)"
            size="large"
          >
            <div>
              <span class="node-title">{{ node.nodeName }}</span>
              <span class="node-sub-title">{{ node.createTime }}</span>
            </div>
            <div v-if="node.nodeType != 'end'">
              <el-card
                style="margin-top: 12px"
                :body-style="{ padding: '10px' }"
                v-for="task in node.tasks"
                :key="task.instanceTaskId"
              >
                <div style="margin: 6px 0" v-if="node.nodeType == 'start'">
                  {{ t('flow.instance.initiator') }}：{{ getName(task.actualUser) }}
                </div>
                <div style="margin: 6px 0" v-if="node.nodeType == 'task'">
                  <div style="display: flex">
                    {{ t('flow.instance.handler') }}：{{ getName(task.actualUser) }}
                    <status-tag :status="task.taskResult" style="margin-left: 8px" />
                  </div>
                  <div style="margin-top: 9px">
                    {{ t('flow.instance.approvalComment') }}：{{ task.comments }}
                    <el-button
                      size="small"
                      type="primary"
                      style="float: right"
                      v-if="task.taskResult === 0"
                      @click="handleDelegate(task)"
                    >
                      {{ t('flow.instance.delegate') }}
                    </el-button>
                  </div>
                </div>
                <div style="margin: 6px 0" v-if="node.nodeType == 'notice'">
                  {{ t('flow.instance.viewer') }}：{{ getName(task.actualUser) }}
                </div>
              </el-card>
            </div>
          </el-timeline-item>
        </el-timeline>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="cancel">{{ t('flow.instance.confirm') }}</el-button>
        </div>
      </template>
    </el-dialog>

    <select-user ref="selectUserRef" :isSingle="true" @handleData="handleSelectUser" />
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { getInstance, delegateTask } from "@/api/flow/instance";
import statusTag from "./statusTag.vue";
import selectUser from "@/views/system/user/select.vue";

const { t } = useI18n();

const selectUserRef = ref(null);

const title = ref("");
const open = ref(false);
const form = ref({
  createUser: {},
  process: {},
  version: {},
});
const curTask = ref({});

function cancel() {
  open.value = false;
  reset();
}

function reset() {
  form.value = {
    createUser: {},
    process: {},
    version: {},
  };
}

/** 打开编辑页面 */
function openDialog(id) {
  reset();
  handleGet(id);
}

function handleGet(id) {
  getInstance(id).then((response) => {
    form.value = response.data;
    title.value = `${t('flow.instance.title')}-${form.value.businessCode}`;
    open.value = true;
  });
}

function handleDelegate(task) {
  curTask.value = task;
  selectUserRef.value.open();
}

function handleSelectUser(rows) {
  if (!rows || !rows.length) {
    return false;
  }
  delegateTask(curTask.value.instanceTaskId, rows[0].userId).then(() => {
    handleGet(form.value.instanceId);
  });
}

function getNodeIcon(nodeResult) {
  switch (nodeResult) {
    case 0:
    case 10:
      return "el-icon-more";
    case 100:
      return "el-icon-circle-check";
    case 101:
      return "el-icon-circle-close";
    case 200:
    case 201:
      return "el-icon-warning-outline";
    default:
      return "";
  }
}

function getNodeColor(nodeResult) {
  switch (nodeResult) {
    case 0:
    case 10:
      return "#909399";
    case 100:
      return "#67C23A";
    case 101:
      return "#F56C6C";
    case 200:
    case 201:
      return "#E6A23C";
    default:
      return "";
  }
}

function getName(user) {
  return user?.userName;
}

defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
.node-title {
  color: var(--text-primary);
  font-size: 14px;
  font-weight: 600;
}

.node-sub-title {
  margin-left: 16px;
  color: var(--text-primary);
  font-size: 12px;
}
</style>
