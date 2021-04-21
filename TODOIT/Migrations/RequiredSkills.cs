using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TODOIT.Migrations
{
    public partial class RequiredSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsedSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    SkillName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsedSkills_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsedSkills_Skills_SkillName",
                        column: x => x.SkillName,
                        principalTable: "Skills",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsedSkills_OrderId",
                table: "UsedSkills",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UsedSkills_SkillName",
                table: "UsedSkills",
                column: "SkillName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedSkills");
        }
    }
}
