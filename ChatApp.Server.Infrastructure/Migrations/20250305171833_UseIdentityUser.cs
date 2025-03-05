using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_UserEntity_AdminUserIdId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomTextMessages_UserEntity_SenderId",
                table: "ChatRoomTextMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_UserEntity_User1Id",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_UserEntity_User2Id",
                table: "UserFriends");

            migrationBuilder.DropTable(
                name: "UserEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserFriends_User2Id",
                table: "UserFriends");

            migrationBuilder.AddColumn<string>(
                name: "User1Id1",
                table: "UserFriends",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "User2Id1",
                table: "UserFriends",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "ChatRoomTextMessages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "AdminUserIdId",
                table: "ChatRooms",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriends_User1Id1",
                table: "UserFriends",
                column: "User1Id1");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriends_User2Id1",
                table: "UserFriends",
                column: "User2Id1");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_AspNetUsers_AdminUserIdId",
                table: "ChatRooms",
                column: "AdminUserIdId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomTextMessages_AspNetUsers_SenderId",
                table: "ChatRoomTextMessages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_User1Id1",
                table: "UserFriends",
                column: "User1Id1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_User2Id1",
                table: "UserFriends",
                column: "User2Id1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_AspNetUsers_AdminUserIdId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomTextMessages_AspNetUsers_SenderId",
                table: "ChatRoomTextMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_User1Id1",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_User2Id1",
                table: "UserFriends");

            migrationBuilder.DropIndex(
                name: "IX_UserFriends_User1Id1",
                table: "UserFriends");

            migrationBuilder.DropIndex(
                name: "IX_UserFriends_User2Id1",
                table: "UserFriends");

            migrationBuilder.DropColumn(
                name: "User1Id1",
                table: "UserFriends");

            migrationBuilder.DropColumn(
                name: "User2Id1",
                table: "UserFriends");

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderId",
                table: "ChatRoomTextMessages",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminUserIdId",
                table: "ChatRooms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFriends_User2Id",
                table: "UserFriends",
                column: "User2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_UserEntity_AdminUserIdId",
                table: "ChatRooms",
                column: "AdminUserIdId",
                principalTable: "UserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomTextMessages_UserEntity_SenderId",
                table: "ChatRoomTextMessages",
                column: "SenderId",
                principalTable: "UserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_UserEntity_User1Id",
                table: "UserFriends",
                column: "User1Id",
                principalTable: "UserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_UserEntity_User2Id",
                table: "UserFriends",
                column: "User2Id",
                principalTable: "UserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
