using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Server.Application.Migrations
{
    /// <inheritdoc />
    public partial class Simplifymessagesstructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "ChatRoomId",
                table: "TextMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TextMessages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserId",
                table: "TextMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "TextMessages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_ChatRoomId",
                table: "TextMessages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_ReceiverUserId",
                table: "TextMessages",
                column: "ReceiverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_SenderId",
                table: "TextMessages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_TextMessages_AspNetUsers_ReceiverUserId",
                table: "TextMessages",
                column: "ReceiverUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TextMessages_AspNetUsers_SenderId",
                table: "TextMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TextMessages_ChatRooms_ChatRoomId",
                table: "TextMessages",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "ChatRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TextMessages_AspNetUsers_ReceiverUserId",
                table: "TextMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_TextMessages_AspNetUsers_SenderId",
                table: "TextMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_TextMessages_ChatRooms_ChatRoomId",
                table: "TextMessages");

            migrationBuilder.DropIndex(
                name: "IX_TextMessages_ChatRoomId",
                table: "TextMessages");

            migrationBuilder.DropIndex(
                name: "IX_TextMessages_ReceiverUserId",
                table: "TextMessages");

            migrationBuilder.DropIndex(
                name: "IX_TextMessages_SenderId",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "ChatRoomId",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "ReceiverUserId",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "TextMessages");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    TextMessageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatRoomId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.TextMessageId);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_ReceiverUserId",
                        column: x => x.ReceiverUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "ChatRoomId");
                    table.ForeignKey(
                        name: "FK_Messages_TextMessages_TextMessageId",
                        column: x => x.TextMessageId,
                        principalTable: "TextMessages",
                        principalColumn: "TextMessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatRoomId",
                table: "Messages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverUserId",
                table: "Messages",
                column: "ReceiverUserId");
        }
    }
}
