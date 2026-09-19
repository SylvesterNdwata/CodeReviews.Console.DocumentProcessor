using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace silvermax.DocumentProcessor.Migrations
{
    /// <inheritdoc />
    public partial class UniquePhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Contacts_PhoneNumber",
                table: "Contacts",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contacts_PhoneNumber",
                table: "Contacts");
        }
    }
}
