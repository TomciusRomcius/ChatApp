using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Application.Migrations
{
    /// <inheritdoc />
    public partial class renamemessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_AspNetUsers_ReceiverUserId",
                table: "UserMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_ChatRooms_ChatRoomId",
                table: "UserMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_TextMessages_TextMessageId",
                table: "UserMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMessages",
                table: "UserMessages");

            migrationBuilder.RenameTable(
                name: "UserMessages",
                newName: "Messages");

            migrationBuilder.RenameIndex(
                name: "IX_UserMessages_ReceiverUserId",
                table: "Messages",
                newName: "IX_Messages_ReceiverUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMessages_ChatRoomId",
                table: "Messages",
                newName: "IX_Messages_ChatRoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "TextMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverUserId",
                table: "Messages",
                column: "ReceiverUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatRooms_ChatRoomId",
                table: "Messages",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "ChatRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_TextMessages_TextMessageId",
                table: "Messages",
                column: "TextMessageId",
                principalTable: "TextMessages",
                principalColumn: "TextMessageId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverUserId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatRooms_ChatRoomId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_TextMessages_TextMessageId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "UserMessages");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ReceiverUserId",
                table: "UserMessages",
                newName: "IX_UserMessages_ReceiverUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChatRoomId",
                table: "UserMessages",
                newName: "IX_UserMessages_ChatRoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMessages",
                table: "UserMessages",
                column: "TextMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_AspNetUsers_ReceiverUserId",
                table: "UserMessages",
                column: "ReceiverUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_ChatRooms_ChatRoomId",
                table: "UserMessages",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "ChatRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_TextMessages_TextMessageId",
                table: "UserMessages",
                column: "TextMessageId",
                principalTable: "TextMessages",
                principalColumn: "TextMessageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
