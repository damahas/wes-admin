/*
 Navicat Premium Data Transfer

 Source Server         : 8.153.171.43
 Source Server Type    : MySQL
 Source Server Version : 50736
 Source Host           : 8.153.171.43:3306
 Source Schema         : temp

 Target Server Type    : MySQL
 Target Server Version : 50736
 File Encoding         : 65001

 Date: 01/08/2026 16:00:55
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for sys_menu
-- ----------------------------
DROP TABLE IF EXISTS `sys_menu`;
CREATE TABLE `sys_menu`  (
  `menu_id` bigint(20) NOT NULL COMMENT '菜单ID',
  `menu_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '菜单名称',
  `parent_id` bigint(20) NOT NULL DEFAULT 0 COMMENT '父菜单ID',
  `order_num` int(11) NULL DEFAULT 0 COMMENT '显示顺序',
  `path` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '路由地址',
  `route_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '路由名称',
  `component` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '组件路径',
  `query` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '路由参数',
  `is_frame` int(11) NULL DEFAULT 1 COMMENT '是否为外链（0是 1否）',
  `is_cache` int(11) NULL DEFAULT 0 COMMENT '是否缓存（0缓存 1不缓存）',
  `menu_type` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '菜单类型（M目录 C菜单 F按钮）',
  `visible` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '菜单状态（0显示 1隐藏）',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '菜单状态（0正常 1停用）',
  `perms` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '权限标识',
  `icon` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '#' COMMENT '菜单图标',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '备注',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `system_id` bigint(20) NULL DEFAULT NULL COMMENT '系统id',
  PRIMARY KEY (`menu_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '菜单权限' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_menu
-- ----------------------------
INSERT INTO `sys_menu` VALUES (1, '系统管理', 0, 500, 'system', NULL, NULL, '', 1, 0, 'M', '0', '0', '', 'gear', 'admin', '2022-04-15 15:51:01', 'admin', '2024-11-01 22:55:50', '系统管理目录', 0, 100028);
INSERT INTO `sys_menu` VALUES (2, '流程管理', 1, 499, 'flow', NULL, NULL, NULL, 1, 0, 'M', '0', '0', NULL, 'link', 'admin', '2026-07-01 22:48:12', 'admin', '2026-07-15 08:56:48', '流程管理目录', 0, 100028);
INSERT INTO `sys_menu` VALUES (3, '系统工具', 0, 501, 'tool', NULL, NULL, '', 1, 0, 'M', '0', '0', '', 'toolbox', 'admin', '2022-04-15 15:51:01', 'admin', '2022-08-14 21:43:45', '系统工具目录', 0, 100028);
INSERT INTO `sys_menu` VALUES (100, '用户管理', 1, 1, 'user', NULL, 'system/user/index', '', 1, 0, 'C', '0', '0', 'system:user:list', 'user', 'admin', '2022-04-15 15:51:01', '', NULL, '用户管理菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (101, '角色管理', 1, 2, 'role', NULL, 'system/role/index', '', 1, 0, 'C', '0', '0', 'system:role:list', 'user-group', 'admin', '2022-04-15 15:51:01', 'admin', '2024-10-29 22:08:23', '角色管理菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (102, '菜单管理', 1, 3, 'menu', NULL, 'system/menu/index', '', 1, 0, 'C', '0', '0', 'system:menu:list', 'folder-tree', 'admin', '2022-04-15 15:51:01', 'admin', '2024-10-29 22:14:19', '菜单管理菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (103, '部门管理', 1, 4, 'dept', NULL, 'system/dept/index', '', 1, 0, 'C', '0', '0', 'system:dept:list', 'network-wired', 'admin', '2022-04-15 15:51:01', 'admin', '2024-10-29 22:15:33', '部门管理菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (104, '岗位管理', 1, 5, 'post', NULL, 'system/post/index', '', 1, 0, 'C', '0', '0', 'system:post:list', 'user-tie', 'admin', '2022-04-15 15:51:01', 'admin', '2024-10-29 22:19:36', '岗位管理菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (105, '字典管理', 1, 6, 'dict', NULL, 'system/dict/index', '', 1, 0, 'C', '0', '0', 'system:dict:list', 'file-invoice', 'admin', '2022-04-15 15:51:01', '', NULL, '字典管理菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (106, '参数设置', 1, 7, 'config', NULL, 'system/config/index', '', 1, 0, 'C', '0', '0', 'system:config:list', 'file-signature', 'admin', '2022-04-15 15:51:01', '', NULL, '参数设置菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (109, '在线用户', 3, 1, 'monitor', NULL, 'system/online/index', '', 1, 0, 'C', '0', '0', 'monitor:online:list', 'signal', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-01 17:42:46', '在线用户菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (110, '定时任务', 3, 2, 'job', NULL, 'system/job/index', '', 1, 0, 'C', '0', '0', 'monitor:job:list', 'bars-progress', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-01 17:44:42', '定时任务菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (114, '数据服务', 1, 98, 'data/service', 'DataService', 'system/dataService/index', '', 1, 0, 'C', '0', '0', 'system:dataService:list', 'crop-simple', 'admin', '2022-04-15 15:51:01', 'admin', '2025-01-01 14:20:45', '表单构建菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (118, '系统日志', 3, 8, 'log', NULL, 'system/logRec/index', '', 1, 0, 'C', '0', '0', 'system:log:list', 'book-atlas', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-09 14:51:22', '登录日志菜单', 0, 100028);
INSERT INTO `sys_menu` VALUES (119, '编码服务', 1, 9, 'coderule', NULL, 'system/codeRule/index', NULL, 1, 0, 'C', '0', '0', 'system:coderule:list', 'cubes', 'admin', '2022-07-24 23:41:24', 'admin', '2026-04-03 14:56:46', NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (120, '序号生成新增', 119, 1, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:coderule:add', NULL, 'admin', '2022-07-25 01:10:30', NULL, NULL, NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (121, '序号生成修改', 119, 2, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:coderule:edit', NULL, 'admin', '2022-07-25 01:10:53', NULL, NULL, NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (122, '序号生成删除', 119, 3, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:coderule:remove', NULL, 'admin', '2022-07-25 01:11:12', NULL, NULL, NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (123, '用户查询', 100, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (124, '用户新增', 100, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (125, '用户修改', 100, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (126, '用户删除', 100, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (127, '用户导出', 100, 5, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (128, '用户导入', 100, 6, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:import', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (129, '重置密码', 100, 7, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:resetPwd', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (130, '角色查询', 101, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (131, '角色新增', 101, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (132, '角色修改', 101, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (133, '角色删除', 101, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (134, '角色导出', 101, 5, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (135, '菜单查询', 102, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:menu:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (136, '菜单新增', 102, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:menu:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (137, '菜单修改', 102, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:menu:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (138, '菜单删除', 102, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:menu:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (139, '部门查询', 103, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dept:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (140, '部门新增', 103, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dept:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (141, '部门修改', 103, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dept:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (142, '部门删除', 103, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dept:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (143, '岗位查询', 104, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (144, '岗位新增', 104, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (145, '岗位修改', 104, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (146, '岗位删除', 104, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (147, '岗位导出', 104, 5, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (148, '字典查询', 105, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (149, '字典新增', 105, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (150, '字典修改', 105, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (151, '字典删除', 105, 4, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (152, '字典导出', 105, 5, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (153, '参数查询', 106, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (154, '参数新增', 106, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (155, '参数修改', 106, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (156, '参数删除', 106, 4, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (157, '参数导出', 106, 5, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (161, '登录查询', 118, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:logininfor:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (162, '登录删除', 118, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:logininfor:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (163, '日志导出', 118, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:logininfor:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (164, '在线查询', 109, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:online:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (165, '批量强退', 109, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:online:batchLogout', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (166, '单条强退', 109, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:online:forceLogout', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (167, '任务查询', 110, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (168, '任务新增', 110, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (169, '任务修改', 110, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (170, '任务删除', 110, 4, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (171, '状态修改', 110, 5, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:changeStatus', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (172, '任务导出', 110, 7, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO `sys_menu` VALUES (179, '许可证', 3, 100, 'license', NULL, 'system/license/index', NULL, 1, 0, 'C', '0', '0', 'system:license:list', 'cubes', 'admin', '2022-12-07 21:43:43', 'admin', '2026-07-01 22:56:10', NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (180, '流程管理', 1, 99, '/flow', NULL, NULL, NULL, 1, 0, 'M', '0', '0', NULL, 'shuffle', 'admin', '2022-12-16 23:06:28', 'admin', '2026-05-05 21:51:27', NULL, 1, 100028);
INSERT INTO `sys_menu` VALUES (181, '流程定义', 2, 1, 'process', NULL, 'flow/process/index', NULL, 1, 0, 'C', '0', '0', 'flow:process:list', 'pen-ruler', 'admin', '2022-12-18 00:23:46', 'admin', '2026-07-01 22:50:43', NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (182, '流程实例', 2, 2, 'instance', NULL, 'flow/instance/index', NULL, 1, 0, 'C', '0', '0', 'flow:instance:list', 'shapes', 'admin', '2022-12-18 16:36:06', 'admin', '2026-07-01 22:50:53', NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (183, '版本编辑', 2, 11, 'process/:processId/version/:versionId', NULL, 'flow/process/flow', NULL, 1, 0, 'C', '1', '0', 'flow:version:edit', NULL, 'admin', '2022-12-21 00:52:26', 'admin', '2026-07-01 22:51:01', NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (184, '数据服务查询', 114, 1, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:dataService:query', NULL, 'admin', '2024-12-30 11:54:13', NULL, NULL, NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (185, '数据服务编辑', 1, 98, 'data/service/edit', 'DataServiceEdit', 'system/dataService/edit', NULL, 1, 0, 'C', '1', '0', 'system:dataService:edit', NULL, 'admin', '2024-12-30 11:55:11', 'admin', '2024-12-30 16:42:37', NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (186, '数据服务删除', 114, 3, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:dataService:remove', NULL, 'admin', '2024-12-30 11:55:31', NULL, NULL, NULL, 0, 100028);
INSERT INTO `sys_menu` VALUES (187, '数据服务新增', 114, 2, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', NULL, NULL, 'admin', '2026-07-03 10:37:28', NULL, NULL, NULL, 0, NULL);
INSERT INTO `sys_menu` VALUES (188, '国际化词典', 1, 99, 'i18n', NULL, 'system/i18n/index', '', 1, 0, 'C', '0', '0', 'system:i18n:list', 'globe', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-03 22:43:08', '国际化词典管理菜单', 0, 100028);

-- ----------------------------
-- Table structure for sys_dict_data
-- ----------------------------
DROP TABLE IF EXISTS `sys_dict_data`;
CREATE TABLE `sys_dict_data`  (
  `dict_data_id` bigint(20) NOT NULL COMMENT '字典编码',
  `dict_sort` int(11) NULL DEFAULT 0 COMMENT '字典排序',
  `dict_label` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '字典标签',
  `dict_value` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '字典键值',
  `dict_type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '字典类型',
  `css_class` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '样式属性（其他样式扩展）',
  `list_class` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '表格回显样式',
  `is_default` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '是否默认（Y是 N否）',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '状态（0正常 1停用）',
  `parent_id` bigint(20) NOT NULL COMMENT '父节点id',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  PRIMARY KEY (`dict_data_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '字典数据' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_dict_data
-- ----------------------------
INSERT INTO `sys_dict_data` VALUES (4, 1, '显示', '0', 'sys_show_hide', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2022-05-11 22:02:10', '显示菜单', 0);
INSERT INTO `sys_dict_data` VALUES (5, 2, '隐藏', '1', 'sys_show_hide', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '隐藏菜单', 0);
INSERT INTO `sys_dict_data` VALUES (6, 1, '正常', '0', 'sys_normal_disable', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2022-05-20 22:51:46', '正常状态', 0);
INSERT INTO `sys_dict_data` VALUES (7, 2, '停用', '1', 'sys_normal_disable', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '停用状态', 0);
INSERT INTO `sys_dict_data` VALUES (8, 1, '正常', '0', 'sys_job_status', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '正常状态', 0);
INSERT INTO `sys_dict_data` VALUES (9, 2, '暂停', '1', 'sys_job_status', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '停用状态', 0);
INSERT INTO `sys_dict_data` VALUES (10, 1, '默认', 'DEFAULT', 'sys_job_group', '', '', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '默认分组', 0);
INSERT INTO `sys_dict_data` VALUES (11, 2, '系统', 'SYSTEM', 'sys_job_group', '', '', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '系统分组', 0);
INSERT INTO `sys_dict_data` VALUES (12, 1, '是', 'Y', 'sys_yes_no', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '系统默认是', 0);
INSERT INTO `sys_dict_data` VALUES (13, 2, '否', 'N', 'sys_yes_no', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '系统默认否', 0);
INSERT INTO `sys_dict_data` VALUES (14, 1, '正常', 'normal', 'sys_priority_type', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2022-07-06 01:08:04', '正常', 0);
INSERT INTO `sys_dict_data` VALUES (15, 2, '加急', 'urgent', 'sys_priority_type', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2022-07-06 01:08:11', '加急', 0);
INSERT INTO `sys_dict_data` VALUES (18, 1, '新增', '1', 'sys_oper_type', '', 'info', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '新增操作', 0);
INSERT INTO `sys_dict_data` VALUES (19, 2, '修改', '2', 'sys_oper_type', '', 'info', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '修改操作', 0);
INSERT INTO `sys_dict_data` VALUES (20, 3, '删除', '3', 'sys_oper_type', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '删除操作', 0);
INSERT INTO `sys_dict_data` VALUES (21, 4, '授权', '4', 'sys_oper_type', '', 'primary', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '授权操作', 0);
INSERT INTO `sys_dict_data` VALUES (22, 5, '导出', '5', 'sys_oper_type', '', 'warning', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '导出操作', 0);
INSERT INTO `sys_dict_data` VALUES (23, 6, '导入', '6', 'sys_oper_type', '', 'warning', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '导入操作', 0);
INSERT INTO `sys_dict_data` VALUES (24, 7, '强退', '7', 'sys_oper_type', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '强退操作', 0);
INSERT INTO `sys_dict_data` VALUES (25, 8, '生成代码', '8', 'sys_oper_type', '', 'warning', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '生成操作', 0);
INSERT INTO `sys_dict_data` VALUES (26, 9, '清空数据', '9', 'sys_oper_type', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '清空操作', 0);
INSERT INTO `sys_dict_data` VALUES (27, 1, '成功', '0', 'sys_common_status', '', 'primary', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '正常状态', 0);
INSERT INTO `sys_dict_data` VALUES (28, 2, '失败', '1', 'sys_common_status', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '停用状态', 0);
INSERT INTO `sys_dict_data` VALUES (60, 0, '毫米', '毫米', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (61, 2, '厘米', '厘米', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (62, 4, '米', '米', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (63, 6, '千米', '千米', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (64, 8, '毫克', '毫克', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (65, 10, '克', '克', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (66, 12, '千克', '千克', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (67, 14, '吨', '吨', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (68, 16, '毫升', '毫升', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (69, 18, '升', '升', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (70, 20, 'cm³', 'cm³', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (71, 22, 'm³', 'm³', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (72, 24, 'cm²', 'cm²', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (73, 26, 'm²', 'm²', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (74, 28, '两', '两', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (75, 30, '斤', '斤', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (76, 32, '尺', '尺', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (77, 34, '亩', '亩', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (78, 36, '加仑', '加仑', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (79, 38, '磅', '磅', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (80, 40, '箱', '箱', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (81, 42, '个', '个', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (82, 44, '条', '条', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (83, 46, '包', '包', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (84, 48, '件', '件', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (85, 50, '桶', '桶', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (86, 52, '瓶', '瓶', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (87, 54, '本', '本', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (88, 56, '丝', '丝', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (89, 58, '套', '套', 'sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (90, 60, '张', '张', 'sys_unit_type', NULL, 'default', 'Y', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (161, 0, '测试', 'test', 'sys_flow_field', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-18 17:23:45', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (1860616524608442368, 0, '公告', 'notice', 'sys_message_type', 'fa-sign-hanging', 'warning', 'N', '0', 0, 'admin', '2024-11-24 17:28:43', 'admin', '2026-03-19 17:50:56', NULL, 0);
INSERT INTO `sys_dict_data` VALUES (1860616635468091392, 1, '流程', 'flow', 'sys_message_type', 'fa-stamp', 'success', 'N', '0', 0, 'admin', '2024-11-24 17:29:09', 'admin', '2026-03-19 17:51:21', NULL, 0);
INSERT INTO `sys_dict_data` VALUES (1860618461936488448, 2, '系统', 'system', 'sys_message_type', 'fa-bell', 'info', 'N', '0', 0, 'admin', '2024-11-24 17:36:25', 'admin', '2026-03-19 17:51:02', NULL, 0);
INSERT INTO `sys_dict_data` VALUES (1860618605608177664, 0, '日程', 'schedule', 'sys_message_type', 'fa-alarm-clock', 'primary', 'N', '0', 0, 'admin', '2024-11-24 17:36:59', 'admin', '2026-03-19 17:51:15', NULL, 0);
INSERT INTO `sys_dict_data` VALUES (2042162054340677632, 0, '系统内置', 'builtin', 'sys_data_service_category', NULL, 'default', 'N', '0', 0, 'admin', '2026-04-09 16:45:50', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (2042162266962530304, 0, '测试1', '1', 'sys_data_service_category', NULL, 'default', 'N', '0', 0, 'admin', '2026-04-09 16:46:41', NULL, NULL, NULL, 0);
INSERT INTO `sys_dict_data` VALUES (2042162308423225344, 0, '测2', '2', 'sys_data_service_category', NULL, 'default', 'N', '0', 2042162266962530304, 'admin', '2026-04-09 16:46:51', 'admin', '2026-05-25 17:22:25', NULL, 0);
INSERT INTO `sys_dict_data` VALUES (2042162343185616896, 0, '厕所2', '11', 'sys_data_service_category', NULL, 'default', 'N', '0', 2042162266962530304, 'admin', '2026-04-09 16:46:59', NULL, NULL, NULL, 0);

-- ----------------------------
-- Table structure for sys_i18n
-- ----------------------------
DROP TABLE IF EXISTS `sys_i18n`;
CREATE TABLE `sys_i18n`  (
  `i18n_id` bigint(20) NOT NULL COMMENT '翻译ID',
  `i18n_key` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '翻译键名',
  `lang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '语言',
  `i18n_value` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '翻译文本',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '创建人',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '修改人',
  `update_time` datetime NULL DEFAULT NULL COMMENT '修改时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志(0=正常,1=删除)',
  PRIMARY KEY (`i18n_id`) USING BTREE,
  UNIQUE INDEX `idx_i18n_key_lang`(`i18n_key`, `lang`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '国际化翻译表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_i18n
-- ----------------------------
INSERT INTO `sys_i18n` VALUES (1, 'sys.menu.1', 'zh-CN', '系统管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (2, 'sys.menu.2', 'zh-CN', '流程管理', NULL, '1900-01-01 00:00:00', 'admin', '2026-07-15 08:56:48', NULL, 0);
INSERT INTO `sys_i18n` VALUES (3, 'sys.menu.3', 'zh-CN', '系统工具', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (4, 'sys.menu.100', 'zh-CN', '用户管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (5, 'sys.menu.101', 'zh-CN', '角色管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (6, 'sys.menu.102', 'zh-CN', '菜单管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (7, 'sys.menu.103', 'zh-CN', '部门管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (8, 'sys.menu.104', 'zh-CN', '岗位管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (9, 'sys.menu.105', 'zh-CN', '字典管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (10, 'sys.menu.106', 'zh-CN', '参数设置', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (11, 'sys.menu.109', 'zh-CN', '在线用户', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (12, 'sys.menu.110', 'zh-CN', '定时任务', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (13, 'sys.menu.114', 'zh-CN', '数据服务', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (14, 'sys.menu.118', 'zh-CN', '系统日志', NULL, '1900-01-01 00:00:00', 'admin', '2026-07-09 14:51:23', NULL, 0);
INSERT INTO `sys_i18n` VALUES (15, 'sys.menu.119', 'zh-CN', '编码服务', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (16, 'sys.menu.179', 'zh-CN', '许可证', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (17, 'sys.menu.180', 'zh-CN', '流程管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (18, 'sys.menu.181', 'zh-CN', '流程定义', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (19, 'sys.menu.182', 'zh-CN', '流程实例', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (20, 'sys.menu.183', 'zh-CN', '版本编辑', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (21, 'sys.menu.185', 'zh-CN', '数据服务编辑', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (22, 'sys.menu.188', 'zh-CN', '国际化词典', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO `sys_i18n` VALUES (23, 'sys.menu.1', 'en-US', 'System Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (24, 'sys.menu.2', 'en-US', 'Process Management', NULL, '1900-01-01 00:00:00', 'admin', '2026-07-15 08:56:48', NULL, 0);
INSERT INTO `sys_i18n` VALUES (25, 'sys.menu.3', 'en-US', 'System Tools', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (26, 'sys.menu.100', 'en-US', 'User Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (27, 'sys.menu.101', 'en-US', 'Role Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (28, 'sys.menu.102', 'en-US', 'Menu Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (29, 'sys.menu.103', 'en-US', 'Dept Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (30, 'sys.menu.104', 'en-US', 'Position Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (31, 'sys.menu.105', 'en-US', 'Dict Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (32, 'sys.menu.106', 'en-US', 'Parameter Settings', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (33, 'sys.menu.109', 'en-US', 'Online Users', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (34, 'sys.menu.110', 'en-US', 'Scheduled Tasks', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (35, 'sys.menu.114', 'en-US', 'Data Services', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (36, 'sys.menu.118', 'en-US', 'System Logs', NULL, '1900-01-01 00:00:00', 'admin', '2026-07-09 14:51:23', NULL, 0);
INSERT INTO `sys_i18n` VALUES (37, 'sys.menu.119', 'en-US', 'Code Services', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (38, 'sys.menu.179', 'en-US', 'License', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (39, 'sys.menu.180', 'en-US', 'Process Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (40, 'sys.menu.181', 'en-US', 'Process Definition', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (41, 'sys.menu.182', 'en-US', 'Process Instances', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (42, 'sys.menu.183', 'en-US', 'Version Editor', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (43, 'sys.menu.185', 'en-US', 'Data Services Editor', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO `sys_i18n` VALUES (44, 'sys.menu.188', 'en-US', 'I18n Dict', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);

-- ----------------------------
-- Table structure for sys_code_rule_part
-- ----------------------------
DROP TABLE IF EXISTS `sys_code_rule_part`;
CREATE TABLE `sys_code_rule_part`  (
  `part_id` bigint(20) NOT NULL COMMENT '编码片段规则',
  `rule_id` bigint(20) NOT NULL COMMENT '规则id',
  `part_type` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '片段类型 string/calc/date',
  `part_value` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '固定字符串/字符规则/日期格式',
  `reset_type` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '重置类型 week/month/quarter/year',
  `week_start_day` int(2) NOT NULL COMMENT '周开始时间 0周日 1周一',
  `reset_time` datetime NULL DEFAULT NULL COMMENT '下次重置日期',
  `current_index` varchar(800) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '当前位置',
  `sort` int(11) NOT NULL DEFAULT 0 COMMENT '排序字段',
  `is_del` int(11) NOT NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `remark` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `is_skip_zero` int(11) NOT NULL DEFAULT 0 COMMENT '是否跳过0',
  PRIMARY KEY (`part_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '序号生成规则片段' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_code_rule_part
-- ----------------------------
INSERT INTO `sys_code_rule_part` VALUES (1, 1, 'string', '-', 'week', 1, '2022-10-10 00:00:00', '[{\"StartIndex\":10,\"EndIndex\":35,\"CurrentIndex\":10,\"StartChar\":\"a\",\"EndChar\":\"z\",\"CurrentChar\":\"a\"},{\"StartIndex\":12,\"EndIndex\":13,\"CurrentIndex\":13,\"StartChar\":\"c\",\"EndChar\":\"d\",\"CurrentChar\":\"d\"},{\"StartIndex\":2,\"EndIndex\":8,\"CurrentIndex\":4,\"StartChar\":\"2\",\"EndChar\":\"8\",\"CurrentChar\":\"4\"}]', 3, 0, NULL, 'admin', '2022-07-26 22:41:18', 'admin', '2022-12-16 18:10:19', 0);
INSERT INTO `sys_code_rule_part` VALUES (2, 1, 'string', 'HWKC-', NULL, 1, NULL, NULL, 1, 0, NULL, 'admin', '2022-07-26 23:02:45', 'admin', '2022-12-16 18:10:19', 0);
INSERT INTO `sys_code_rule_part` VALUES (3, 1, 'date', 'yyyyMM', NULL, 1, NULL, NULL, 2, 0, NULL, 'admin', '2022-07-26 23:54:43', 'admin', '2022-12-16 18:10:19', 0);
INSERT INTO `sys_code_rule_part` VALUES (4, 1, 'calc', '[0-9][0-9][0-9][0-9][0-9]', 'month', 1, '2022-11-01 00:00:00', '[{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":4,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"4\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":4,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"4\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":4,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"4\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":5,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"5\"}]', 4, 0, NULL, 'admin', '2022-07-27 21:54:18', 'admin', '2022-12-16 18:10:19', 1);
INSERT INTO `sys_code_rule_part` VALUES (5, 2, 'string', 'JHD-', 'week', 1, '2022-10-17 00:00:00', '[{\"StartIndex\":10,\"EndIndex\":35,\"CurrentIndex\":11,\"StartChar\":\"a\",\"EndChar\":\"z\",\"CurrentChar\":\"b\"}]', 1, 0, NULL, 'admin', '2022-07-28 22:40:24', 'admin', '2022-12-16 17:19:40', 0);
INSERT INTO `sys_code_rule_part` VALUES (6, 2, 'date', 'yyyyMM', 'week', 0, '2022-10-23 00:00:00', '[{\"StartIndex\":10,\"EndIndex\":35,\"CurrentIndex\":10,\"StartChar\":\"a\",\"EndChar\":\"z\",\"CurrentChar\":\"a\"},{\"StartIndex\":10,\"EndIndex\":35,\"CurrentIndex\":11,\"StartChar\":\"a\",\"EndChar\":\"z\",\"CurrentChar\":\"b\"}]', 2, 0, NULL, 'admin', '2022-07-28 22:40:44', 'admin', '2022-12-16 17:19:40', 0);
INSERT INTO `sys_code_rule_part` VALUES (7, 2, 'string', '-', 'week', 1, '2022-10-17 00:00:00', '[{\"StartIndex\":10,\"EndIndex\":35,\"CurrentIndex\":11,\"StartChar\":\"a\",\"EndChar\":\"z\",\"CurrentChar\":\"b\"}]', 3, 0, NULL, 'admin', '2022-07-28 22:40:44', 'admin', '2022-12-16 17:19:40', 0);
INSERT INTO `sys_code_rule_part` VALUES (8, 2, 'calc', '[0-9][0-9][0-9][0-9][0-9]', 'month', 4, '2022-11-01 00:00:00', '[{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":2,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"2\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":1,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"1\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":7,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"7\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":3,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"3\"}]', 4, 0, NULL, 'admin', '2022-07-28 22:46:15', 'admin', '2022-12-16 17:19:40', 1);
INSERT INTO `sys_code_rule_part` VALUES (9, 3, 'string', 'LLD-', NULL, 1, NULL, NULL, 1, 0, NULL, 'admin', '2022-10-10 21:22:14', '刘洋', '2022-12-16 17:06:37', 0);
INSERT INTO `sys_code_rule_part` VALUES (10, 3, 'date', 'yyyyMM', NULL, 1, NULL, NULL, 2, 0, NULL, 'admin', '2022-10-10 21:22:14', '刘洋', '2022-12-16 17:06:38', 0);
INSERT INTO `sys_code_rule_part` VALUES (11, 3, 'string', '-', NULL, 1, NULL, NULL, 3, 0, NULL, 'admin', '2022-10-10 21:22:14', '刘洋', '2022-12-16 17:06:38', 0);
INSERT INTO `sys_code_rule_part` VALUES (12, 3, 'calc', '[0-9][0-9][0-9][0-9][0-9]', 'month', 1, '2022-11-01 00:00:00', '[{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":2,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"2\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":3,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"3\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":4,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"4\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":5,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"5\"}]', 4, 0, NULL, 'admin', '2022-10-10 21:22:15', '刘洋', '2022-12-16 17:06:38', 1);
INSERT INTO `sys_code_rule_part` VALUES (19, 4, 'string', 'CCK', NULL, 1, NULL, NULL, 1, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-12-16 17:19:46', 0);
INSERT INTO `sys_code_rule_part` VALUES (20, 4, 'date', 'yyyyMM', NULL, 1, NULL, NULL, 2, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-12-16 17:19:46', 0);
INSERT INTO `sys_code_rule_part` VALUES (21, 4, 'string', '-', NULL, 1, NULL, NULL, 3, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-12-16 17:19:46', 0);
INSERT INTO `sys_code_rule_part` VALUES (22, 4, 'calc', '[0-9][0-9][0-9][0-9][0-9]', 'month', 1, '2022-11-01 00:00:00', '[{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":1,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"1\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":4,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"4\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":6,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"6\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":5,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"5\"}]', 4, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-12-16 17:19:46', 1);
INSERT INTO `sys_code_rule_part` VALUES (23, 5, 'string', 'SJD', NULL, 1, NULL, NULL, 1, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-10-31 00:11:25', 0);
INSERT INTO `sys_code_rule_part` VALUES (24, 5, 'date', 'yyyyMM', NULL, 1, NULL, NULL, 2, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-10-31 00:11:25', 0);
INSERT INTO `sys_code_rule_part` VALUES (25, 5, 'string', '-', NULL, 1, NULL, NULL, 3, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-10-31 00:11:25', 0);
INSERT INTO `sys_code_rule_part` VALUES (26, 5, 'calc', '[0-9][0-9][0-9][0-9][0-9]', 'month', 1, '2022-11-01 00:00:00', '[{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":3,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"3\"}]', 4, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-10-31 00:11:25', 1);
INSERT INTO `sys_code_rule_part` VALUES (27, 6, 'string', 'KCPD', NULL, 1, NULL, NULL, 1, 0, NULL, 'admin', '2022-10-17 00:16:44', '管理员122', '2024-11-17 09:07:21', 0);
INSERT INTO `sys_code_rule_part` VALUES (28, 6, 'date', 'yyyyMM', NULL, 1, NULL, NULL, 2, 0, NULL, 'admin', '2022-10-17 00:16:44', '管理员122', '2024-11-17 09:07:21', 0);
INSERT INTO `sys_code_rule_part` VALUES (29, 6, 'string', '-', NULL, 1, NULL, NULL, 3, 0, NULL, 'admin', '2022-10-17 00:16:44', '管理员122', '2024-11-17 09:07:21', 0);
INSERT INTO `sys_code_rule_part` VALUES (30, 6, 'calc', '[0-9][0-9][0-9][0-9][0-9]', 'month', 1, '2024-12-01 00:00:00', '[{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":8,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"8\"}]', 4, 0, NULL, 'admin', '2022-10-17 00:16:44', '管理员122', '2024-11-17 09:07:21', 1);
INSERT INTO `sys_code_rule_part` VALUES (31, 7, 'string', 'HW', NULL, 1, NULL, NULL, 1, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-11-11 22:05:33', 0);
INSERT INTO `sys_code_rule_part` VALUES (32, 7, 'date', 'yyyyMM', NULL, 1, NULL, NULL, 2, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-11-11 22:05:33', 0);
INSERT INTO `sys_code_rule_part` VALUES (33, 7, 'string', '-', NULL, 1, NULL, NULL, 3, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-11-11 22:05:33', 0);
INSERT INTO `sys_code_rule_part` VALUES (34, 7, 'calc', '[0-9][0-9][0-9][0-9][0-9]', 'month', 1, '2022-12-01 00:00:00', '[{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":2,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"2\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":7,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"7\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":8,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"8\"}]', 4, 0, NULL, 'admin', '2022-10-17 00:16:44', 'admin', '2022-11-11 22:05:33', 1);
INSERT INTO `sys_code_rule_part` VALUES (35, 8, 'string', 'SHD-', NULL, 1, NULL, NULL, 1, 0, NULL, 'admin', '2022-10-17 00:16:44', '管理员122', '2026-06-02 00:15:17', 0);
INSERT INTO `sys_code_rule_part` VALUES (36, 8, 'date', 'yyyyMM', NULL, 1, NULL, NULL, 2, 0, NULL, 'admin', '2022-10-17 00:16:44', '管理员122', '2026-06-02 00:15:17', 0);
INSERT INTO `sys_code_rule_part` VALUES (37, 8, 'string', '-', NULL, 1, NULL, NULL, 3, 0, NULL, 'admin', '2022-10-17 00:16:44', '管理员122', '2026-06-02 00:15:17', 0);
INSERT INTO `sys_code_rule_part` VALUES (38, 8, 'calc', '[0-9][0-9][0-9][0-9][0-9]', 'month', 1, '2026-07-01 00:00:00', '[{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":0,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"0\"},{\"StartIndex\":0,\"EndIndex\":9,\"CurrentIndex\":9,\"StartChar\":\"0\",\"EndChar\":\"9\",\"CurrentChar\":\"9\"}]', 4, 0, NULL, 'admin', '2022-10-17 00:16:44', '管理员122', '2026-06-02 00:15:17', 1);

-- ----------------------------
-- Table structure for sys_role
-- ----------------------------
DROP TABLE IF EXISTS `sys_role`;
CREATE TABLE `sys_role`  (
  `role_id` bigint(20) NOT NULL COMMENT '角色ID',
  `role_name` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色名称',
  `role_key` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色权限字符串',
  `role_sort` int(11) NOT NULL COMMENT '显示顺序',
  `data_scope` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限）',
  `menu_check_strictly` tinyint(1) NULL DEFAULT 1 COMMENT '菜单树选择项是否关联显示',
  `dept_check_strictly` tinyint(1) NULL DEFAULT 1 COMMENT '部门树选择项是否关联显示',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色状态（0正常 1停用）',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`role_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '角色信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_role
-- ----------------------------
INSERT INTO `sys_role` VALUES (1, '超级管理员', 'admin', 1, '1', 1, 1, '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2022-05-27 23:55:48', '超级管理员');
INSERT INTO `sys_role` VALUES (2, '普通角色', 'common', 2, '2', 1, 1, '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2022-11-11 10:12:06', '普通角色');
INSERT INTO `sys_role` VALUES (3, 'ceshi', 'ceshi', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-20 09:44:38', 'admin', '2022-10-20 09:44:46', NULL);
INSERT INTO `sys_role` VALUES (4, '仓管1', '1', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-21 14:01:07', 'admin', '2022-10-21 14:12:27', NULL);
INSERT INTO `sys_role` VALUES (5, '拣货', '拣货', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-21 14:13:09', 'admin', '2022-10-21 14:14:56', NULL);
INSERT INTO `sys_role` VALUES (6, 'test-bug', '2', 1, NULL, 1, 1, '0', 1, 'admin', '2022-10-21 19:52:10', NULL, NULL, NULL);
INSERT INTO `sys_role` VALUES (7, 'ceshi1', '11', 2, NULL, 1, 1, '0', 1, 'admin', '2022-10-23 21:11:51', 'admin', '2022-10-23 21:12:42', NULL);
INSERT INTO `sys_role` VALUES (8, 'ceshi3', '3', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-23 21:16:28', NULL, NULL, '4');
INSERT INTO `sys_role` VALUES (9, '测试0001', '4', 1, NULL, 1, 1, '0', 1, 'admin', '2022-10-24 17:06:17', NULL, NULL, NULL);
INSERT INTO `sys_role` VALUES (10, '销售组别', '9', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-25 14:13:26', NULL, NULL, NULL);
INSERT INTO `sys_role` VALUES (11, '普通管理员', '3', 3, '2', 1, 1, '0', 0, 'admin', '2022-11-11 13:53:22', 'admin', '2026-06-22 21:56:46', NULL);
INSERT INTO `sys_role` VALUES (12, '流程测试', 'lccs', 0, NULL, 1, 1, '0', 0, 'admin', '2023-08-10 22:16:26', NULL, NULL, NULL);
INSERT INTO `sys_role` VALUES (13, 'a', '1', 0, NULL, 1, 1, '0', 1, 'admin', '2024-11-01 22:52:01', NULL, NULL, NULL);

-- ----------------------------
-- Table structure for sys_dict_type
-- ----------------------------
DROP TABLE IF EXISTS `sys_dict_type`;
CREATE TABLE `sys_dict_type`  (
  `dict_id` bigint(20) NOT NULL COMMENT '字典主键',
  `dict_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '字典名称',
  `dict_type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '字典类型',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '状态（0正常 1停用）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  PRIMARY KEY (`dict_id`) USING BTREE,
  UNIQUE INDEX `dict_type`(`dict_type`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '字典类型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_dict_type
-- ----------------------------
INSERT INTO `sys_dict_type` VALUES (1860613374338011136, '站内信类型', 'sys_message_type', '0', 'admin', '2024-11-24 17:16:12', NULL, NULL, '站内信类型', 0);
INSERT INTO `sys_dict_type` VALUES (1860614565067362304, '菜单状态', 'sys_show_hide', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '菜单状态列表', 0);
INSERT INTO `sys_dict_type` VALUES (1860614591415980032, '系统开关', 'sys_normal_disable', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '系统开关列表', 0);
INSERT INTO `sys_dict_type` VALUES (1860614621262647296, '任务状态', 'sys_job_status', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '任务状态列表', 0);
INSERT INTO `sys_dict_type` VALUES (1860614640476753920, '任务分组', 'sys_job_group', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '任务分组列表', 0);
INSERT INTO `sys_dict_type` VALUES (1860615007348330496, '系统是否', 'sys_yes_no', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '系统是否列表', 0);
INSERT INTO `sys_dict_type` VALUES (1860615007348330497, '优先级类型', 'sys_priority_type', '0', 'admin', '2022-04-15 15:51:02', 'admin', '2022-07-06 01:06:05', '优先级类型列表', 0);
INSERT INTO `sys_dict_type` VALUES (1860615007348330499, '系统状态', 'sys_common_status', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '登录状态列表', 0);
INSERT INTO `sys_dict_type` VALUES (1860615007348330500, '操作类型', 'sys_oper_type', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '操作类型列表', 0);
INSERT INTO `sys_dict_type` VALUES (1860615007348330501, '序号生成类型', 'sys_code_type', '0', 'admin', '2022-07-24 22:57:31', NULL, NULL, '系统序号生成类型', 0);
INSERT INTO `sys_dict_type` VALUES (1860615007348330503, '流程模块', 'sys_flow_field', '0', 'admin', '2022-12-18 17:22:50', NULL, NULL, '流程模块类型', 0);
INSERT INTO `sys_dict_type` VALUES (1860615007348330505, '系统单位', 'sys_unit_type', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '系统单位类型', 0);
INSERT INTO `sys_dict_type` VALUES (1872169845576044544, '数据服务分类', 'sys_data_service_category', '0', 'admin', '2024-12-26 14:37:29', 'admin', '2024-12-26 14:37:40', '数据服务类型', 0);

-- ----------------------------
-- Table structure for flow_process_version
-- ----------------------------
DROP TABLE IF EXISTS `flow_process_version`;
CREATE TABLE `flow_process_version`  (
  `version_id` bigint(20) NOT NULL COMMENT '主键id',
  `process_id` bigint(20) NOT NULL COMMENT '流程id',
  `version` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '流程版本',
  `content` varchar(3000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '流程内容',
  `is_lock` int(11) NOT NULL DEFAULT 0 COMMENT '是否锁定',
  `remark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `is_del` int(11) NOT NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`version_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '审批流定义' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of flow_process_version
-- ----------------------------
INSERT INTO `flow_process_version` VALUES (1, 4, 'V1', '', 0, NULL, 0, 'admin', '2022-12-18 23:57:43', NULL, NULL);
INSERT INTO `flow_process_version` VALUES (4, 5, 'V1', '{\"nodes\":[{\"id\":\"node7lkvHOF4YuZX2Em5\",\"width\":68,\"height\":76,\"coordinate\":[406.1999816894531,310],\"meta\":{\"name\":\"结束\",\"icon\":\"flow-end\",\"type\":\"end\"}},{\"id\":\"noded7wK2b5X038mAMct\",\"width\":68,\"height\":76,\"coordinate\":[136,50],\"meta\":{\"name\":\"开始\",\"icon\":\"flow-start\",\"type\":\"start\"}},{\"id\":\"nodeuTJPUfAjRwgUBi9a\",\"width\":68,\"height\":76,\"coordinate\":[239.99996948242188,207],\"meta\":{\"name\":\"处理\",\"icon\":\"flow-task\",\"type\":\"task\",\"isNoRepeatHandle\":false,\"handleBy\":[{\"type\":\"user\",\"handleId\":1,\"handleName\":\"管理员122\"}],\"handleRule\":\"one\"}}],\"lines\":[{\"id\":\"linkB6fbIAKdSHGhG66X\",\"startId\":\"nodeuTJPUfAjRwgUBi9a\",\"endId\":\"node7lkvHOF4YuZX2Em5\",\"startAt\":[34,76],\"endAt\":[0,38],\"meta\":null},{\"id\":\"link9Ctv2WgIrlibwd36\",\"startId\":\"noded7wK2b5X038mAMct\",\"endId\":\"nodeuTJPUfAjRwgUBi9a\",\"startAt\":[34,76],\"endAt\":[34,0],\"meta\":null}]}', 1, NULL, 0, 'admin', '2022-12-23 21:23:22', 'admin', '2023-08-01 22:38:27');
INSERT INTO `flow_process_version` VALUES (5, 6, 'V1', '{\"nodes\":[{\"id\":\"nodeuiUwFZ13dcQ5ZJmV\",\"x\":0,\"y\":0,\"type\":\"task\",\"meta\":{\"name\":\"处理\",\"icon\":\"flow-task\",\"type\":\"task\",\"isNoRepeatHandle\":false,\"handleBy\":[],\"handleRule\":\"all\"}},{\"id\":\"nodemlPD9KwNxXA3hgrN\",\"x\":0,\"y\":0,\"type\":\"task\",\"meta\":{\"name\":\"通知\",\"icon\":\"flow-notice\",\"type\":\"notice\",\"isNoRepeatHandle\":false,\"handleBy\":[]}},{\"id\":\"nodedKGGCYHDx9LP7qoS\",\"x\":0,\"y\":0,\"type\":\"task\",\"meta\":{\"name\":\"处理\",\"icon\":\"flow-task\",\"type\":\"task\",\"isNoRepeatHandle\":false,\"handleBy\":[],\"handleRule\":\"all\"}},{\"id\":\"nodegZh9SZnxE4fN0lFm\",\"x\":0,\"y\":0,\"type\":\"task\",\"meta\":{\"name\":\"开始\",\"icon\":\"flow-start\",\"type\":\"start\"}},{\"id\":\"nodeDgxhSW52i4YrdQdT\",\"x\":0,\"y\":0,\"type\":\"task\",\"meta\":{\"name\":\"结束\",\"icon\":\"flow-end\",\"type\":\"end\"}},{\"id\":\"297b9d5a-7ed5-404d-8ddb-ab3201e4bb0e\",\"x\":-100,\"y\":-100,\"type\":\"notice\",\"meta\":{\"name\":\"通知节点\",\"handleBy\":[{\"type\":\"dept\",\"handleId\":\"103\",\"handleName\":\"研发部门\"},{\"type\":\"dept\",\"handleId\":\"2066408574942318592\",\"handleName\":\"采购科\"},{\"type\":\"dept\",\"handleId\":\"105\",\"handleName\":\"测试部门\"},{\"type\":\"dept\",\"handleId\":\"106\",\"handleName\":\"财务部门\"},{\"type\":\"dept\",\"handleId\":\"2066408570584436736\",\"handleName\":\"仓储科\"}]}},{\"id\":\"e811fc8e-ac9f-49d3-ba49-1fa53b92f828\",\"x\":-110,\"y\":40,\"type\":\"branch\",\"meta\":{\"name\":\"分支节点\",\"conditions\":[]}}],\"lines\":[{\"id\":\"eb6e6f6a-2382-4123-86bf-403df33f7314\",\"source\":\"e811fc8e-ac9f-49d3-ba49-1fa53b92f828\",\"sourcePoint\":\"right\",\"target\":\"nodeuiUwFZ13dcQ5ZJmV\",\"targetPoint\":\"left\"}]}', 0, NULL, 0, 'admin', '2023-07-24 23:02:05', 'admin', '2026-07-18 23:19:39');
INSERT INTO `flow_process_version` VALUES (7, 6, 'V2', '{\"nodes\":[{\"id\":\"nodeGkrHnYxrisHXLQ9i\",\"width\":68,\"height\":76,\"coordinate\":[261.1750183105469,31],\"meta\":{\"name\":\"开始\",\"icon\":\"flow-start\",\"type\":\"start\"}},{\"id\":\"noderEZA7wtQXIvviQIH\",\"width\":68,\"height\":76,\"coordinate\":[215.18746948242188,228],\"meta\":{\"name\":\"结束\",\"icon\":\"flow-end\",\"type\":\"end\"}}],\"lines\":[{\"id\":\"linkdRe2iGcjq7Ec9YvH\",\"startId\":\"nodeGkrHnYxrisHXLQ9i\",\"endId\":\"noderEZA7wtQXIvviQIH\",\"startAt\":[34,76],\"endAt\":[34,0],\"meta\":null}]}', 0, NULL, 0, 'admin', '2023-08-03 21:50:50', 'admin', '2023-07-24 23:04:26');
INSERT INTO `flow_process_version` VALUES (8, 5, 'V2', '{\"nodes\":[{\"id\":\"node23su3SptduWHsxPW\",\"width\":120,\"height\":30,\"coordinate\":[270.1999816894531,379.1999969482422],\"meta\":{\"name\":\"结束\",\"icon\":\"flow-end\",\"type\":\"end\"}},{\"id\":\"nodeURjwQ6FOSQvHRXXU\",\"width\":120,\"height\":30,\"coordinate\":[284.1874694824219,45],\"meta\":{\"name\":\"开始\",\"icon\":\"flow-start\",\"type\":\"start\"}},{\"id\":\"nodebGurfuAmSBy3BkLw\",\"width\":120,\"height\":30,\"coordinate\":[269.1999816894531,224.40000915527344],\"meta\":{\"name\":\"处理\",\"icon\":\"flow-task\",\"type\":\"task\",\"isNoRepeatHandle\":false,\"handleBy\":[{\"type\":\"user\",\"handleId\":12,\"handleName\":\"流程测试\"}],\"handleRule\":\"all\"}}],\"lines\":[{\"id\":\"link3nB9WWIxm2eH6y1Z\",\"startId\":\"nodeURjwQ6FOSQvHRXXU\",\"endId\":\"nodebGurfuAmSBy3BkLw\",\"startAt\":[60,30],\"endAt\":[60,0],\"meta\":null},{\"id\":\"linkB3XGSCNnCxmF3Gef\",\"startId\":\"nodebGurfuAmSBy3BkLw\",\"endId\":\"node23su3SptduWHsxPW\",\"startAt\":[60,30],\"endAt\":[60,0],\"meta\":null}]}', 1, NULL, 0, 'admin', '2023-08-10 22:14:56', 'admin', '2023-08-10 22:37:10');
INSERT INTO `flow_process_version` VALUES (9, 5, 'V3', '{\"nodes\":[{\"id\":\"node23su3SptduWHsxPW\",\"width\":120,\"height\":30,\"coordinate\":[270.1999816894531,379.1999969482422],\"meta\":{\"name\":\"结束\",\"icon\":\"flow-end\",\"type\":\"end\"}},{\"id\":\"nodeURjwQ6FOSQvHRXXU\",\"width\":120,\"height\":30,\"coordinate\":[284.1874694824219,45],\"meta\":{\"name\":\"开始\",\"icon\":\"flow-start\",\"type\":\"start\"}},{\"id\":\"nodebGurfuAmSBy3BkLw\",\"width\":120,\"height\":30,\"coordinate\":[269.1999816894531,224.40000915527344],\"meta\":{\"name\":\"处理\",\"icon\":\"flow-task\",\"type\":\"task\",\"isNoRepeatHandle\":false,\"handleBy\":[{\"type\":\"role\",\"handleId\":12,\"handleName\":\"流程测试\"}],\"handleRule\":\"all\"}}],\"lines\":[{\"id\":\"linkq5Y0zVHKG5SKBxgS\",\"startId\":\"nodeURjwQ6FOSQvHRXXU\",\"endId\":\"nodebGurfuAmSBy3BkLw\",\"startAt\":[60,30],\"endAt\":[60,0],\"meta\":null},{\"id\":\"linkfIDIEStP2mnMvASc\",\"startId\":\"nodebGurfuAmSBy3BkLw\",\"endId\":\"node23su3SptduWHsxPW\",\"startAt\":[60,30],\"endAt\":[60,0],\"meta\":null}]}', 1, NULL, 0, 'admin', '2023-08-10 22:51:45', 'admin', '2023-08-10 22:53:41');
INSERT INTO `flow_process_version` VALUES (10, 5, 'V4', '{\"nodes\":[{\"id\":\"node23su3SptduWHsxPW\",\"width\":120,\"height\":30,\"coordinate\":[270.1999816894531,379.1999969482422],\"meta\":{\"name\":\"结束\",\"icon\":\"flow-end\",\"type\":\"end\"}},{\"id\":\"nodeURjwQ6FOSQvHRXXU\",\"width\":120,\"height\":30,\"coordinate\":[284.1874694824219,45],\"meta\":{\"name\":\"开始\",\"icon\":\"flow-start\",\"type\":\"start\"}},{\"id\":\"nodebGurfuAmSBy3BkLw\",\"width\":120,\"height\":30,\"coordinate\":[269.1999816894531,224.40000915527344],\"meta\":{\"name\":\"处理\",\"icon\":\"flow-task\",\"type\":\"task\",\"isNoRepeatHandle\":false,\"handleBy\":[{\"type\":\"role\",\"handleId\":12,\"handleName\":\"流程测试\"}],\"handleRule\":\"one\"}}],\"lines\":[{\"id\":\"linkKvXaloUPN3KDYpOc\",\"startId\":\"nodebGurfuAmSBy3BkLw\",\"endId\":\"node23su3SptduWHsxPW\",\"startAt\":[60,30],\"endAt\":[60,0],\"meta\":null},{\"id\":\"linkyWU5C9NtNxvlsfwb\",\"startId\":\"nodeURjwQ6FOSQvHRXXU\",\"endId\":\"nodebGurfuAmSBy3BkLw\",\"startAt\":[60,30],\"endAt\":[60,0],\"meta\":null}]}', 1, NULL, 0, 'admin', '2023-08-10 22:55:38', 'admin', '2023-08-10 22:56:20');
INSERT INTO `flow_process_version` VALUES (2053660312732831744, 2053660312145629184, 'V1', '{\"nodes\":[{\"id\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"x\":-60,\"y\":140,\"type\":\"task\",\"meta\":{\"name\":\"处理节点13\",\"isNoRepeatHandle\":true,\"handleBy\":[{\"type\":\"author\"}],\"handleRule\":\"one\"}},{\"id\":\"728321dc-877f-4778-9546-87df2977638b\",\"x\":380,\"y\":80,\"type\":\"end\",\"meta\":{\"name\":\"结束节点\"}},{\"id\":\"2df47a71-24d9-4c90-b34e-4e855c2e6c36\",\"x\":-40,\"y\":-60,\"type\":\"start\",\"meta\":{\"name\":\"开始节点\"}}],\"lines\":[{\"id\":\"bbbd0b98-8c31-49fd-918b-ee7061f102df\",\"source\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"sourcePoint\":\"right\",\"target\":\"728321dc-877f-4778-9546-87df2977638b\",\"targetPoint\":\"left\"},{\"id\":\"46459930-fcae-4287-86e5-61418f5b2398\",\"source\":\"2df47a71-24d9-4c90-b34e-4e855c2e6c36\",\"sourcePoint\":\"bottom\",\"target\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"targetPoint\":\"top\"}]}', 1, NULL, 0, 'admin', '2026-05-11 10:15:48', 'admin', '2026-06-03 14:17:27');
INSERT INTO `flow_process_version` VALUES (2057406060221829120, 2053660312145629184, 'V2', '{\"nodes\":[{\"id\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"x\":-60,\"y\":140,\"type\":\"task\",\"meta\":{\"name\":\"处理节点13\",\"isNoRepeatHandle\":true,\"handleBy\":[],\"handleRule\":\"one\"}},{\"id\":\"3610bba7-0a9c-459f-ad92-28c708772510\",\"x\":120,\"y\":300,\"type\":\"notice\",\"meta\":{\"name\":\"通知节点\"}},{\"id\":\"f2636d88-5f28-4594-84c9-a512e4535e6e\",\"x\":-120,\"y\":-40,\"type\":\"start\",\"meta\":{\"name\":\"结束节点\"}},{\"id\":\"728321dc-877f-4778-9546-87df2977638b\",\"x\":320,\"y\":440,\"type\":\"end\",\"meta\":{\"name\":\"结束节点\"}}],\"lines\":[{\"id\":\"e791850f-1e9b-44ad-8f47-67ccb1218ae3\",\"source\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"sourcePoint\":\"right\",\"target\":\"3610bba7-0a9c-459f-ad92-28c708772510\",\"targetPoint\":\"left\"},{\"id\":\"cc32abf7-8b5a-426e-b8e6-eda65cece3ab\",\"source\":\"f2636d88-5f28-4594-84c9-a512e4535e6e\",\"sourcePoint\":\"bottom\",\"target\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"targetPoint\":\"top\"},{\"id\":\"4231d2b8-bb62-4600-8172-611680b7f4b7\",\"source\":\"3610bba7-0a9c-459f-ad92-28c708772510\",\"sourcePoint\":\"right\",\"target\":\"728321dc-877f-4778-9546-87df2977638b\",\"targetPoint\":\"top\"}]}', 0, NULL, 0, 'admin', '2026-05-21 18:20:04', 'admin', '2026-05-21 18:16:59');
INSERT INTO `flow_process_version` VALUES (2058724112049770496, 2053660312145629184, 'V3', '{\"nodes\":[{\"id\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"x\":-60,\"y\":140,\"type\":\"task\",\"meta\":{\"name\":\"处理节点13\",\"isNoRepeatHandle\":true,\"handleBy\":[],\"handleRule\":\"one\"}},{\"id\":\"3610bba7-0a9c-459f-ad92-28c708772510\",\"x\":160,\"y\":140,\"type\":\"notice\",\"meta\":{\"name\":\"通知节点\"}},{\"id\":\"f2636d88-5f28-4594-84c9-a512e4535e6e\",\"x\":-120,\"y\":-40,\"type\":\"start\",\"meta\":{\"name\":\"结束节点\"}},{\"id\":\"728321dc-877f-4778-9546-87df2977638b\",\"x\":380,\"y\":80,\"type\":\"end\",\"meta\":{\"name\":\"结束节点\"}}],\"lines\":[{\"id\":\"e791850f-1e9b-44ad-8f47-67ccb1218ae3\",\"source\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"sourcePoint\":\"right\",\"target\":\"3610bba7-0a9c-459f-ad92-28c708772510\",\"targetPoint\":\"left\"},{\"id\":\"cc32abf7-8b5a-426e-b8e6-eda65cece3ab\",\"source\":\"f2636d88-5f28-4594-84c9-a512e4535e6e\",\"sourcePoint\":\"bottom\",\"target\":\"4c634d4c-cde6-4934-bb38-563a90bb4dd9\",\"targetPoint\":\"top\"},{\"id\":\"4231d2b8-bb62-4600-8172-611680b7f4b7\",\"source\":\"3610bba7-0a9c-459f-ad92-28c708772510\",\"sourcePoint\":\"right\",\"target\":\"728321dc-877f-4778-9546-87df2977638b\",\"targetPoint\":\"left\"}]}', 0, NULL, 1, 'admin', '2026-05-25 09:37:32', 'admin', '2026-05-22 23:54:54');
INSERT INTO `flow_process_version` VALUES (2078302135375761408, 2078302135107325952, 'V1', '', 0, NULL, 0, 'admin', '2026-07-18 10:13:37', NULL, NULL);
INSERT INTO `flow_process_version` VALUES (2078302191910785024, 2078302135107325952, 'V2', '{\"nodes\":[],\"lines\":[]}', 0, NULL, 0, 'admin', '2026-07-18 10:13:50', NULL, NULL);

-- ----------------------------
-- Table structure for sys_dept
-- ----------------------------
DROP TABLE IF EXISTS `sys_dept`;
CREATE TABLE `sys_dept`  (
  `dept_id` bigint(20) NOT NULL COMMENT '部门id',
  `parent_id` bigint(20) NOT NULL COMMENT '父部门id',
  `ancestors` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '祖级列表',
  `dept_name` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '部门名称',
  `order_num` int(11) NULL DEFAULT 0 COMMENT '显示顺序',
  `leader_user_id` bigint(20) NULL DEFAULT NULL COMMENT '负责人',
  `phone` varchar(11) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '联系电话',
  `email` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '邮箱',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '部门状态（0正常 1停用）',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `dept_level` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '部门等级',
  PRIMARY KEY (`dept_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '部门' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_dept
-- ----------------------------
INSERT INTO `sys_dept` VALUES (2066408560685879296, 2066415154064658432, '2066415154064658432', '技术研发部', 1085091401, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:47', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408561927393280, 2066415154064658432, '2066415154064658432', '销售部', 1084505570, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:48', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408562711728128, 2066415154064658432, '2066415154064658432', '生产部', 1084845536, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:48', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408563538006016, 2066415154064658432, '2066415154064658432', '供应链部', 1084944419, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:48', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408565375111168, 2066415154064658432, '2066415154064658432', '财务部', 1084942359, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:49', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408566247526400, 2066408562711728128, '2066415154064658432,2066408562711728128', '设备维护科', 1084562550, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:49', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408567036055552, 2066408562711728128, '2066415154064658432,2066408562711728128', '一车间', 1084566691, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:49', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408567828779008, 2066408562711728128, '2066415154064658432,2066408562711728128', '二车间', 1084619690, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:49', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408570584436736, 2066408563538006016, '2066415154064658432,2066408563538006016', '仓储科', 1084978430, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:50', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO `sys_dept` VALUES (2066408574942318592, 2066408563538006016, '2066415154064658432,2066408563538006016', '采购科', 1084898355, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:51', NULL, '2026-06-15 20:29:02', NULL);
INSERT INTO `sys_dept` VALUES (2066415154064658432, 0, '0', '二狗子元宇宙养猪研讨会', 0, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:58:59', NULL, '2026-06-15 20:29:01', NULL);

-- ----------------------------
-- Table structure for sys_ai_model
-- ----------------------------
DROP TABLE IF EXISTS `sys_ai_model`;
CREATE TABLE `sys_ai_model`  (
  `id` bigint(20) NOT NULL,
  `provider` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `provider_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `model_id` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `display_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `api_key` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `base_url` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `is_default` tinyint(1) NOT NULL,
  `is_default_provider` tinyint(1) NOT NULL,
  `sort` int(11) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_ai_model
-- ----------------------------
INSERT INTO `sys_ai_model` VALUES (2075931753100480512, 'deepseek', 'DeepSeek', 'deepseek-chat', 'DeepSeek Chat', '', 'https://api.deepseek.com/v1', 1, 1, 1, '2026-07-11 21:14:34', '2026-07-11 21:14:34');
INSERT INTO `sys_ai_model` VALUES (2075931753113063424, 'deepseek', 'DeepSeek', 'deepseek-reasoner', 'DeepSeek Reasoner', '', 'https://api.deepseek.com/v1', 0, 0, 2, '2026-07-11 21:14:34', '2026-07-11 21:14:34');
INSERT INTO `sys_ai_model` VALUES (2075931753113063425, 'qwen', '通义千问', 'qwen-plus', '通义千问 Plus', '', 'https://dashscope.aliyuncs.com/compatible-mode/v1', 1, 0, 3, '2026-07-11 21:14:34', '2026-07-11 21:14:34');
INSERT INTO `sys_ai_model` VALUES (2075931753113063426, 'qwen', '通义千问', 'qwen-max', '通义千问 Max', '', 'https://dashscope.aliyuncs.com/compatible-mode/v1', 0, 0, 4, '2026-07-11 21:14:34', '2026-07-11 21:14:34');
INSERT INTO `sys_ai_model` VALUES (2075931753113063427, 'glm', '智谱 GLM', 'glm-4-flash', 'GLM-4-Flash', '', 'https://open.bigmodel.cn/api/paas/v4', 1, 0, 5, '2026-07-11 21:14:34', '2026-07-11 21:14:34');
INSERT INTO `sys_ai_model` VALUES (2075931753113063428, 'openai', 'OpenAI', 'gpt-4o-mini', 'GPT-4o Mini', '', 'https://api.openai.com/v1', 1, 0, 6, '2026-07-11 21:14:34', '2026-07-11 21:14:34');
INSERT INTO `sys_ai_model` VALUES (2075931753113063429, 'openai', 'OpenAI', 'gpt-4o', 'GPT-4o', '', 'https://api.openai.com/v1', 0, 0, 7, '2026-07-11 21:14:34', '2026-07-11 21:14:34');
INSERT INTO `sys_ai_model` VALUES (2075931753113063666, 'codebuddy', 'codebuddy', 'auto-chat', 'auto-chat', '123456', 'http://127.0.0.1:8001/codebuddy/v1', 1, 1, 8, '2026-07-22 00:00:00', '2026-07-22 00:00:00');
INSERT INTO `sys_ai_model` VALUES (2075931753113063888, 'qwen', '通义千问', 'qwen3.7-max-2026-05-20', 'qwen3.7-max', 'sk-ws-H.EDXXHDX.tw2v.MEYCIQD6mepbudQ5iAXMhzTCAxwh6pz4T_uCO0J-vRnom9xx0wIhANYc12MdfceccY8bnZMuU7hna0pdCSTCgqs4E1yHGnEo', 'https://ws-gz3rqoybjc6el9i6.cn-beijing.maas.aliyuncs.com/compatible-mode/v1', 1, 0, 8, '2026-07-22 00:00:00', '2026-07-22 00:00:00');

-- ----------------------------
-- Table structure for sys_code_rule
-- ----------------------------
DROP TABLE IF EXISTS `sys_code_rule`;
CREATE TABLE `sys_code_rule`  (
  `rule_id` bigint(20) NOT NULL COMMENT '规则id',
  `rule_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '规则编码',
  `rule_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '规则名称',
  `rule_type` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '规则类型',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '状态（0正常 1停用）',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`rule_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '序号生成规则' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_code_rule
-- ----------------------------
INSERT INTO `sys_code_rule` VALUES (1, 'wms-stock', '【仓库】库存编码', NULL, '0', 0, '', NULL, 'admin', '2022-10-05 23:31:41');
INSERT INTO `sys_code_rule` VALUES (2, 'wms-picking', '【仓库】拣货单编码', NULL, '0', 0, 'admin', '2022-07-25 01:55:09', 'admin', '2022-10-16 22:17:23');
INSERT INTO `sys_code_rule` VALUES (3, 'wms-material', '【仓库】领料单编码', NULL, '0', 0, 'admin', '2022-10-08 23:53:39', 'admin', '2022-10-16 22:17:07');
INSERT INTO `sys_code_rule` VALUES (4, 'wms-outbound', '【仓库】出库单编码', NULL, '0', 0, 'admin', '2022-10-17 00:15:52', 'admin', '2022-10-17 00:16:56');
INSERT INTO `sys_code_rule` VALUES (5, 'wms-shelve', '【仓库】上架单编码', NULL, '0', 0, 'admin', '2022-10-25 22:28:28', 'admin', '2022-10-31 00:11:25');
INSERT INTO `sys_code_rule` VALUES (6, 'wms-check', '【仓库】盘点编码', NULL, '0', 0, 'admin', '2022-10-31 00:10:34', '管理员122', '2024-11-17 09:07:21');
INSERT INTO `sys_code_rule` VALUES (7, 'wms-goods', '【仓库】货物编码', NULL, '0', 0, 'admin', '2022-11-03 21:19:32', 'admin', '2022-11-03 21:22:11');
INSERT INTO `sys_code_rule` VALUES (8, 'wms-delivery', '【仓库】入库单编码', NULL, '0', 0, 'admin', '2022-11-23 00:05:48', '管理员122', '2026-06-02 00:15:17');

-- ----------------------------
-- Table structure for sys_post
-- ----------------------------
DROP TABLE IF EXISTS `sys_post`;
CREATE TABLE `sys_post`  (
  `post_id` bigint(20) NOT NULL COMMENT '岗位ID',
  `post_code` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '岗位编码',
  `post_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '岗位名称',
  `post_sort` int(11) NOT NULL COMMENT '显示顺序',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '状态（0正常 1停用）',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`post_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '岗位信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_post
-- ----------------------------
INSERT INTO `sys_post` VALUES (1, 'ceo', '董事长', 1, '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2024-11-01 23:08:50', '');
INSERT INTO `sys_post` VALUES (2, 'se', '项目经理', 2, '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, '');
INSERT INTO `sys_post` VALUES (3, 'hr', '人力资源', 3, '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, '');
INSERT INTO `sys_post` VALUES (4, 'user', '普通员工', 4, '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, '');

-- ----------------------------
-- Table structure for sys_job
-- ----------------------------
DROP TABLE IF EXISTS `sys_job`;
CREATE TABLE `sys_job`  (
  `job_id` bigint(20) NOT NULL COMMENT '任务ID',
  `job_name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '' COMMENT '任务名称',
  `job_group` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'DEFAULT' COMMENT '任务组名',
  `invoke_target` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '调用目标字符串',
  `cron_expression` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT 'cron执行表达式',
  `misfire_policy` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '3' COMMENT '计划执行错误策略（1立即执行 2执行一次 3放弃执行）',
  `concurrent` char(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '1' COMMENT '是否并发执行（0允许 1禁止）',
  `status` char(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '0' COMMENT '状态（0正常 1暂停）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '备注信息',
  PRIMARY KEY (`job_id`, `job_name`, `job_group`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '定时任务调度表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_job
-- ----------------------------
INSERT INTO `sys_job` VALUES (1, '系统默认（无参）', 'DEFAULT', 'ryTask.ryNoParams', '0/10 * * * * ?', '3', '1', '1', 'admin', '2022-04-15 15:51:02', '', NULL, '');
INSERT INTO `sys_job` VALUES (2, '系统默认（有参）', 'DEFAULT', 'ryTask.ryParams(\'ry\')', '0/15 * * * * ?', '3', '1', '1', 'admin', '2022-04-15 15:51:02', '', NULL, '');
INSERT INTO `sys_job` VALUES (3, '系统默认（多参）', 'DEFAULT', 'ryTask.ryMultipleParams(\'ry\', true, 2000L, 316.50D, 100)', '0/20 * * * * ?', '3', '1', '1', 'admin', '2022-04-15 15:51:02', '', NULL, '');
INSERT INTO `sys_job` VALUES (2071956606450536448, 'DingTalkUserSync', 'SYSTEM', 'DingTalkUserSync', '0 0 0 * * ?', '3', '1', '1', 'SYSTEM', '2026-06-30 21:58:45', NULL, NULL, '系统自动发现');

-- ----------------------------
-- Table structure for flow_process
-- ----------------------------
DROP TABLE IF EXISTS `flow_process`;
CREATE TABLE `flow_process`  (
  `process_id` bigint(20) NOT NULL COMMENT '主键',
  `process_code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '流程编码',
  `process_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '流程名称',
  `parent_id` bigint(20) NOT NULL DEFAULT 0 COMMENT '父级id',
  `cur_version_id` bigint(20) NOT NULL COMMENT '当前流程图',
  `business_field` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '业务模块',
  `remark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `is_del` int(11) NOT NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `form_url` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '页面地址{id}占位符',
  `back_url` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '回调地址',
  PRIMARY KEY (`process_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '审批流' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of flow_process
-- ----------------------------
INSERT INTO `flow_process` VALUES (5, 'test', '测试', 0, 10, 'test', NULL, 0, 'admin', '2022-12-19 00:01:32', 'admin', '2023-08-10 22:55:40', NULL, NULL);
INSERT INTO `flow_process` VALUES (6, 'test2', '测试2', 0, 5, 'test', NULL, 0, 'admin', '2023-07-24 23:02:05', 'admin', '2026-04-30 17:14:26', NULL, NULL);
INSERT INTO `flow_process` VALUES (2053660312145629184, 'new-flow-01', '新流程测试', 0, 2053660312732831744, 'test', NULL, 0, 'admin', '2026-05-11 10:15:48', 'admin', '2026-07-18 10:12:58', NULL, NULL);
INSERT INTO `flow_process` VALUES (2078302135107325952, 'test002', 'cs', 0, 2078302135375761408, 'test', NULL, 0, 'admin', '2026-07-18 10:13:37', 'admin', '2026-07-18 10:13:58', NULL, NULL);

-- ----------------------------
-- Table structure for sys_user_role
-- ----------------------------
DROP TABLE IF EXISTS `sys_user_role`;
CREATE TABLE `sys_user_role`  (
  `user_id` bigint(20) NOT NULL COMMENT '用户ID',
  `role_id` bigint(20) NOT NULL COMMENT '角色ID',
  PRIMARY KEY (`user_id`, `role_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '用户和角色关联' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_user_role
-- ----------------------------
INSERT INTO `sys_user_role` VALUES (1, 1);
INSERT INTO `sys_user_role` VALUES (1, 2);
INSERT INTO `sys_user_role` VALUES (1, 12);

-- ----------------------------
-- Table structure for sys_data_service_node
-- ----------------------------
DROP TABLE IF EXISTS `sys_data_service_node`;
CREATE TABLE `sys_data_service_node`  (
  `ds_part_id` bigint(20) NOT NULL COMMENT '主键id',
  `ds_id` bigint(20) NULL DEFAULT NULL COMMENT '关联主表id',
  `part_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `var_type` int(1) NULL DEFAULT NULL COMMENT '变量类型',
  `var_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '变量名',
  `part_config` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '内容',
  `sort_by` int(11) NULL DEFAULT NULL COMMENT '排序字段',
  `part_type` int(11) NULL DEFAULT NULL COMMENT '类型 0 sql，1 js',
  `is_del` int(1) NULL DEFAULT NULL COMMENT '删除标志（0代表存在 1代表删除）',
  PRIMARY KEY (`ds_part_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '数据服务节点' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_data_service_node
-- ----------------------------
INSERT INTO `sys_data_service_node` VALUES (2044976888220356608, 1874451681413042176, '步骤1', 0, 'user', 'SELECT * FROM sys_user where user_id=#{user.id} and create_time > #{createTime}', 0, 0, 0);
INSERT INTO `sys_data_service_node` VALUES (2044978518391132160, 1874451681413042176, '步骤2', 0, 'step2', 'function handle(data){data.user = [...data.user,...data.user];data.vvv=1;return data;}', 2, 1, 0);
INSERT INTO `sys_data_service_node` VALUES (2078486297990795264, 1874451681413042176, '步骤3', 0, 'param3', 'SELECT\n    d.dept_id,\n    d.dept_name,\n    COUNT(u.user_id) AS user_count\nFROM sys_dept d\nLEFT JOIN sys_user u\n    ON u.dept_id = d.dept_id\n    AND (u.is_del IS NULL OR u.is_del = 0)\nWHERE (d.is_del = 0 OR d.is_del IS NULL)\n  AND d.dept_id = #{sys.deptId}\nGROUP BY d.dept_id, d.dept_name\nORDER BY d.dept_id', 1, 0, 0);

-- ----------------------------
-- Table structure for sys_config
-- ----------------------------
DROP TABLE IF EXISTS `sys_config`;
CREATE TABLE `sys_config`  (
  `config_id` bigint(20) NOT NULL COMMENT '参数主键',
  `config_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '参数名称',
  `config_key` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '参数键名',
  `config_value` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '参数键值',
  `config_type` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '系统内置（Y是 N否）',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `sort_by` int(11) NULL DEFAULT NULL COMMENT '排序字段',
  PRIMARY KEY (`config_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '参数配置' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_config
-- ----------------------------
INSERT INTO `sys_config` VALUES (10000010, '【系统】账号初始密码', 'sys.user.initPassword', '123456', 'Y', NULL, NULL, '1900-01-01 00:00:00', 'admin', '2026-07-02 01:35:31', '初始化密码 123456', NULL);
INSERT INTO `sys_config` VALUES (10000011, '【系统】登录验证码', 'sys.login.isCaptchaOn', '{\"enabled\":false}', 'Y', 0, NULL, '1900-01-01 00:00:00', 'admin', '2026-07-02 00:35:24', '是否开启验证码功能（true开启，false关闭）', 3);

-- ----------------------------
-- Table structure for sys_user_post
-- ----------------------------
DROP TABLE IF EXISTS `sys_user_post`;
CREATE TABLE `sys_user_post`  (
  `user_id` bigint(20) NOT NULL COMMENT '用户ID',
  `post_id` bigint(20) NOT NULL COMMENT '岗位ID',
  PRIMARY KEY (`user_id`, `post_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '用户与岗位关联' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_user_post
-- ----------------------------
INSERT INTO `sys_user_post` VALUES (1, 1);

-- ----------------------------
-- Table structure for sys_user
-- ----------------------------
DROP TABLE IF EXISTS `sys_user`;
CREATE TABLE `sys_user`  (
  `user_id` bigint(20) NOT NULL COMMENT '用户ID',
  `dept_id` bigint(20) NULL DEFAULT NULL COMMENT '部门ID',
  `account` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户账户',
  `user_name` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户名称',
  `user_type` varchar(2) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '00' COMMENT '用户类型（00系统用户）',
  `email` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '用户邮箱',
  `phonenumber` varchar(11) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '手机号码',
  `sex` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '用户性别（0男 1女 2未知）',
  `avatar` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '头像地址',
  `password` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '密码',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '帐号状态（0正常 1停用）',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `login_ip` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '最后登录IP',
  `login_date` datetime NULL DEFAULT NULL COMMENT '最后登录时间',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`user_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '用户信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_user
-- ----------------------------
INSERT INTO `sys_user` VALUES (1, 103, 'admin', '管理员', '00', 'wes@gmail.com', '15888888888', '1', '', '0gtW3BPaOmFNQ1ag2mt7llGYkp4MXSaPSdbzwUmPWVEMlxcELN7PmYBWzSH6tqxo', '0', 0, '127.0.0.1', '2022-04-17 15:33:03', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-04 00:33:02', '管理员');

-- ----------------------------
-- Table structure for sys_data_service
-- ----------------------------
DROP TABLE IF EXISTS `sys_data_service`;
CREATE TABLE `sys_data_service`  (
  `ds_id` bigint(20) NOT NULL COMMENT '主键id',
  `service_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '数据服务编码',
  `service_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '名称',
  `category` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '分类字典',
  `param_config` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '参数配置',
  `status` varchar(11) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '是否有效（0代表无效 1代表有效）',
  `remark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `is_del` int(1) NULL DEFAULT NULL COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`ds_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '数据服务' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_data_service
-- ----------------------------
INSERT INTO `sys_data_service` VALUES (1874451681413042176, 'aaa', 'test', 'builtin', '[{\"key\":\"createTime\",\"type\":\"date\"},{\"key\":\"user\",\"type\":\"object\"}]', '0', '', 0, 'admin', '2025-01-01 21:44:41', 'admin', '2026-07-18 22:47:41');

-- ----------------------------
-- Table structure for sys_ai_session
-- ----------------------------
DROP TABLE IF EXISTS `sys_ai_session`;
CREATE TABLE `sys_ai_session`  (
  `id` bigint(20) NOT NULL,
  `user_id` bigint(20) NOT NULL,
  `title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `agent_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `provider` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  `model` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_ai_session
-- ----------------------------
INSERT INTO `sys_ai_session` VALUES (2075497646277136384, 1, 'hello', 'sql_expert', 'qwen', '2026-07-10 16:29:34', '2026-07-18 12:40:45', 'qwen3.6-35b-a3b');

-- ----------------------------
-- Table structure for sys_token
-- ----------------------------
DROP TABLE IF EXISTS `sys_token`;
CREATE TABLE `sys_token`  (
  `token_id` bigint(20) NOT NULL COMMENT '主键自增id',
  `user_id` bigint(20) NOT NULL COMMENT '用户id',
  `token` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '登录token',
  `status` int(11) NOT NULL COMMENT '状态（0正常 1停用 2过期）',
  `login_ip` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '生成Token的IP',
  `login_location` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '登录地点',
  `browser` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '浏览器类型',
  `os` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '操作系统',
  `is_del` int(11) NOT NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `expiration_time` datetime NOT NULL COMMENT '过期时间',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `source` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'web' COMMENT '来源 web app',
  PRIMARY KEY (`token_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '登录token' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_token
-- ----------------------------

-- ----------------------------
-- Table structure for sys_third_party_mapping
-- ----------------------------
DROP TABLE IF EXISTS `sys_third_party_mapping`;
CREATE TABLE `sys_third_party_mapping`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `provider` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `entity_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `third_party_id` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `local_id` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `display_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `create_time` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 17 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '三方平台映射' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_third_party_mapping
-- ----------------------------

-- ----------------------------
-- Table structure for sys_role_menu
-- ----------------------------
DROP TABLE IF EXISTS `sys_role_menu`;
CREATE TABLE `sys_role_menu`  (
  `role_id` bigint(20) NOT NULL COMMENT '角色ID',
  `menu_id` bigint(20) NOT NULL COMMENT '菜单ID',
  PRIMARY KEY (`role_id`, `menu_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '角色和菜单关联' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_role_menu
-- ----------------------------

-- ----------------------------
-- Table structure for sys_role_dept
-- ----------------------------
DROP TABLE IF EXISTS `sys_role_dept`;
CREATE TABLE `sys_role_dept`  (
  `role_id` bigint(20) NOT NULL COMMENT '角色ID',
  `dept_id` bigint(20) NOT NULL COMMENT '部门ID',
  PRIMARY KEY (`role_id`, `dept_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '角色和部门关联' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_role_dept
-- ----------------------------

-- ----------------------------
-- Table structure for sys_oper_log
-- ----------------------------
DROP TABLE IF EXISTS `sys_oper_log`;
CREATE TABLE `sys_oper_log`  (
  `oper_id` bigint(20) NOT NULL COMMENT '日志主键',
  `title` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '模块标题',
  `business_type` int(11) NULL DEFAULT 0 COMMENT '业务类型（0其它 1新增 2修改 3删除）',
  `method` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '方法名称',
  `request_method` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '请求方式',
  `operator_type` int(11) NULL DEFAULT 0 COMMENT '操作类别（0其它 1后台用户 2手机端用户）',
  `oper_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '操作人员',
  `dept_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '部门名称',
  `oper_url` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '请求URL',
  `oper_ip` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '主机地址',
  `oper_location` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '操作地点',
  `oper_param` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '请求参数',
  `json_result` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '返回参数',
  `status` int(11) NULL DEFAULT 0 COMMENT '操作状态（0正常 1异常）',
  `error_msg` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '错误消息',
  `oper_time` datetime NULL DEFAULT NULL COMMENT '操作时间',
  `cost_time` int(11) NULL DEFAULT NULL COMMENT '消耗时间',
  PRIMARY KEY (`oper_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '操作日志记录' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_oper_log
-- ----------------------------

-- ----------------------------
-- Table structure for sys_message
-- ----------------------------
DROP TABLE IF EXISTS `sys_message`;
CREATE TABLE `sys_message`  (
  `message_id` bigint(20) NOT NULL COMMENT '主键id',
  `message_type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '消息类型',
  `open_type` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '对话框dialog，内部in，外部out',
  `message_title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '消息标题',
  `message_body` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '消息内容',
  `send_user_id` bigint(20) NOT NULL COMMENT '发送人',
  `user_id` bigint(20) NOT NULL COMMENT '接收人',
  `is_read` int(1) NOT NULL COMMENT '是否已读',
  `is_del` int(11) NOT NULL COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  `read_time` datetime NULL DEFAULT NULL COMMENT '阅读时间',
  PRIMARY KEY (`message_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '站内信' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_message
-- ----------------------------

-- ----------------------------
-- Table structure for sys_login_log
-- ----------------------------
DROP TABLE IF EXISTS `sys_login_log`;
CREATE TABLE `sys_login_log`  (
  `login_id` bigint(20) NOT NULL COMMENT '访问ID',
  `user_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '用户账号',
  `ipaddr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '登录IP地址',
  `login_location` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '登录地点',
  `browser` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '浏览器类型',
  `os` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '操作系统',
  `status` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '登录状态（0成功 1失败）',
  `msg` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '提示消息',
  `login_time` datetime NULL DEFAULT NULL COMMENT '访问时间',
  `is_del` int(11) NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  PRIMARY KEY (`login_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '系统访问记录' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_login_log
-- ----------------------------

-- ----------------------------
-- Table structure for sys_job_log
-- ----------------------------
DROP TABLE IF EXISTS `sys_job_log`;
CREATE TABLE `sys_job_log`  (
  `job_log_id` bigint(20) NOT NULL COMMENT '任务日志ID',
  `job_name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '任务名称',
  `job_group` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '任务组名',
  `invoke_target` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '调用目标字符串',
  `job_message` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '日志信息',
  `status` char(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '0' COMMENT '执行状态（0正常 1失败）',
  `exception_info` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '异常信息',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `elapsed_time` bigint(10) NULL DEFAULT NULL COMMENT '执行耗时（毫秒）',
  PRIMARY KEY (`job_log_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '定时任务调度日志表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_job_log
-- ----------------------------

-- ----------------------------
-- Table structure for sys_file
-- ----------------------------
DROP TABLE IF EXISTS `sys_file`;
CREATE TABLE `sys_file`  (
  `file_id` bigint(20) NOT NULL COMMENT '文件id',
  `file_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '文件名',
  `file_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '文件类型',
  `file_size` bigint(20) NOT NULL COMMENT '文件大小',
  `file_path` varchar(600) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '文件路径',
  `table_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '所属业务表',
  `table_id` bigint(20) NULL DEFAULT NULL COMMENT '所属业务表id',
  `is_del` int(11) NOT NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  PRIMARY KEY (`file_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '文件' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_file
-- ----------------------------

-- ----------------------------
-- Table structure for sys_ai_message
-- ----------------------------
DROP TABLE IF EXISTS `sys_ai_message`;
CREATE TABLE `sys_ai_message`  (
  `id` bigint(20) NOT NULL,
  `session_id` bigint(20) NOT NULL,
  `user_id` bigint(20) NOT NULL,
  `role` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `tool_calls` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `sort` int(11) NOT NULL,
  `created_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_ai_message
-- ----------------------------

-- ----------------------------
-- Table structure for flow_instance_task
-- ----------------------------
DROP TABLE IF EXISTS `flow_instance_task`;
CREATE TABLE `flow_instance_task`  (
  `instance_task_id` bigint(20) NOT NULL COMMENT '主键id',
  `instance_id` bigint(20) NOT NULL COMMENT '实例id',
  `instance_node_id` bigint(20) NOT NULL COMMENT '节点id',
  `task_user_id` bigint(20) NOT NULL COMMENT '任务处理人',
  `actual_user_id` bigint(20) NOT NULL COMMENT '实际处理人',
  `task_result` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '处理结果 pass通过  unpass不通过  pending挂起  delegate委托',
  `comments` varchar(800) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '审批意见',
  `is_recall` int(11) NOT NULL DEFAULT 0 COMMENT '是否抓回 1抓回',
  `recall_task_id` bigint(20) NULL DEFAULT NULL COMMENT '委托人员（用于抓回）',
  `is_del` int(11) NOT NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `handle_time` datetime NULL DEFAULT NULL COMMENT '处理时间',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  PRIMARY KEY (`instance_task_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '节点任务' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of flow_instance_task
-- ----------------------------

-- ----------------------------
-- Table structure for flow_instance_node
-- ----------------------------
DROP TABLE IF EXISTS `flow_instance_node`;
CREATE TABLE `flow_instance_node`  (
  `instance_node_id` bigint(20) NOT NULL COMMENT '主键id',
  `instance_id` bigint(20) NOT NULL COMMENT '实例id',
  `node_id` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `node_type` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '节点类型 start end task notice',
  `pre_node_id` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '前一节点id',
  `node_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '节点名称',
  `node_result` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '处理结果 pass通过  unpass不通过  pending挂起',
  `is_del` int(11) NOT NULL DEFAULT 0 COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT '' COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`instance_node_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '实例节点' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of flow_instance_node
-- ----------------------------

-- ----------------------------
-- Table structure for flow_instance
-- ----------------------------
DROP TABLE IF EXISTS `flow_instance`;
CREATE TABLE `flow_instance`  (
  `instance_id` bigint(20) NOT NULL COMMENT '主键',
  `process_id` bigint(20) NOT NULL COMMENT '流程id',
  `version_id` bigint(20) NOT NULL COMMENT '流程版本',
  `business_id` bigint(20) NOT NULL COMMENT '业务主键id',
  `business_code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '业务编码',
  `current_node_id` bigint(20) NULL DEFAULT NULL COMMENT '当前节点',
  `is_urgent` int(11) NOT NULL COMMENT '是否加急 1 加急 0整除',
  `instance_status` bigint(20) NOT NULL COMMENT '审批状态',
  `extend_info` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '扩展信息',
  `remark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '描述',
  `is_del` int(11) NOT NULL COMMENT '删除标志（0代表存在 1代表删除）',
  `create_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '创建者',
  `create_time` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '更新者',
  `update_time` datetime NULL DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`instance_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '审批流实例' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of flow_instance
-- ----------------------------

SET FOREIGN_KEY_CHECKS = 1;
