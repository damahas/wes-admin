const traitModules = import.meta.glob("./*.config.js", { eager: true })

export const useFlowConfig = (t) => {
  const traits = Object.keys(traitModules).reduce((sum, path) => {
    const matchType = path.replace(/(\.config\.js|\\|\/|\.)/g, "")
    return {
      ...sum,
      [matchType]: traitModules[path].default(t),
    }
  }, {})

  const getElementTrait = (elementType) => {
    return traits[elementType] || { label: t('flow.designer.node.general'), components: [] }
  }

  const nodeGroups = [
    { groupName: t('flow.designer.group.basic'), types: ["start", "end"] },
    { groupName: t('flow.designer.group.process'), types: ["task", "notice"] },
    { groupName: t('flow.designer.group.branch'), types: ["branch"] },
  ]

  const flowNodes = nodeGroups.map(({ groupName, types }) => ({
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

  return { traits, flowNodes, getElementTrait }
}

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
