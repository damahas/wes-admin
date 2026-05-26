<template>
  <el-dialog
    v-model="dialogVisible"
    title="流程设计"
    :fullscreen="true"
    :show-close="false"
    @close="handleClose"
  >
    <template #header>
      <div class="flow-dialog-header">
        <span>
          流程设计--{{ form.process?.processName }}-
          <el-select
            v-model="form.versionId"
            placeholder="请选择版本"
            size="default"
            style="margin-left: 12px; width: 160px"
            @change="handleVersionChange"
          >
            <el-option
              v-for="item in versionList"
              :key="item.versionId"
              :label="item.version"
              :value="item.versionId"
            >
              <span v-if="item.isLock == 1">
                <i class="fa fa-lock" style="margin-right: 4px; color: #909399"></i>
                {{ item.version }}
              </span>
              <span v-else>{{ item.version }}</span>
            </el-option>
          </el-select>
        </span>
        <el-alert
          title="当前版本已锁定，不可修改"
          size="small"
          v-if="form.isLock == 1"
          type="warning"
          style="width: auto"
          :closable="false"
          center
        />
        <div class="header-actions">
          <el-button type="info" @click="handleVersion" link> 新建版本 </el-button>
          <el-button type="primary" @click="handleSave" v-if="form.isLock == 0">
            保存流程
          </el-button>
          <el-button type="danger" @click="handleDelete" v-if="form.isLock == 0">
            删除流程
          </el-button>
          <el-button @click="handleReset" v-if="form.isLock == 0"> 重 置 </el-button>
          <el-button @click="handleClose">关 闭</el-button>
        </div>
      </div>
    </template>

    <el-container class="flow-container">
      <el-aside width="180px" class="flow-nodes">
        <div class="nodes-header">
          <i class="fa fa-sitemap"></i>
          <span>流程节点</span>
        </div>
        <div class="nodes-content">
          <div v-for="item in flowNodes" :key="item.groupName" class="node-group">
            <div class="node-group-title">{{ item.groupName }}</div>
            <div class="node-group-items">
              <div
                v-for="node in item.nodes"
                :key="node.type"
                class="node-item"
                :class="{ 'is-disabled': form.isLock == 1 }"
                @mousedown="(e) => !form.isLock && onNodeDragStart(e, node)"
              >
                <div class="node-icon" :style="{ 'background-color': node.color }">
                  <i :class="'fa ' + node.icon"></i>
                </div>
                <span class="node-name">{{ node.name }}</span>
              </div>
            </div>
          </div>
        </div>
      </el-aside>

      <el-main>
        <div ref="flowContainer" class="route-right-flow">
          <div id="flow-canvas"></div>
        </div>
      </el-main>

      <el-aside width="280px">
        <node-attr
          class="route-right-attr"
          :type="currentType"
          :element="flowElement"
          :readonly="form.isLock == 1"
        />
      </el-aside>
    </el-container>

    <!-- 右键菜单 -->
    <ContextMenu
      v-model:visible="contextMenuVisible"
      :x="contextMenuX"
      :y="contextMenuY"
      :menus="contextMenus"
      @select="handleContextSelect"
    />
  </el-dialog>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, nextTick, reactive } from "vue";
import { Graph, Snapline, Dnd } from "@antv/x6";
import { ElMessageBox, ElMessage } from "element-plus";
import { getVersion, updateVersion, listVersion, delVersion } from "@/api/flow/process";
import { flowNodes, traits, ports } from "./config";
import nodeAttr from "./components/nodeAttr.vue";
import FlowNodeComponent from "./components/customNode/index.vue";
import { register } from "@antv/x6-vue-shape";
import ContextMenu from "@/components/ContextMenu/index.vue";

const emit = defineEmits(["close"]);

// 注册 Vue 组件节点
register({
  shape: "flow-node-vue",
  width: 120,
  height: 40,
  component: FlowNodeComponent,
  ports,
});

const flowContainer = ref(null);
let graph = null;
let dnd = null;

const dialogVisible = ref(false);
const form = ref({});
const currentType = ref("flow");
const flowElement = ref({});
const versionList = ref([]);
const selectedVersionId = ref("");

