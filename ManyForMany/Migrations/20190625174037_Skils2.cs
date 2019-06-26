using Microsoft.EntityFrameworkCore.Migrations;

namespace ManyForMany.Migrations
{
    public partial class Skils2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Skills",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "Skills",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_OrderId",
                table: "Skills",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_OrderId1",
                table: "Skills",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Orders_OrderId",
                table: "Skills",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Orders_OrderId1",
                table: "Skills",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Orders_OrderId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Orders_OrderId1",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_OrderId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_OrderId1",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "Skills");
        }
    }
}
