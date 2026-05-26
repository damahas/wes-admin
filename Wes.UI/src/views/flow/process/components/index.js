const components = import.meta.glob("./*/index.vue", { eager: true })

const result = Object.keys(components).reduce((sum, path) => {
  const matchType = path.replace(/(\/index.vue|\\|\/|\.)/g, "")
  return {
    ...sum,
    [matchType]: components[path].default,
  }
}, {})

export default result
