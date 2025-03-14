using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Correctthefriendsentityforaddingrequestinitiatorandreceiver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_User1Id",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_User2Id",
                table: "UserFriends");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "UserFriends",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "User1Id",
                table: "UserFriends",
                newName: "InitiatorId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFriends_User2Id",
                table: "UserFriends",
                newName: "IX_UserFriends_ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_InitiatorId",
                table: "UserFriends",
                column: "InitiatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_ReceiverId",
                table: "UserFriends",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_InitiatorId",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_ReceiverId",
                table: "UserFriends");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "UserFriends",
                newName: "User2Id");

            migrationBuilder.RenameColumn(
                name: "InitiatorId",
                table: "UserFriends",
                newName: "User1Id");

            migrationBuilder.RenameIndex(
                name: "IX_UserFriends_ReceiverId",
                table: "UserFriends",
                newName: "IX_UserFriends_User2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_User1Id",
                table: "UserFriends",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_User2Id",
                table: "UserFriends",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
