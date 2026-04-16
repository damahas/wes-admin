<template>
  <div class="message-wrapper">
    <el-badge :value="unreadCount" :hidden="unreadCount === 0" class="message-badge">
      <el-popover
        placement="bottom-end"
        :width="450"
        trigger="click"
        v-model:visible="messageVisible"
      >
        <template #reference>
          <div class="message-trigger" @click="handleMessageClick">
            <el-icon class="notification-icon">
              <Bell />
            </el-icon>
          </div>
        </template>

        <div class="message-content">
          <div class="message-header">
            <div class="header-left">
              <el-icon class="notification-icon-small">
                <Bell />
              </el-icon>
              <span>Internal Message</span>
            </div>

            <div class="message-type-select">
              <div
                class="type-btn"
                :class="{ active: activeType === 'all' }"
                @click="handleTypeChange('all')"
              >
                All ({{ messageList.length }})
              </div>
              <div
                class="type-btn"
                :class="{ active: activeType === 'unread' }"
                @click="handleTypeChange('unread')"
              >
                Unread ({{ unreadCount }})
              </div>
              <div
                class="type-btn"
                :class="{ active: activeType === 'read' }"
                @click="handleTypeChange('read')"
              >
                Read ({{ readCount }})
              </div>
            </div>

            <el-input
              v-model="searchKeyword"
              placeholder="Search"
              prefix-icon="Search"
              :style="{ width: '130px' }"
              @input="handleSearch"
            />
          </div>

          <div class="message-actions">
            <div class="action-left">
              <el-checkbox v-model="selectAll" @change="handleSelectAll">
                Select All
              </el-checkbox>
            </div>
            <div class="action-right">
              <span class="action-btn" @click="markAllRead">Mark All as Read</span>
              <span class="action-btn" @click="markRead">Mark as Read</span>
              <span class="action-btn" @click="deleteSelected">Delete</span>
            </div>
          </div>

          <div class="message-list">
            <div
              v-for="msg in filteredMessages"
              :key="msg.id"
              class="message-item"
              :class="{ unread: !msg.isRead }"
            >
              <el-checkbox v-model="msg.selected" class="item-check" />

              <div class="message-item-type-img">
                <img :src="msg.typeIcon" alt="type" width="24" height="24" />
              </div>

              <div class="message-item-content">
                <div class="message-item-top">
                  <div class="title-box-left">
                    <div v-if="!msg.isRead" class="message-item-read-dot"></div>
                    <div class="message-item-cat-name">{{ msg.category }}</div>
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

                <div class="message-item-title">{{ msg.title }}</div>

                <div class="message-item-body">
                  <el-input
                    type="textarea"
                    :model-value="msg.content"
                    readonly
                    :autosize="{ minRows: 1, maxRows: 3 }"
                    class="text-body"
                    style="background-color: transparent"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </el-popover>
    </el-badge>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { Bell } from '@element-plus/icons-vue';
import { ElMessage } from 'element-plus';

const messageVisible = ref(false);
const searchKeyword = ref('');
const selectAll = ref(false);
const activeType = ref('all');

