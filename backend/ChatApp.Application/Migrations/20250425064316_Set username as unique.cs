using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Application.Migrations
{
    /// <inheritdoc />
    public partial class Setusernameasunique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "PublicUserInfos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PublicUserInfos_Username",
                table: "PublicUserInfos",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PublicUserInfos_Username",
                table: "PublicUserInfos");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "PublicUserInfos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
