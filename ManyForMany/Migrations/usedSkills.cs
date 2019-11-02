using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TODOIT.Migrations
{
    public partial class usedSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsedSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: true),
                    SkillName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsedSkills_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsedSkills_Skills_SkillName",
                        column: x => x.SkillName,
                        principalTable: "Skills",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
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