// 右键菜单
const contextMenuVisible = ref(false);
const contextMenuX = ref(0);
const contextMenuY = ref(0);
const contextMenus = ref([]);

// 显示右键菜单
function showContextMenu(x, y, menus) {
  contextMenuX.value = x;
  contextMenuY.value = y;
  contextMenus.value = menus;
  contextMenuVisible.value = true;
}

// 执行菜单操作
function handleContextSelect(item) {
  if (item.action) item.action();
}

function open(v) {
  dialogVisible.value = true;
  nextTick(() => {
    initGraph();
    loadVersionList(v.processId);
    handleGetData(v.versionId);
  });
}

// 加载版本列表
function loadVersionList(processId) {
  listVersion({ params: { processId: processId } }).then((response) => {
    versionList.value = response.rows || [];
  });
}

// 版本切换
function handleVersionChange(versionId) {
  if (!versionId) return;
  // 清空画布
  if (graph) {
    graph.clearCells();
  }
  // 加载选中版本数据
  handleGetData(versionId);
}

// Dnd 方式拖拽节点开始
function onNodeDragStart(e, node) {
  if (!dnd || !graph) return;

  const nodeTraits = traits[node.type];
  if (!nodeTraits) return;

  // 创建节点
  const dragNode = graph.createNode({
    width: 120,
    height: 40,
    shape: "flow-node-vue",
    data: {
      type: node.type,
      name: nodeTraits.label,
      icon: nodeTraits.icon,
      color: nodeTraits.color,
      meta: { ...(nodeTraits.defaultValue ?? {}) },
    },
  });

  dnd.start(dragNode, e);
}

