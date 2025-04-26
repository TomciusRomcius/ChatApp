using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Application.Migrations
{
    /// <inheritdoc />
    public partial class Cascadingondeletewithmostlyclientcascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomMembers_AspNetUsers_MemberId",
                table: "ChatRoomMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomMembers_ChatRooms_ChatRoomId",
                table: "ChatRoomMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_AspNetUsers_AdminUserId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_TextMessages_AspNetUsers_SenderId",
                table: "TextMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_InitiatorId",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_ReceiverId",
                table: "UserFriends");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomMembers_AspNetUsers_MemberId",
                table: "ChatRoomMembers",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomMembers_ChatRooms_ChatRoomId",
                table: "ChatRoomMembers",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "ChatRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_AspNetUsers_AdminUserId",
                table: "ChatRooms",
                column: "AdminUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TextMessages_AspNetUsers_SenderId",
                table: "TextMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_InitiatorId",
                table: "UserFriends",
                column: "InitiatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_ReceiverId",
                table: "UserFriends",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomMembers_AspNetUsers_MemberId",
                table: "ChatRoomMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomMembers_ChatRooms_ChatRoomId",
                table: "ChatRoomMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_AspNetUsers_AdminUserId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_TextMessages_AspNetUsers_SenderId",
                table: "TextMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_InitiatorId",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_ReceiverId",
                table: "UserFriends");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomMembers_AspNetUsers_MemberId",
                table: "ChatRoomMembers",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomMembers_ChatRooms_ChatRoomId",
                table: "ChatRoomMembers",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "ChatRoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_AspNetUsers_AdminUserId",
                table: "ChatRooms",
                column: "AdminUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TextMessages_AspNetUsers_SenderId",
                table: "TextMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
