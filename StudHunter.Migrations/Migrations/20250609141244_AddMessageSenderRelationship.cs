using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudHunter.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageSenderRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                schema: "studhunter",
                table: "Messages",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId",
                schema: "studhunter",
                table: "Messages");
        }
    }
}