function initGraph() {
  const container = document.getElementById("flow-canvas");
  if (!flowContainer.value || !container) return;
  if (graph) {
    graph.clearCells();
    return;
  }

  graph = new Graph({
    container: container,
    width: container.clientWidth || 800,
    height: container.clientHeight || 600,
    background: {
      color: "#f5f7fa",
    },
    grid: {
      size: 20,
      visible: true,
      type: "dot",
    },
    panning: true,
    mousewheel: {
      enabled: true,
      modifiers: ["ctrl", "meta"],
    },
    connecting: {
      snap: true,
      anchor: "center",
      connectionPoint: "anchor",
      allowBlank: false,
      allowLoop: false,
      allowEdge: false,
      allowNode: false,
      allowPort: true,
      highlight: true,
      showConnectSpot: true,
      connector: "rounded",
      router: "manhattan",
      createEdge() {
        return this.createEdge({
          attrs: {
            line: {
              stroke: "#c6c9ce",
              strokeWidth: 2,
              targetMarker: {
                name: "classic",
                size: 8,
              },
            },
          },
        });
      },
      validateEdge({ edge }) {
        // 检查是否已存在相同连线
        const source = edge.getSourceCellId();
        const target = edge.getTargetCellId();
        const edges = graph.getEdges().filter((e) => e.id !== edge.id);
        const exists = edges.some(
          (e) => e.getSourceCellId() === source && e.getTargetCellId() === target
        );
        return !exists;
      },
    },
    selecting: {
      enabled: true,
      showNodeSelectionBox: true,
      rubberEdge: true,
      rubberNode: true,
    },
    keyboard: {
      enabled: true,
    },
    interacting() {
      return form.value?.isLock != 1;
    },
  });

  // 使用对齐线插件
  graph.use(
    new Snapline({
      enabled: true,
      sharp: true,
    })
  );

  // 初始化 Dnd 插件
  dnd = new Dnd({
    target: graph,
  });

  // 监听节点添加到画布
  graph.on("node:added", ({ node }) => {
    const type = node.getData()?.type || "task";
    if (type) {
      nextTick(() => {
        graph.cleanSelection();
        graph.select(node);
        currentType.value = type;
        flowElement.value = reactive(node.getData());
      });
    }
  });

  // 节点点击选中
  graph.on("node:click", ({ node }) => {
    const type = node.getData()?.type || "task";
    if (type) {
      nextTick(() => {
        graph.cleanSelection();
        graph.select(node);
        currentType.value = type;
        flowElement.value = reactive(node.getData());
      });
    }
  });

  // 边点击选中
  graph.on("edge:click", ({ edge }) => {
    currentType.value = "line";
    flowElement.value = edge;
  });

  // 连线完成事件
  graph.on("edge:connected", ({ edge }) => {
    currentType.value = "line";
    flowElement.value = edge;
  });

  // 画布点击
  graph.on("blank:click", () => {
    currentType.value = "flow";
    flowElement.value = form.value;
  });

  // 节点右键菜单
  graph.on("node:contextmenu", ({ e, node }) => {
    e.preventDefault();
    showContextMenu(e.clientX, e.clientY, [
      {
        key: "delete",
        label: "删除",
        icon: "fa fa-trash",
        action: () => node.remove(),
      },
    ]);
  });

  // 边右键菜单
  graph.on("edge:contextmenu", ({ e, edge }) => {
    e.preventDefault();
    showContextMenu(e.clientX, e.clientY, [
      {
        key: "delete",
        label: "删除",
        icon: "fa fa-trash",
        action: () => edge.remove(),
      },
    ]);
  });

  // 节点移动事件 - 更新连线连接桩位置
  graph.on("node:moved", ({ node }) => {
    updateConnectedEdges(node);
  });

  // 节点移动时更新所有相关连线的连接桩
  function updateConnectedEdges(movedNode) {
    if (!graph) return;

    const movedNodeId = movedNode.id;

    // 获取所有与该节点相连的边
    const connectedEdges = graph.getEdges().filter((edge) => {
      const sourceId = edge.getSourceCellId();
      const targetId = edge.getTargetCellId();
      return sourceId === movedNodeId || targetId === movedNodeId;
    });

    connectedEdges.forEach((edge) => {
      const sourceId = edge.getSourceCellId();
      const targetId = edge.getTargetCellId();

      let sourceNode, targetNode;

      if (sourceId === movedNodeId) {
        sourceNode = movedNode;
        targetNode = graph.getCellById(targetId);
      } else {
        sourceNode = graph.getCellById(sourceId);
        targetNode = movedNode;
      }

      if (!sourceNode || !targetNode) return;

      // 根据 group 名查找节点上对应连接桩的实际 id
      function getPortIdByGroup(node, group) {
        const port = node.getPorts().find((p) => p.group === group);
        return port?.id || group;
      }

      const sourceBBox = sourceNode.getBBox();
      const targetBBox = targetNode.getBBox();

      // 计算两个节点之间的相对位置
      const sourceCenterX = sourceBBox.x + sourceBBox.width / 2;
      const sourceCenterY = sourceBBox.y + sourceBBox.height / 2;
      const targetCenterX = targetBBox.x + targetBBox.width / 2;
      const targetCenterY = targetBBox.y + targetBBox.height / 2;

      // 计算相对角度
      const dx = targetCenterX - sourceCenterX;
      const dy = targetCenterY - sourceCenterY;

      let sourcePortGroup, targetPortGroup;

      // 根据相对位置确定连接桩分组
      if (Math.abs(dx) > Math.abs(dy)) {
        // 水平方向为主
        if (dx > 0) {
          sourcePortGroup = "right";
          targetPortGroup = "left";
        } else {
          sourcePortGroup = "left";
          targetPortGroup = "right";
        }
      } else {
        // 垂直方向为主
        if (dy > 0) {
          sourcePortGroup = "bottom";
          targetPortGroup = "top";
        } else {
          sourcePortGroup = "top";
          targetPortGroup = "bottom";
        }
      }

      // 更新边的连接桩（使用实际 port id）
      edge.setSource({
        cell: sourceNode.id,
        port: getPortIdByGroup(sourceNode, sourcePortGroup),
      });
      edge.setTarget({
        cell: targetNode.id,
        port: getPortIdByGroup(targetNode, targetPortGroup),
      });
    });
  }

  // 删除键删除选中节点/边
  graph.bindKey("delete", () => {
    const cells = graph.getSelectedCells();
    if (cells.length) {
      graph.removeCells(cells);
    }
  });
}

function allowDrop(e) {
  e.preventDefault();
}