// 假数据
const messageList = ref([
  {
    id: 1,
    typeIcon: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADwAAAA8CAYAAAA6/NlyAAAAAXNSR0IArs4c6QAAAARzQklUCAgICHwIZIgAAAUDSURBVGiB3Zs7U1tHFMd/ZyUCxhMkDZnEBZ5BYyAzdoHsKnSiciaN48JpDZ8gzidg/A3wJzBu4yJDk8QV6pwqFoVdWCS6M1AlYfRgbOxBuifFlWwJPbiPvUL4X0n37nn8dXZXe3bPCjFBK+U0NHIYcogso5IDTQPzp5o6CA6qNaCIoUAjWZRMthqHX2JTmVbKaUxjDZE7KPlIyoQCuE9oThQkk3Vs+OeptQCtlPIkZCMyycEWtjFsyueLhaiaIhGOn2gPHIyuRyEeirBWyvMkmo9HSLQbwhbNxMMwXd0EFdBa6QGm+eLcyAIoa5jmjtZfrwUVDRRhre1tAj8GNRIzHklq4YHfxr4Ie7OvuwOaC+9XnJAirrnrp4ufSVgr5XlMc4fe/89xg4ObWD2L9FDCF4hsGw5u4uawRcvwScu4v3BxyALMe0NvMAZGOJYJ6m0d6v9C/R/v+3QKvroGE5NWzTBkIutLWOuv11Dz2Jr5t3XY/R0OD/q/n52D5W9hesaaSdCfJLW4efppD2Hr4/bgJRSfnd1uYhKWb8OVBStmgWprPDudD3vHcKK5gS2yh/v+yAKcvIfdZ15vsIM0iWZPL+0irEelPMqaFXMn7/2T7ZR5/rMV8wAoea2U8p2PuiOssmHN2MFLOA4RreO6J2sLiW5OHwi3opu3ZuhwP7zsvkXCp6L8McIuvtejvlD7L7zs2yN7fkBXlA20ZmbkjlUjx7Xzke0HJe9tObUjnDjJ27UAzHwZXvZSyp4fbZjGGnzo0ua+dQMTn4WXnZ2z50cb4vVgo5VyOpZk/uqNCLLX7fnRRqtbG5KNeHLcuRtwKcRScXYOZq/a9weARs7gxrhVs/JDsPYTk96aOi4YcgaIbxdjegaWVvy3X1qxnECcgsiyQSSGKbEDSyv+uvbsHGRvxeoKKjmDjiDB/9pHlO1lSUOg6SQ2MqPDfTgesjrymwEdvBr8buaLaP/tHuZFa3saScXLApT/jOqIP8zdgNztSCoCb8R34fXz0ZEFL4uKmElFI9zemxolImZSBnBCS5+8j2T8HOAYkFgOnscSgmMQLYZWYDtvjRuqNYPq7nn7MUIUDS7hI3zRYCgYSIYnbP/EIF6bjWQxKZlsVet7hVA58dXr8PcIZ+qJyfBLUKEgmWw1CYDqNkg+sJLsrfgX/NbgPoHWUYt34N2shNZVHdECJB1hLe0mspLJOh/OlrS+txO0W+sfv+LuPIV3b8I7EgRTlzGr95BvvgsoqNuSWvweOpeWTX0Y1L7729boyAK8e+P9wEFh2Pz4sQXJLBa86rcAmLoc3HhEyNR0UBGns66r67hUK6U8RoaeoHe1d16hYX7xsJiaxqzegyvz/mWMrg4kDOHG8thC2JKZhfXOR73pYTOxDnwKCYVDM9EzL/UQ9k7Mg09gYwdx+5Ym9t0AaNVGPIrbpxjxSGaWtvq9GF6nVfvrxfhW3w2CFCV17eagt8O3eFyzSpQdkdHDwTV3hzUYSlgy2Spu4qKQ9lV6eOYmnmSyjkdaxjhvlmK/EqW+LYOo/RTKhwNt00pq4QHirjMeXbzqVdv5JwvRrgBsWKvpCgqhQDOxHuYKQLRLHkelPDrCSx5CgaY+lMyIL3mchh6V8l7Zk+VKoDYsEP2oyiK8rn6SB3PfykUt1W3c5JbNW2pWCXdCK+U0yUauVVKRQyTVOoueP9XUAakiWkR119s2ju8q3v8E6sB0d6rewwAAAABJRU5ErkJggg==',
    category: '【Approval】',
    title: "yms管理员's FarccaOrder process",
    content: 'You have a new approval task [或签]',
    time: '12-25 18:17',
    status: 'Awaiting Handle',
    statusType: 'warning',
    isRead: false,
    selected: false,
  },
  {
    id: 2,
    typeIcon: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADwAAAA8CAYAAAA6/NlyAAAAAXNSR0IArs4c6QAAAARzQklUCAgICHwIZIgAAAUDSURBVGiB3Zs7U1tHFMd/ZyUCxhMkDZnEBZ5BYyAzdoHsKnSiciaN48JpDZ8gzidg/A3wJzBu4yJDk8QV6pwqFoVdWCS6M1AlYfRgbOxBuifFlWwJPbiPvUL4X0n37nn8dXZXe3bPCjFBK+U0NHIYcogso5IDTQPzp5o6CA6qNaCIoUAjWZRMthqHX2JTmVbKaUxjDZE7KPlIyoQCuE9oThQkk3Vs+OeptQCtlPIkZCMyycEWtjFsyueLhaiaIhGOn2gPHIyuRyEeirBWyvMkmo9HSLQbwhbNxMMwXd0EFdBa6QGm+eLcyAIoa5jmjtZfrwUVDRRhre1tAj8GNRIzHklq4YHfxr4Ie7OvuwOaC+9XnJAirrnrp4ufSVgr5XlMc4fe/89xg4ObWD2L9FDCF4hsGw5u4uawRcvwScu4v3BxyALMe0NvMAZGOJYJ6m0d6v9C/R/v+3QKvroGE5NWzTBkIutLWOuv11Dz2Jr5t3XY/R0OD/q/n52D5W9hesaaSdCfJLW4efppD2Hr4/bgJRSfnd1uYhKWb8OVBStmgWprPDudD3vHcKK5gS2yh/v+yAKcvIfdZ15vsIM0iWZPL+0irEelPMqaFXMn7/2T7ZR5/rMV8wAoea2U8p2PuiOssmHN2MFLOA4RreO6J2sLiW5OHwi3opu3ZuhwP7zsvkXCp6L8McIuvtejvlD7L7zs2yN7fkBXlA20ZmbkjlUjx7Xzke0HJe9tObUjnDjJ27UAzHwZXvZSyp4fbZjGGnzo0ua+dQMTn4WXnZ2z50cb4vVgo5VyOpZk/uqNCLLX7fnRRqtbG5KNeHLcuRtwKcRScXYOZq/a9weARs7gxrhVs/JDsPYTk96aOi4YcgaIbxdjegaWVvy3X1qxnECcgsiyQSSGKbEDSyv+uvbsHGRvxeoKKjmDjiDB/9pHlO1lSUOg6SQ2MqPDfTgesjrymwEdvBr8buaLaP/tHuZFa3saScXLApT/jOqIP8zdgNztSCoCb8R34fXz0ZEFL4uKmElFI9zemxolImZSBnBCS5+8j2T8HOAYkFgOnscSgmMQLYZWYDtvjRuqNYPq7nn7MUIUDS7hI3zRYCgYSIYnbP/EIF6bjWQxKZlsVet7hVA58dXr8PcIZ+qJyfBLUKEgmWw1CYDqNkg+sJLsrfgX/NbgPoHWUYt34N2shNZVHdECJB1hLe0mspLJOh/OlrS+txO0W+sfv+LuPIV3b8I7EgRTlzGr95BvvgsoqNuSWvweOpeWTX0Y1L7729boyAK8e+P9wEFh2Pz4sQXJLBa86rcAmLoc3HhEyNR0UBGns66r67hUK6U8RoaeoHe1d16hYX7xsJiaxqzegyvz/mWMrg4kDOHG8thC2JKZhfXOR73pYTOxDnwKCYVDM9EzL/UQ9k7Mg09gYwdx+5Ym9t0AaNVGPIrbpxjxSGaWtvq9GF6nVfvrxfhW3w2CFCV17eagt8O3eFyzSpQdkdHDwTV3hzUYSlgy2Spu4qKQ9lV6eOYmnmSyjkdaxjhvlmK/EqW+LYOo/RTKhwNt00pq4QHirjMeXbzqVdv5JwvRrgBsWKvpCgqhQDOxHuYKQLRLHkelPDrCSx5CgaY+lMyIL3mchh6V8l7Zk+VKoDYsEP2oyiK8rn6SB3PfykUt1W3c5JbNW2pWCXdCK+U0yUauVVKRQyTVOoueP9XUAakiWkR119s2ju8q3v8E6sB0d6rewwAAAABJRU5ErkJggg==',
    category: '【Approval】',
    title: "yms管理员's FarccaOrder process",
    content: 'You have a new approval task [或签]',
    time: '12-22 14:53',
    status: 'Handled',
    statusType: '',
    isRead: true,
    selected: false,
  },
  {
    id: 3,
    typeIcon: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADwAAAA8CAYAAAA6/NlyAAAAAXNSR0IArs4c6QAAAARzQklUCAgICHwIZIgAAAUDSURBVGiB3Zs7U1tHFMd/ZyUCxhMkDZnEBZ5BYyAzdoHsKnSiciaN48JpDZ8gzidg/A3wJzBu4yJDk8QV6pwqFoVdWCS6M1AlYfRgbOxBuifFlWwJPbiPvUL4X0n37nn8dXZXe3bPCjFBK+U0NHIYcogso5IDTQPzp5o6CA6qNaCIoUAjWZRMthqHX2JTmVbKaUxjDZE7KPlIyoQCuE9oThQkk3Vs+OeptQCtlPIkZCMyycEWtjFsyueLhaiaIhGOn2gPHIyuRyEeirBWyvMkmo9HSLQbwhbNxMMwXd0EFdBa6QGm+eLcyAIoa5jmjtZfrwUVDRRhre1tAj8GNRIzHklq4YHfxr4Ie7OvuwOaC+9XnJAirrnrp4ufSVgr5XlMc4fe/89xg4ObWD2L9FDCF4hsGw5u4uawRcvwScu4v3BxyALMe0NvMAZGOJYJ6m0d6v9C/R/v+3QKvroGE5NWzTBkIutLWOuv11Dz2Jr5t3XY/R0OD/q/n52D5W9hesaaSdCfJLW4efppD2Hr4/bgJRSfnd1uYhKWb8OVBStmgWprPDudD3vHcKK5gS2yh/v+yAKcvIfdZ15vsIM0iWZPL+0irEelPMqaFXMn7/2T7ZR5/rMV8wAoea2U8p2PuiOssmHN2MFLOA4RreO6J2sLiW5OHwi3opu3ZuhwP7zsvkXCp6L8McIuvtejvlD7L7zs2yN7fkBXlA20ZmbkjlUjx7Xzke0HJe9tObUjnDjJ27UAzHwZXvZSyp4fbZjGGnzo0ua+dQMTn4WXnZ2z50cb4vVgo5VyOpZk/uqNCLLX7fnRRqtbG5KNeHLcuRtwKcRScXYOZq/a9weARs7gxrhVs/JDsPYTk96aOi4YcgaIbxdjegaWVvy3X1qxnECcgsiyQSSGKbEDSyv+uvbsHGRvxeoKKjmDjiDB/9pHlO1lSUOg6SQ2MqPDfTgesjrymwEdvBr8buaLaP/tHuZFa3saScXLApT/jOqIP8zdgNztSCoCb8R34fXz0ZEFL4uKmElFI9zemxolImZSBnBCS5+8j2T8HOAYkFgOnscSgmMQLYZWYDtvjRuqNYPq7nn7MUIUDS7hI3zRYCgYSIYnbP/EIF6bjWQxKZlsVet7hVA58dXr8PcIZ+qJyfBLUKEgmWw1CYDqNkg+sJLsrfgX/NbgPoHWUYt34N2shNZVHdECJB1hLe0mspLJOh/OlrS+txO0W+sfv+LuPIV3b8I7EgRTlzGr95BvvgsoqNuSWvweOpeWTX0Y1L7729boyAK8e+P9wEFh2Pz4sQXJLBa86rcAmLoc3HhEyNR0UBGns66r67hUK6U8RoaeoHe1d16hYX7xsJiaxqzegyvz/mWMrg4kDOHG8thC2JKZhfXOR73pYTOxDnwKCYVDM9EzL/UQ9k7Mg09gYwdx+5Ym9t0AaNVGPIrbpxjxSGaWtvq9GF6nVfvrxfhW3w2CFCV17eagt8O3eFyzSpQdkdHDwTV3hzUYSlgy2Spu4qKQ9lV6eOYmnmSyjkdaxjhvlmK/EqW+LYOo/RTKhwNt00pq4QHirjMeXbzqVdv5JwvRrgBsWKvpCgqhQDOxHuYKQLRLHkelPDrCSx5CgaY+lMyIL3mchh6V8l7Zk+VKoDYsEP2oyiK8rn6SB3PfykUt1W3c5JbNW2pWCXdCK+U0yUauVVKRQyTVOoueP9XUAakiWkR119s2ju8q3v8E6sB0d6rewwAAAABJRU5ErkJggg==',
    category: '【Approval】',
    title: "yms管理员's FarccaOrder process",
    content: 'You have a new approval task [或签]',
    time: '12-22 14:51',
    status: 'Handled',
    statusType: '',
    isRead: false,
    selected: false,
  },
  {
    id: 4,
    typeIcon: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAAXNSR0IArs4c6QAAAARzQklUCAgICHwIZIgAAAKgSURBVEiJxZcxT1NRFMd/57YYYsCQ9okOJFCRxMREwmocSRcGBxZmMQExMWGQwQ/gwCYDaAK7i3Fy4TM4yKTEmAemA5r22VQkpbze4/Ba2lLad8Em/pebl3vu/3fue+fdd57goK8FvWYsMwhZA5MqjKIMASAURdm3sIOybQ0fJtJSivOUbpP+T71JghVreSzCoEuSqvw2hk2qrGaG5eDCYD/QBZSXqqRcgG3GQoDwIpOSN05gVU3uFVhTeHIZ4DmAjbE0z0Qk7AhW1aQf8BZlthfQJsq7TIq5ZrhpnvcLvOo5FECZ3Suw1ppLTd8K+kiULVev0EZj0nSPa4EZFuvPXAB2S+olj/ksgudiUD6Bj98j+O3rMDLkCBYCqtzNDMuBAeirsOwKDS3sBY0d535FibhIlRQJVgDE97VfB8kB6biF5RP4lINy2D43loKxWIfoPbeGEWMHmP5XKER3IVeMB4swaCwzRoRsfHhk3Al6GlNoPILudLIGYcoFfBB7+kbQ4lF8nIFJgzIeF3h47JJaAx4nFUYNxFdzWHUHO0kZMkCix7ZOMggOtegux+IqGlH24+Liqrkl1uEwEWXfWNjpFhRayP9xB+cP4+EWdgzKdregLz8iM1eVw2hNVynbUuuncp1am9BevKqTic5frfqRmZxIS8kv6KYqy+eamFaTowo8fx+Ndc3fhwexp0EkY9gcT0spsqyyKkLgsvDqFXh4r3F954Y7tPZZXIWmRsAPdEEtr90soh0fVcAbcF1xTiNwCs/req+avDYobGQ8WWq6bui/NXsiEmZSzAHrvWOycRbaBq7Db3nyVIV5VfKXBgqBGBYzniydhdYS6qzdknp9FZaBBRy6FOjBL0yzfF/77QDTImQRprCMEzWHicv+tP0FAqsZBy4O6OgAAAAASUVORK5CYII=',
    category: '【Notification】',
    title: 'Defect alarm',
    content: '缺陷率告警了,缺陷[Cut Offset],目标值[0.0],实际值[5.0E-4]',
    time: '12-06 12:24',
    status: '',
    statusType: '',
    isRead: false,
    selected: false,
  },
  {
    id: 5,
    typeIcon: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAAXNSR0IArs4c6QAAAARzQklUCAgICHwIZIgAAAKgSURBVEiJxZcxT1NRFMd/57YYYsCQ9okOJFCRxMREwmocSRcGBxZmMQExMWGQwQ/gwCYDaAK7i3Fy4TM4yKTEmAemA5r22VQkpbze4/Ba2lLad8Em/pebl3vu/3fue+fdd57goK8FvWYsMwhZA5MqjKIMASAURdm3sIOybQ0fJtJSivOUbpP+T71JghVreSzCoEuSqvw2hk2qrGaG5eDCYD/QBZSXqqRcgG3GQoDwIpOSN05gVU3uFVhTeHIZ4DmAjbE0z0Qk7AhW1aQf8BZlthfQJsq7TIq5ZrhpnvcLvOo5FECZ3Suw1ppLTd8K+kiULVev0EZj0nSPa4EZFuvPXAB2S+olj/ksgudiUD6Bj98j+O3rMDLkCBYCqtzNDMuBAeirsOwKDS3sBY0d535FibhIlRQJVgDE97VfB8kB6biF5RP4lINy2D43loKxWIfoPbeGEWMHmP5XKER3IVeMB4swaCwzRoRsfHhk3Al6GlNoPILudLIGYcoFfBB7+kbQ4lF8nIFJgzIeF3h47JJaAx4nFUYNxFdzWHUHO0kZMkCix7ZOMggOtegux+IqGlH24+Liqrkl1uEwEWXfWNjpFhRayP9xB+cP4+EWdgzKdregLz8iM1eVw2hNVynbUuuncp1am9BevKqTic5frfqRmZxIS8kv6KYqy+eamFaTowo8fx+Ndc3fhwexp0EkY9gcT0spsqyyKkLgsvDqFXh4r3F954Y7tPZZXIWmRsAPdEEtr90soh0fVcAbcF1xTiNwCs/req+avDYobGQ8WWq6bui/NXsiEmZSzAHrvWOycRbaBq7Db3nyVIV5VfKXBgqBGBYzniydhdYS6qzdknp9FZaBBRy6FOjBL0yzfF/77QDTImQRprCMEzWHicv+tP0FAqsZBy4O6OgAAAAASUVORK5CYII=',
    category: '【Notification】',
    title: 'Defect alarm',
    content: '缺陷率告警了,缺陷[Cut Offset],目标值[0.0],实际值[5.0E-4]',
    time: '12-06 12:18',
    status: '',
    statusType: '',
    isRead: false,
    selected: false,
  },
  {
    id: 6,
    typeIcon: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADwAAAA8CAYAAAA6/NlyAAAAAXNSR0IArs4c6QAAAARzQklUCAgICHwIZIgAAAUDSURBVGiB3Zs7U1tHFMd/ZyUCxhMkDZnEBZ5BYyAzdoHsKnSiciaN48JpDZ8gzidg/A3wJzBu4yJDk8QV6pwqFoVdWCS6M1AlYfRgbOxBuifFlWwJPbiPvUL4X0n37nn8dXZXe3bPCjFBK+U0NHIYcogso5IDTQPzp5o6CA6qNaCIoUAjWZRMthqHX2JTmVbKaUxjDZE7KPlIyoQCuE9oThQkk3Vs+OeptQCtlPIkZCMyycEWtjFsyueLhaiaIhGOn2gPHIyuRyEeirBWyvMkmo9HSLQbwhbNxMMwXd0EFdBa6QGm+eLcyAIoa5jmjtZfrwUVDRRhre1tAj8GNRIzHklq4YHfxr4Ie7OvuwOaC+9XnJAirrnrp4ufSVgr5XlMc4fe/89xg4ObWD2L9FDCF4hsGw5u4uawRcvwScu4v3BxyALMe0NvMAZGOJYJ6m0d6v9C/R/v+3QKvroGE5NWzTBkIutLWOuv11Dz2Jr5t3XY/R0OD/q/n52D5W9hesaaSdCfJLW4efppD2Hr4/bgJRSfnd1uYhKWb8OVBStmgWprPDudD3vHcKK5gS2yh/v+yAKcvIfdZ15vsIM0iWZPL+0irEelPMqaFXMn7/2T7ZR5/rMV8wAoea2U8p2PuiOssmHN2MFLOA4RreO6J2sLiW5OHwi3opu3ZuhwP7zsvkXCp6L8McIuvtejvlD7L7zs2yN7fkBXlA20ZmbkjlUjx7Xzke0HJe9tObUjnDjJ27UAzHwZXvZSyp4fbZjGGnzo0ua+dQMTn4WXnZ2z50cb4vVgo5VyOpZk/uqNCLLX7fnRRqtbG5KNeHLcuRtwKcRScXYOZq/a9weARs7gxrhVs/JDsPYTk96aOi4YcgaIbxdjegaWVvy3X1qxnECcgsiyQSSGKbEDSyv+uvbsHGRvxeoKKjmDjiDB/9pHlO1lSUOg6SQ2MqPDfTgesjrymwEdvBr8buaLaP/tHuZFa3saScXLApT/jOqIP8zdgNztSCoCb8R34fXz0ZEFL4uKmElFI9zemxolImZSBnBCS5+8j2T8HOAYkFgOnscSgmMQLYZWYDtvjRuqNYPq7nn7MUIUDS7hI3zRYCgYSIYnbP/EIF6bjWQxKZlsVet7hVA58dXr8PcIZ+qJyfBLUKEgmWw1CYDqNkg+sJLsrfgX/NbgPoHWUYt34N2shNZVHdECJB1hLe0mspLJOh/OlrS+txO0W+sfv+LuPIV3b8I7EgRTlzGr95BvvgsoqNuSWvweOpeWTX0Y1L7729boyAK8e+P9wEFh2Pz4sQXJLBa86rcAmLoc3HhEyNR0UBGns66r67hUK6U8RoaeoHe1d16hYX7xsJiaxqzegyvz/mWMrg4kDOHG8thC2JKZhfXOR73pYTOxDnwKCYVDM9EzL/UQ9k7Mg09gYwdx+5Ym9t0AaNVGPIrbpxjxSGaWtvq9GF6nVfvrxfhW3w2CFCV17eagt8O3eFyzSpQdkdHDwTV3hzUYSlgy2Spu4qKQ9lV6eOYmnmSyjkdaxjhvlmK/EqW+LYOo/RTKhwNt00pq4QHirjMeXbzqVdv5JwvRrgBsWKvpCgqhQDOxHuYKQLRLHkelPDrCSx5CgaY+lMyIL3mchh6V8l7Zk+VKoDYsEP2oyiK8rn6SB3PfykUt1W3c5JbNW2pWCXdCK+U0yUauVVKRQyTVOoueP9XUAakiWkR119s2ju8q3v8E6sB0d6rewwAAAABJRU5ErkJggg==',
    category: '【Approval】',
    title: '超级管理员的requirement list流程',
    content: 'item: NO_202510230004',
    time: '10-23 14:31',
    status: 'Awaiting Handle',
    statusType: 'warning',
    isRead: false,
    selected: false,
  },
]);

