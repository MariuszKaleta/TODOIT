using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TODOIT.Migrations
{
    public partial class HeadSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeadSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    SkillName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeadSkills_Skills_SkillName",
                        column: x => x.SkillName,
                        principalTable: "Skills",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeadSkills_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeadSkills_SkillName",
                table: "HeadSkills",
                column: "SkillName");

            migrationBuilder.CreateIndex(
                name: "IX_HeadSkills_UserId",
                table: "HeadSkills",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeadSkills");
        }
    }
}