// 添加手动连线功能
function addEdge(sourceId, targetId) {
  if (!graph || sourceId === targetId) return;

  // 检查是否已存在连线
  const edges = graph.getEdges();
  const exists = edges.some(
    (edge) => edge.getSourceCellId() === sourceId && edge.getTargetCellId() === targetId
  );

  if (!exists) {
    graph.addEdge({
      source: sourceId,
      target: targetId,
      attrs: {
        line: { stroke: "#c6c9ce", strokeWidth: 2 },
      },
    });
  }
}

function handleGetData(versionId) {
  getVersion(versionId).then((response) => {
    form.value = response.data;
    form.value.enableVersionId = "";
    if (form.value.versionId != form.value?.process?.curVersionId) {
      form.value.enableVersionId = form.value.versionId;
    }
    flowElement.value = form.value;
    currentType.value = "flow";

    if (response.data.content) {
      const { nodes, lines } = JSON.parse(response.data.content);
      loadGraphData(nodes || [], lines || []);
    }
  });
}

function loadGraphData(nodes, lines) {
  if (!graph) return;

  // 加载节点
  nodes.forEach((nodeData) => {
    const nodeType = nodeData.type || "task";
    const nodeTraits = traits[nodeType] || {};

    graph.addNode({
      id: nodeData.id,
      x: nodeData.x,
      y: nodeData.y,
      width: 120,
      height: 40,
      shape: "flow-node-vue",
      data: {
        type: nodeType,
        name: nodeTraits.label,
        icon: nodeTraits.icon,
        color: nodeTraits.color,
        meta: nodeData.meta,
      },
    });
  });

  const graphNodes = graph.getNodes();

  const findPort = (nodeId, portGroup) => {
    return graphNodes
      .find((p) => p.id === nodeId)
      ?.getPorts()
      ?.find((p) => p.group === portGroup)?.id;
  };

  // 加载连线
  lines.forEach((lineData) => {
    graph.addEdge({
      id: lineData.id,
      source: {
        cell: lineData.source,
        port: findPort(lineData.source, lineData.sourcePoint),
      },
      target: {
        cell: lineData.target,
        port: findPort(lineData.target, lineData.targetPoint),
      },
      attrs: {
        line: { stroke: "#c6c9ce", strokeWidth: 2 },
      },
      data: lineData.meta,
      labels: lineData.meta?.name
        ? [{ attrs: { text: { text: lineData.meta.name } } }]
        : [],
    });
  });

  // 默认选中 & 居中
  nextTick(() => {
    currentType.value = "flow";
    flowElement.value = form.value;
    graph.centerContent();
  });
}

function getGraphData() {
  if (!graph) return;

  const graphData = graph.toJSON();
  // 转换数据格式
  const graphNodes = graphData.cells.filter((cell) => cell.shape !== "edge");

  const findGroup = (point) => {
    return graphNodes
      .find((p) => p.id === point.cell)
      ?.ports?.items?.find((p) => p.id === point.port)?.group;
  };

  const lines = graphData.cells
    .filter((cell) => cell.shape === "edge")
    .map((cell) => ({
      id: cell.id,
      source: cell.source.cell,
      sourcePoint: findGroup(cell.source),
      target: cell.target.cell,
      targetPoint: findGroup(cell.target),
      meta: cell.data,
    }));

  return JSON.stringify({
    nodes: graphNodes.map((cell) => ({
      id: cell.id,
      x: cell.position.x,
      y: cell.position.y,
      type: cell.data?.type,
      meta: cell.data?.meta,
    })),
    lines,
  });
}

function handleVersion() {
  form.value.content = getGraphData();
  form.value.versionId = undefined;
  updateVersion(form.value).then((res) => {
    loadVersionList(form.value.processId);
    handleGetData(res.data.versionId);
  });
}

function handleSave() {
  form.value.content = getGraphData();
  updateVersion(form.value).then(() => {
    handleClose();
  });
}

function handleDelete() {
  ElMessageBox.confirm("确定要删除该流程版本吗？此操作不可恢复。", "提示", {
    confirmButtonText: "确定删除",
    cancelButtonText: "取消",
    confirmButtonType: "danger",
    type: "warning",
  }).then(() => {
    delVersion(form.value.versionId).then(() => {
      ElMessage.success("删除成功");
      loadVersionList(form.value.processId);
      handleClose();
    });
  }).catch(() => {});
}

function handleReset() {
  if (graph) {
    graph.clearCells();
  }
  flowElement.value = form.value;
  currentType.value = "flow";
}