const unreadCount = computed(() => messageList.value.filter(msg => !msg.isRead).length);
const readCount = computed(() => messageList.value.filter(msg => msg.isRead).length);

const filteredMessages = computed(() => {
  let messages = messageList.value;

  // 按类型筛选
  if (activeType.value === 'unread') {
    messages = messages.filter(msg => !msg.isRead);
  } else if (activeType.value === 'read') {
    messages = messages.filter(msg => msg.isRead);
  }

  // 按关键词搜索
  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase();
    messages = messages.filter(msg =>
      msg.title.toLowerCase().includes(keyword) ||
      msg.content.toLowerCase().includes(keyword)
    );
  }

  return messages;
});

const handleMessageClick = () => {
  messageVisible.value = true;
};

const handleTypeChange = (type) => {
  activeType.value = type;
  selectAll.value = false;
  messageList.value.forEach(msg => msg.selected = false);
};

const handleSearch = () => {
  // 搜索逻辑已在 computed 中处理
};

const handleSelectAll = (val) => {
  filteredMessages.value.forEach(msg => {
    msg.selected = val;
  });
};

const markAllRead = () => {
  messageList.value.forEach(msg => {
    msg.isRead = true;
  });
  ElMessage.success('All marked as read');
};

const markRead = () => {
  const selected = filteredMessages.value.filter(msg => msg.selected);
  selected.forEach(msg => {
    msg.isRead = true;
    msg.selected = false;
  });
  ElMessage.success('Marked as read');
};

