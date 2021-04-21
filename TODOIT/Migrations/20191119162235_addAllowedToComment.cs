using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TODOIT.Migrations
{
    public partial class addAllowedToComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllowToMakeOpines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowToMakeOpines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllowToMakeOpines_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_AllowToMakeOpines_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllowToMakeOpines_OrderId",
                table: "AllowToMakeOpines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AllowToMakeOpines_UserId",
                table: "AllowToMakeOpines",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowToMakeOpines");
        }
    }
}