function handleClose() {
  if (dnd) {
    dnd.dispose();
    dnd = null;
  }
  if (graph) {
    graph.dispose();
    graph = null;
  }
  dialogVisible.value = false;
  emit("close");
}

function handleResize() {
  if (graph && flowContainer.value) {
    graph.resize(flowContainer.value.clientWidth, flowContainer.value.clientHeight);
  }
}

onMounted(() => {
  // 监听窗口resize
  window.addEventListener("resize", handleResize);
});

onBeforeUnmount(() => {
  window.removeEventListener("resize", handleResize);
});

defineExpose({ open });
</script>

<style lang="scss" scoped>
:global(.el-dialog.is-fullscreen) {
  padding: 0;
}

:global(.is-fullscreen .el-dialog__header) {
  padding-bottom: 0;
}

// .flow-container {
//   height: calc(100% - 80px);
// }

.flow-dialog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  border-bottom: 1px solid #dcdfe6;

  .header-actions {
    display: flex;
    gap: 10px;
  }
}

.flow-nodes {
  display: flex;
  flex-direction: column;
  background-color: #fafafa;

  .nodes-header {
    display: flex;
    align-items: center;
    gap: 8px;
    position: relative;
    padding: 12px 16px;
    font-size: 14px;
    font-weight: 600;
    color: #303133;
    border-bottom: 1px solid #dcdfe6;
    background-color: #fff;

    > i {
      font-size: 14px;
      color: #409eff;
    }

    &::after {
      content: "";
      position: absolute;
      bottom: 0;
      left: 16px;
      right: 16px;
      height: 1px;
      background-color: #dcdfe6;
    }
  }

  .nodes-content {
    flex: 1;
    overflow-y: auto;
    padding: 8px 0;

    .node-group {
      margin-bottom: 8px;

      .node-group-title {
        padding: 8px 16px 4px;
        font-size: 12px;
        color: #909399;
        font-weight: 500;
      }

      .node-group-items {
        padding: 0 8px;
      }

      .node-item {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 8px 12px;
        margin: 4px 0;
        background-color: #fff;
        border: 1px solid #e4e7ed;
        border-radius: 4px;
        cursor: move;
        transition: all 0.2s;

        &:hover {
          border-color: #409eff;
          box-shadow: 0 2px 6px rgba(64, 158, 255, 0.2);
        }

        &.is-disabled {
          cursor: not-allowed;
          opacity: 0.5;

          &:hover {
            border-color: #e4e7ed;
            box-shadow: none;
          }
        }

        .node-icon {
          width: 24px;
          height: 24px;
          border-radius: 4px;
          display: flex;
          align-items: center;
          justify-content: center;
          color: #fff;
          font-size: 12px;
        }

        .node-name {
          font-size: 13px;
          color: #606266;
          flex: 1;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
        }
      }
    }
  }
}

main {
  margin-bottom: 0;
  padding: 0;
  border-left: 1px solid #dcdfe6;
  border-right: 1px solid #dcdfe6;

  .route-right-flow {
    position: relative;
    width: 100%;
    height: calc(100vh - 53px);
    background: #f5f7fa;
    overflow: hidden;

    #flow-canvas {
      width: 100%;
      height: 100%;
    }
  }
}

.route-right-attr {
  display: flex;
  flex-direction: column;
  overflow-y: auto;
}

// 右键菜单
.context-menu {
  position: fixed;
  z-index: 9999;
  background: #fff;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  padding: 4px 0;

  .context-menu-item {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 8px 16px;
    cursor: pointer;
    font-size: 14px;
    color: #606266;
    transition: all 0.2s;

    &:hover {
      background-color: #f5f7fa;
      color: #409eff;
    }

    i {
      font-size: 14px;
    }
  }
}
</style>

<style lang="scss">
// X6 SVG 节点样式
.flow-node-body {
  width: 100%;
  height: 100%;
}

.flow-node-text {
  pointer-events: none;
}

// 锚点悬浮显示
.x6-node {
  .x6-port-body {
    opacity: 0;
    transition: opacity 0.2s;
  }

  &:hover {
    .x6-port-body {
      opacity: 1;
    }
  }
}
</style>
