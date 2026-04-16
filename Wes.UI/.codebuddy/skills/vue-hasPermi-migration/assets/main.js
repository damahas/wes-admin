import { createApp } from 'vue'
import App from './App'
import directive from './directive' // 引入指令

const app = createApp(App)

// 注册自定义指令
directive(app)

app.mount('#app')
