using Microsoft.EntityFrameworkCore.Migrations;

namespace ManyForMany.Migrations
{
    public partial class remakeUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Orders_OrderId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrderId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrderId1",
                table: "AspNetUsers",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Orders_OrderId1",
                table: "AspNetUsers",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
