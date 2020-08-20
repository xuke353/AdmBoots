using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Adm.Boot.Data.Migrations
{
    public partial class AdmBoot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    LastLoginTime = table.Column<DateTime>(nullable: true),
                    IsMaster = table.Column<bool>(nullable: false),
                    UnreadCount = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
