import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "path";

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src"),
    },
    extensions: [".mjs", ".js", ".ts", ".jsx", ".tsx", ".json", ".vue"],
  },
  css: {
    preprocessorOptions: {
      scss: {
        api: "modern-compiler",
      },
    },
  },
  build: {
    // element-plus 和 @antv/x6 库体量大，调高告警阈值
    chunkSizeWarningLimit: 1000,
    rollupOptions: {
      output: {
        manualChunks(id) {
          if (id.includes("node_modules")) {
            // element-plus 全量引入，体积最大，独立拆分
            if (id.includes("element-plus")) return "element-plus";
            // element-plus 图标库全量引入，独立拆分
            if (id.includes("@element-plus/icons-vue")) return "element-icons";
            // Vue 生态核心
            if (
              id.includes("/vue/") || id.includes("/vue@") ||
              id.includes("/vue-router/") ||
              id.includes("/vuex/") ||
              id.includes("/vue-i18n/")
            ) return "vue-vendor";
            // 流程图引擎（仅在流程模块使用）
            if (id.includes("@antv/x6")) return "x6-vendor";
            // 代码高亮（仅在代码编辑器使用）
            if (id.includes("highlight.js")) return "highlight";
            // 其他第三方依赖归入 vendor
            return "vendor";
          }
        },
      },
    },
  },
  server: {
    port: 3000,
    open: true,
    proxy: {
      "/api": {
        target: "http://localhost:5113",
        changeOrigin: true,
        // rewrite: (path) => path.replace(/^\/api/, '/api'),
      },
    },
  },
});
