using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Server.Application.Migrations
{
    /// <inheritdoc />
    public partial class Modifymessagesstructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRoomTextMessages");

            migrationBuilder.AddColumn<Guid>(
                name: "ChatRoomEntityChatRoomId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TextMessages",
                columns: table => new
                {
                    TextMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextMessages", x => x.TextMessageId);
                });

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    TextMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.TextMessageId);
                    table.ForeignKey(
                        name: "FK_UserMessages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMessages_TextMessages_TextMessageId",
                        column: x => x.TextMessageId,
                        principalTable: "TextMessages",
                        principalColumn: "TextMessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChatRoomEntityChatRoomId",
                table: "AspNetUsers",
                column: "ChatRoomEntityChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_ReceiverId",
                table: "UserMessages",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ChatRooms_ChatRoomEntityChatRoomId",
                table: "AspNetUsers",
                column: "ChatRoomEntityChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "ChatRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ChatRooms_ChatRoomEntityChatRoomId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropTable(
                name: "TextMessages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChatRoomEntityChatRoomId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChatRoomEntityChatRoomId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ChatRoomTextMessages",
                columns: table => new
                {
                    ChatRoomTextMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomTextMessages", x => x.ChatRoomTextMessageId);
                    table.ForeignKey(
                        name: "FK_ChatRoomTextMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatRoomTextMessages_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "ChatRoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoomTextMessages_ChatRoomId",
                table: "ChatRoomTextMessages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoomTextMessages_SenderId",
                table: "ChatRoomTextMessages",
                column: "SenderId");
        }
    }
}
