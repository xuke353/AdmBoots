using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdmBoots.Data.Migrations
{
    public partial class Initdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BeginTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    JobName = table.Column<string>(maxLength: 50, nullable: false),
                    Seconds = table.Column<double>(nullable: false),
                    Level = table.Column<string>(nullable: true),
                    Result = table.Column<string>(nullable: true),
                    ErrorMsg = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailSetting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    To = table.Column<string>(maxLength: 2000, nullable: false),
                    FrHost = table.Column<string>(maxLength: 50, nullable: false),
                    Fr = table.Column<string>(maxLength: 50, nullable: false),
                    Cc = table.Column<string>(maxLength: 2000, nullable: true),
                    Notify = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "menu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatorId = table.Column<int>(nullable: false),
                    CreatorName = table.Column<string>(maxLength: 100, nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ModifierId = table.Column<int>(nullable: true),
                    ModifierName = table.Column<string>(maxLength: 100, nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Icon = table.Column<string>(maxLength: 50, nullable: true),
                    Uri = table.Column<string>(maxLength: 50, nullable: false),
                    MenuType = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Sort = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatorId = table.Column<int>(nullable: false),
                    CreatorName = table.Column<string>(maxLength: 100, nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    ModifierId = table.Column<int>(nullable: true),
                    ModifierName = table.Column<string>(maxLength: 100, nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    LastLoginTime = table.Column<DateTime>(nullable: true),
                    IsMaster = table.Column<bool>(nullable: false, defaultValue: false),
                    Email = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_menu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    MenuId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_menu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_menu_menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_menu_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_role_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_role_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "menu",
                columns: new[] { "Id", "Code", "CreateTime", "CreatorId", "CreatorName", "Description", "Icon", "IsActive", "MenuType", "ModifierId", "ModifierName", "ModifyTime", "Name", "ParentId", "Sort", "Status", "Uri" },
                values: new object[,]
                {
                    { 1, "yibp", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3758), 1, "管理员", "菜单的Uri为路由地址", "AreaChartOutlined", true, 1, null, null, null, "仪表盘", -1, 0, 1, "/dashboard" },
                    { 13, "auth", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3929), 1, "管理员", "编号是前端判断权限的key", null, true, 2, null, null, null, "权限", 4, 4, 1, "Role:UpdateRoleMenu" },
                    { 12, "delete", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3925), 1, "管理员", "编号是前端判断权限的key", null, true, 2, null, null, null, "删除", 4, 3, 1, "Role:Delete" },
                    { 11, "update", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3921), 1, "管理员", "编号是前端判断权限的key", null, true, 2, null, null, null, "修改", 4, 2, 1, "Role:Update" },
                    { 10, "query", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3917), 1, "管理员", "编号是前端判断权限的key", null, true, 2, null, null, null, "查询", 4, 1, 1, "Role:Query" },
                    { 8, "youxsz", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3910), 1, "管理员", "菜单的Uri为路由地址", "MailOutlined", true, 1, null, null, null, "邮箱设置", 3, 2, 1, "/mailSetting" },
                    { 9, "add", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3914), 1, "管理员", "编号是前端判断权限的key", null, true, 2, null, null, null, "添加", 4, 0, 1, "Role:Add" },
                    { 6, "yonghugl", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3902), 1, "管理员", "菜单的Uri为路由地址", "UserSwitchOutlined", true, 1, null, null, null, "用户管理", 2, 3, 1, "/user" },
                    { 5, "caidgl", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3898), 1, "管理员", "菜单的Uri为路由地址", "MenuOutlined", true, 1, null, null, null, "菜单管理", 2, 2, 1, "/menu" },
                    { 4, "juesgl", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3894), 1, "管理员", "菜单的Uri为路由地址", "ClusterOutlined", true, 1, null, null, null, "角色管理", 2, 1, 1, "/role" },
                    { 3, "zuoydd", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3889), 1, "管理员", "菜单的Uri为路由地址", "ScheduleOutlined", true, 1, null, null, null, "任务调度", -1, 2, 1, "/schedule" },
                    { 2, "xitgl", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3884), 1, "管理员", "菜单的Uri为路由地址", "ClusterOutlined", true, 1, null, null, null, "系统管理", -1, 1, 1, "/system" },
                    { 7, "renwlb", new DateTime(2020, 9, 15, 22, 35, 38, 7, DateTimeKind.Local).AddTicks(3906), 1, "管理员", "菜单的Uri为路由地址", "OrderedListOutlined", true, 1, null, null, null, "任务列表", 3, 1, 1, "/job" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "Id", "Code", "CreateTime", "CreatorId", "CreatorName", "Description", "ModifierId", "ModifierName", "ModifyTime", "Name", "Status" },
                values: new object[,]
                {
                    { 1, "xtgly", new DateTime(2020, 9, 15, 22, 35, 38, 6, DateTimeKind.Local).AddTicks(7227), 1, "管理员", "拥有最高权限", null, null, null, "系统管理员", 1 },
                    { 2, "zmr", new DateTime(2020, 9, 15, 22, 35, 38, 6, DateTimeKind.Local).AddTicks(8939), 1, "管理员", "拥有最高权限", null, null, null, "掌门人", 1 }
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "Id", "CreateTime", "Email", "LastLoginTime", "Name", "Password", "Status", "UserName" },
                values: new object[] { 2, new DateTime(2020, 9, 15, 22, 35, 38, 4, DateTimeKind.Local).AddTicks(6976), null, null, "张无忌", "E10ADC3949BA59ABBE56E057F20F883E", 1, "zhangwj" });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "Id", "CreateTime", "Email", "IsMaster", "LastLoginTime", "Name", "Password", "Status", "UserName" },
                values: new object[] { 1, new DateTime(2020, 9, 15, 22, 35, 38, 2, DateTimeKind.Local).AddTicks(3165), null, true, null, "管理员", "DC483E80A7A0BD9EF71D8CF973673924", 1, "admin" });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "Id", "CreateTime", "Email", "LastLoginTime", "Name", "Password", "Status", "UserName" },
                values: new object[] { 3, new DateTime(2020, 9, 15, 22, 35, 38, 4, DateTimeKind.Local).AddTicks(7032), null, null, "周芷若", "E10ADC3949BA59ABBE56E057F20F883E", 1, "zhouzr" });

            migrationBuilder.InsertData(
                table: "role_menu",
                columns: new[] { "Id", "MenuId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 14, 1, 2 },
                    { 13, 13, 1 },
                    { 12, 12, 1 },
                    { 11, 11, 1 },
                    { 10, 10, 1 },
                    { 8, 8, 1 },
                    { 9, 9, 1 },
                    { 6, 6, 1 },
                    { 5, 5, 1 },
                    { 4, 4, 1 },
                    { 3, 3, 1 },
                    { 2, 2, 1 },
                    { 7, 7, 1 }
                });

            migrationBuilder.InsertData(
                table: "user_role",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 2, 2, 2 },
                    { 1, 1, 1 },
                    { 3, 2, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_menu_MenuId",
                table: "role_menu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_role_menu_RoleId",
                table: "role_menu",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_RoleId",
                table: "user_role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_UserId",
                table: "user_role",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobLog");

            migrationBuilder.DropTable(
                name: "MailSetting");

            migrationBuilder.DropTable(
                name: "role_menu");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "menu");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
