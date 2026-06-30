const traitModules = import.meta.glob("./*.config.js", { eager: true })

export const traits = Object.keys(traitModules).reduce((sum, path) => {
  const matchType = path.replace(/(\.config\.js|\\|\/|\.)/g, "")
  return {
    ...sum,
    [matchType]: traitModules[path].default,
  }
}, {})

export const getElementTrait = (elementType) => {
  return traits[elementType] || { label: "节点", components: [] }
}

// 节点分组配置
const nodeGroups = [
  { groupName: "基础", types: ["start", "end"] },
  { groupName: "处理", types: ["task", "notice"] },
]

// 左侧节点列表
export const flowNodes = nodeGroups.map(({ groupName, types }) => ({
  groupName,
  nodes: types.map(type => {
    const trait = traits[type]
    return {
      name: trait.label,
      icon: trait.icon,
      color: trait.color,
      type: trait.type,
    }
  }),
}))

// 连接桩
export const ports = {
  groups: {
    top: {
      position: "top",
      attrs: {
        circle: {
          r: 4,
          magnet: true,
          stroke: "#c6c9ce",
          strokeWidth: 1,
          fill: "#fff",
        },
        text: { text: "" },
      },
      markup: [
        {
          tagName: "circle",
          attrs: {
            r: 4,
            magnet: true,
            stroke: "#c6c9ce",
            strokeWidth: 1,
            fill: "#fff",
            class: "port-body",
          },
        },
      ],
    },
    bottom: {
      position: "bottom",
      attrs: {
        circle: {
          r: 4,
          magnet: true,
          stroke: "#c6c9ce",
          strokeWidth: 1,
          fill: "#fff",
        },
        text: { text: "" },
      },
      markup: [
        {
          tagName: "circle",
          attrs: {
            r: 4,
            magnet: true,
            stroke: "#c6c9ce",
            strokeWidth: 1,
            fill: "#fff",
            class: "port-body",
          },
        },
      ],
    },
    left: {
      position: "left",
      attrs: {
        circle: {
          r: 4,
          magnet: true,
          stroke: "#c6c9ce",
          strokeWidth: 1,
          fill: "#fff",
        },
        text: { text: "" },
      },
      markup: [
        {
          tagName: "circle",
          attrs: {
            r: 4,
            magnet: true,
            stroke: "#c6c9ce",
            strokeWidth: 1,
            fill: "#fff",
            class: "port-body",
          },
        },
      ],
    },
    right: {
      position: "right",
      attrs: {
        circle: {
          r: 4,
          magnet: true,
          stroke: "#c6c9ce",
          strokeWidth: 1,
          fill: "#fff",
        },
        text: { text: "" },
      },
      markup: [
        {
          tagName: "circle",
          attrs: {
            r: 4,
            magnet: true,
            stroke: "#c6c9ce",
            strokeWidth: 1,
            fill: "#fff",
            class: "port-body",
          },
        },
      ],
    },
  },
  items: [{ group: "top" }, { group: "bottom" }, { group: "left" }, { group: "right" }],
}