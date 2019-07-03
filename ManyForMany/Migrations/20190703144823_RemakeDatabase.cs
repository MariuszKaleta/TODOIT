using Microsoft.EntityFrameworkCore.Migrations;

namespace ManyForMany.Migrations
{
    public partial class RemakeDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId2",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId3",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ApplicationUserId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ApplicationUserId2",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ApplicationUserId3",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId2",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId3",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "OrderId2",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrderId2",
                table: "AspNetUsers",
                column: "OrderId2");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Orders_OrderId2",
                table: "AspNetUsers",
                column: "OrderId2",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Orders_OrderId2",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrderId2",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrderId2",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId2",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId3",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId1",
                table: "Orders",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId2",
                table: "Orders",
                column: "ApplicationUserId2");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId3",
                table: "Orders",
                column: "ApplicationUserId3");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId1",
                table: "Orders",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId2",
                table: "Orders",
                column: "ApplicationUserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId3",
                table: "Orders",
                column: "ApplicationUserId3",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
