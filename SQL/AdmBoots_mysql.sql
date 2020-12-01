CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;

CREATE TABLE `AuditLog` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NULL,
    `ServiceName` varchar(250) CHARACTER SET utf8mb4 NULL,
    `MethodName` varchar(250) CHARACTER SET utf8mb4 NULL,
    `Parameters` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `ReturnValue` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `ExecutionTime` datetime(6) NOT NULL,
    `ExecutionDuration` int NOT NULL,
    `ClientIpAddress` varchar(50) CHARACTER SET utf8mb4 NULL,
    `ClientName` varchar(100) CHARACTER SET utf8mb4 NULL,
    `BrowserInfo` varchar(250) CHARACTER SET utf8mb4 NULL,
    `Exception` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `CustomData` varchar(2000) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AuditLog` PRIMARY KEY (`Id`)
);

CREATE TABLE `JobLog` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `BeginTime` datetime(6) NULL,
    `EndTime` datetime(6) NULL,
    `JobName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Seconds` double NOT NULL,
    `Level` longtext CHARACTER SET utf8mb4 NULL,
    `Result` longtext CHARACTER SET utf8mb4 NULL,
    `ErrorMsg` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_JobLog` PRIMARY KEY (`Id`)
);

CREATE TABLE `MailSetting` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Code` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `To` varchar(2000) CHARACTER SET utf8mb4 NOT NULL,
    `FrHost` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Fr` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Cc` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `Notify` tinyint(1) NOT NULL,
    CONSTRAINT `PK_MailSetting` PRIMARY KEY (`Id`)
);

CREATE TABLE `menu` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Code` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Icon` varchar(50) CHARACTER SET utf8mb4 NULL,
    `Uri` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `MenuType` int NOT NULL,
    `ParentId` int NULL,
    `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
    `IsActive` tinyint(1) NOT NULL,
    `Status` int NOT NULL,
    `Sort` int NOT NULL,
    `CreatorId` int NOT NULL,
    `CreatorName` varchar(100) CHARACTER SET utf8mb4 NULL,
    `CreateTime` datetime(6) NULL,
    `ModifierId` int NULL,
    `ModifierName` varchar(100) CHARACTER SET utf8mb4 NULL,
    `ModifyTime` datetime(6) NULL,
    CONSTRAINT `PK_menu` PRIMARY KEY (`Id`)
);

CREATE TABLE `role` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Code` varchar(50) CHARACTER SET utf8mb4 NULL,
    `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
    `Status` int NOT NULL,
    `CreatorId` int NOT NULL,
    `CreatorName` varchar(100) CHARACTER SET utf8mb4 NULL,
    `CreateTime` datetime(6) NULL,
    `ModifierId` int NULL,
    `ModifierName` varchar(100) CHARACTER SET utf8mb4 NULL,
    `ModifyTime` datetime(6) NULL,
    CONSTRAINT `PK_role` PRIMARY KEY (`Id`)
);

CREATE TABLE `user` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Password` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Status` int NOT NULL,
    `CreateTime` datetime(6) NOT NULL,
    `LastLoginTime` datetime(6) NULL,
    `IsMaster` tinyint(1) NOT NULL DEFAULT FALSE,
    `Email` varchar(50) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_user` PRIMARY KEY (`Id`)
);

CREATE TABLE `role_menu` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `RoleId` int NOT NULL,
    `MenuId` int NOT NULL,
    CONSTRAINT `PK_role_menu` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_role_menu_menu_MenuId` FOREIGN KEY (`MenuId`) REFERENCES `menu` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_role_menu_role_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `role` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `user_role` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `RoleId` int NOT NULL,
    CONSTRAINT `PK_user_role` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_user_role_role_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `role` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_user_role_user_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE
);

INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (1, 'yibp', '2020-12-01 11:18:31', 1, '管理员', '菜单的Uri为路由地址', 'AreaChartOutlined', TRUE, 1, NULL, NULL, NULL, '仪表盘', -1, 0, 1, '/dashboard');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (13, 'auth', '2020-12-01 11:18:31', 1, '管理员', '编号是前端判断权限的key', NULL, TRUE, 2, NULL, NULL, NULL, '权限', 4, 4, 1, 'Role:UpdateRoleMenu');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (12, 'delete', '2020-12-01 11:18:31', 1, '管理员', '编号是前端判断权限的key', NULL, TRUE, 2, NULL, NULL, NULL, '删除', 4, 3, 1, 'Role:Delete');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (11, 'update', '2020-12-01 11:18:31', 1, '管理员', '编号是前端判断权限的key', NULL, TRUE, 2, NULL, NULL, NULL, '修改', 4, 2, 1, 'Role:Update');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (10, 'query', '2020-12-01 11:18:31', 1, '管理员', '编号是前端判断权限的key', NULL, TRUE, 2, NULL, NULL, NULL, '查询', 4, 1, 1, 'Role:Query');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (8, 'youxsz', '2020-12-01 11:18:31', 1, '管理员', '菜单的Uri为路由地址', 'MailOutlined', TRUE, 1, NULL, NULL, NULL, '邮箱设置', 3, 2, 1, '/mailSetting');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (9, 'add', '2020-12-01 11:18:31', 1, '管理员', '编号是前端判断权限的key', NULL, TRUE, 2, NULL, NULL, NULL, '添加', 4, 0, 1, 'Role:Add');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (6, 'yonghugl', '2020-12-01 11:18:31', 1, '管理员', '菜单的Uri为路由地址', 'UserSwitchOutlined', TRUE, 1, NULL, NULL, NULL, '用户管理', 2, 3, 1, '/user');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (5, 'caidgl', '2020-12-01 11:18:31', 1, '管理员', '菜单的Uri为路由地址', 'MenuOutlined', TRUE, 1, NULL, NULL, NULL, '菜单管理', 2, 2, 1, '/menu');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (4, 'juesgl', '2020-12-01 11:18:31', 1, '管理员', '菜单的Uri为路由地址', 'ClusterOutlined', TRUE, 1, NULL, NULL, NULL, '角色管理', 2, 1, 1, '/role');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (3, 'zuoydd', '2020-12-01 11:18:31', 1, '管理员', '菜单的Uri为路由地址', 'ScheduleOutlined', TRUE, 1, NULL, NULL, NULL, '任务调度', -1, 2, 1, '/schedule');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (2, 'xitgl', '2020-12-01 11:18:31', 1, '管理员', '菜单的Uri为路由地址', 'ClusterOutlined', TRUE, 1, NULL, NULL, NULL, '系统管理', -1, 1, 1, '/system');
INSERT INTO `menu` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `Icon`, `IsActive`, `MenuType`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `ParentId`, `Sort`, `Status`, `Uri`)
VALUES (7, 'renwlb', '2020-12-01 11:18:31', 1, '管理员', '菜单的Uri为路由地址', 'OrderedListOutlined', TRUE, 1, NULL, NULL, NULL, '任务列表', 3, 1, 1, '/job');

INSERT INTO `role` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `Status`)
VALUES (1, 'xtgly', '2020-12-01 11:18:31', 1, '管理员', '拥有最高权限', NULL, NULL, NULL, '系统管理员', 1);
INSERT INTO `role` (`Id`, `Code`, `CreateTime`, `CreatorId`, `CreatorName`, `Description`, `ModifierId`, `ModifierName`, `ModifyTime`, `Name`, `Status`)
VALUES (2, 'zmr', '2020-12-01 11:18:31', 1, '管理员', NULL, NULL, NULL, NULL, '掌门人', 1);

INSERT INTO `user` (`Id`, `CreateTime`, `Email`, `LastLoginTime`, `Name`, `Password`, `Status`, `UserName`)
VALUES (2, '2020-12-01 11:18:31', NULL, NULL, '张无忌', 'E10ADC3949BA59ABBE56E057F20F883E', 1, 'zhangwj');

INSERT INTO `user` (`Id`, `CreateTime`, `Email`, `IsMaster`, `LastLoginTime`, `Name`, `Password`, `Status`, `UserName`)
VALUES (1, '2020-12-01 11:18:31', NULL, TRUE, NULL, '管理员', 'DC483E80A7A0BD9EF71D8CF973673924', 1, 'admin');

INSERT INTO `user` (`Id`, `CreateTime`, `Email`, `LastLoginTime`, `Name`, `Password`, `Status`, `UserName`)
VALUES (3, '2020-12-01 11:18:31', NULL, NULL, '周芷若', 'E10ADC3949BA59ABBE56E057F20F883E', 1, 'zhouzr');

INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (1, 1, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (14, 1, 2);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (13, 13, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (12, 12, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (11, 11, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (10, 10, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (8, 8, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (9, 9, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (6, 6, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (5, 5, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (4, 4, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (3, 3, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (2, 2, 1);
INSERT INTO `role_menu` (`Id`, `MenuId`, `RoleId`)
VALUES (7, 7, 1);

INSERT INTO `user_role` (`Id`, `RoleId`, `UserId`)
VALUES (2, 2, 2);
INSERT INTO `user_role` (`Id`, `RoleId`, `UserId`)
VALUES (1, 1, 1);
INSERT INTO `user_role` (`Id`, `RoleId`, `UserId`)
VALUES (3, 2, 3);

CREATE INDEX `IX_role_menu_MenuId` ON `role_menu` (`MenuId`);

CREATE INDEX `IX_role_menu_RoleId` ON `role_menu` (`RoleId`);

CREATE INDEX `IX_user_role_RoleId` ON `user_role` (`RoleId`);

CREATE INDEX `IX_user_role_UserId` ON `user_role` (`UserId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201201031832_initDb', '5.0.0');

COMMIT;

