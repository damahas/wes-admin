import moment from 'moment';
import request from '@/utils/request';
import { saveAs } from 'file-saver';
import { getDicts } from '@/api/system/dict'
import { ElNotification, ElMessageBox, ElMessage, ElLoading } from 'element-plus';
import { ref, toRefs } from 'vue'
import store from '@/store'

let downloadLoadingInstance;

/**
 * 日期格式化
 * @param {*} time
 * @param {string} pattern - 格式化模板，默认 'YYYY-MM-DD HH:mm:ss'
 * @returns {string} 格式化后的日期字符串
 */
export function formatTime(time, pattern = 'YYYY-MM-DD HH:mm:ss') {
    return moment(time).format(pattern);
}

/**
 * 参数处理 - 将对象转换为 URL 查询字符串
 * @param {Object} params - 参数对象
 * @returns {string} URL 编码后的查询字符串
 */
export function tansParams(params) {
    const result = [];
    for (const propName of Object.keys(params)) {
        const value = params[propName];
        if (value !== null && value !== undefined) {
            if (typeof value === "object") {
                for (const key of Object.keys(value)) {
                    if (value[key] !== null && value[key] !== undefined) {
                        const nestedKey = `${propName}.${key}`;
                        result.push(`${encodeURIComponent(nestedKey)}=${encodeURIComponent(value[key])}`);
                    }
                }
            } else {
                result.push(`${encodeURIComponent(propName)}=${encodeURIComponent(value)}`);
            }
        }
    }
    return result.join('&');
}

/**
 * 添加日期范围
 * @param {*} params 
 * @param {*} dateRange 
 * @param {*} propName 
 * @returns 
 */
export function addDateRange(params, dateRange, propName) {
    let search = params;
    search.params = typeof (search.params) === 'object' && search.params !== null && !Array.isArray(search.params) ? search.params : {};
    dateRange = Array.isArray(dateRange) ? dateRange : [];
    if (typeof (propName) === 'undefined') {
        search.params['beginTime'] = dateRange[0];
        search.params['endTime'] = dateRange[1];
    } else {
        search.params['begin' + propName] = dateRange[0];
        search.params['end' + propName] = dateRange[1];
    }
    return search;
}

/**
 * 通用下载方法
 * @param {string} url - 下载接口地址
 * @param {Object} params - 请求参数
 * @param {string} filename - 保存的文件名
 * @param {Object} config - axios 配置
 * @returns {Promise} 下载 Promise
 */
export function download(url, params, filename, config) {
    downloadLoadingInstance = ElLoading.service({
        text: "正在下载数据，请稍候",
        background: "rgba(0, 0, 0, 0.7)",
    });
    return request.post(url, params, {
        transformRequest: [(params) => tansParams(params)],
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        responseType: 'blob',
        ...config
    }).then(async (data) => {
        const isBlob = blobValidate(data);
        console.log(isBlob);
        if (isBlob) {
            saveAs(new Blob([data]), filename);
        } else {
            const resText = await data.text();
            const rspObj = JSON.parse(resText);
            const errMsg = errorCode?.[rspObj.code] || rspObj.msg || errorCode?.['default'] || '下载失败';
            ElMessage.error(errMsg);
        }
        downloadLoadingInstance.close();
    }).catch(() => {
        ElMessage.error('下载文件出现错误，请联系管理员！');
        downloadLoadingInstance.close();
    });
}

/**
 * 验证是否为 blob 格式
 * @param {*} data
 * @returns {boolean}
 */
export function blobValidate(data) {
    return data.type !== 'application/json';
}

/**
 * 获取字典数据
 * @param {...string} dictTypes - 字典类型
 * @returns {Object} 返回包含所有请求字典的响应式对象
 * @example
 * const { sys_normal_disable, sys_user_sex } = getDict('sys_normal_disable', 'sys_user_sex')
 */
export function getDict(...dictTypes) {
    const res = ref({});
    dictTypes.forEach(dictType => {
        // 初始化空数组
        res.value[dictType] = [];
        // 从 store 获取缓存的字典
        const cachedDict = store.getters['dict/getDict'](dictType);
        if (cachedDict?.length) {
            // 使用缓存数据
            res.value[dictType] = cachedDict;
            return;
        }
        // 从接口获取数据
        getDicts(dictType).then(resp => {
            const dictData = resp.data.map(item => ({
                id: item.dictDataId,
                label: item.dictLabel,
                value: item.dictValue,
                parentId: item.parentId,
                elTagType: item.listClass,
                elTagClass: item.cssClass
            }));
            // 更新响应式数据
            res.value[dictType] = dictData;
            // 缓存到 store
            store.dispatch('dict/setDict', {
                dictType,
                dictData
            });
        }).catch(err => {
            console.error(`获取字典 ${dictType} 失败:`, err);
        });
    });

    return toRefs(res.value);
}

/**
 * 构造树型结构数据
 * @param {*} data 数据源
 * @param {*} id id字段 默认 'id'
 * @param {*} parentId 父节点字段 默认 'parentId'
 * @param {*} children 孩子节点字段 默认 'children'
 */
export function handleTree(data, id, parentId, children) {
    let config = {
        id: id || 'id',
        parentId: parentId || 'parentId',
        childrenList: children || 'children'
    };

    var childrenListMap = {};
    var nodeIds = {};
    var tree = [];

    for (let d of data) {
        let parentId = d[config.parentId];
        if (childrenListMap[parentId] == null) {
            childrenListMap[parentId] = [];
        }
        nodeIds[d[config.id]] = d;
        childrenListMap[parentId].push(d);
    }

    for (let d of data) {
        let parentId = d[config.parentId];
        if (nodeIds[parentId] == null) {
            tree.push(d);
        }
    }

    for (let t of tree) {
        adaptToChildrenList(t);
    }

    function adaptToChildrenList(o) {
        if (childrenListMap[o[config.id]] !== null) {
            o[config.childrenList] = childrenListMap[o[config.id]];
        }
        if (o[config.childrenList]) {
            for (let c of o[config.childrenList]) {
                adaptToChildrenList(c);
            }
        }
    }
    return tree;
}