const deleteSelected = () => {
  const selectedIds = filteredMessages.value.filter(msg => msg.selected).map(msg => msg.id);
  messageList.value = messageList.value.filter(msg => !selectedIds.includes(msg.id));
  selectAll.value = false;
  ElMessage.success('Deleted');
};
</script>

<style scoped>
.message-wrapper {
  display: flex;
  align-items: center;
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

.notification-icon {
  font-size: 20px;
  color: var(--text-primary);
}

.message-content {
  padding: 0;
}

.message-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px;
  border-bottom: 1px solid var(--el-border-color-light);
}

.header-left {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
}

.notification-icon-small {
  font-size: 16px;
  color: var(--text-primary);
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
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 12px;
  background-color: var(--el-fill-color-light);
  border-bottom: 1px solid var(--el-border-color-light);
}

.action-left {
  display: flex;
  align-items: center;
}

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
}

.message-item:hover {
  background-color: var(--el-fill-color-light);
}

.message-item.unread {
  background-color: var(--el-fill-color-extra-light);
}

.item-check {
  margin-top: 4px;
}

.message-item-type-img {
  flex-shrink: 0;
}

.message-item-type-img img {
  border-radius: 4px;
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
  background-color: var(--theme-color);
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
  font-weight: 500;
  color: var(--text-primary);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.message-item-body {
  margin-top: 4px;
}

.text-body :deep(.el-textarea__inner) {
  background-color: transparent;
  border: none;
  resize: none;
  font-size: 13px;
  color: var(--text-regular);
  padding: 0;
}
</style>
