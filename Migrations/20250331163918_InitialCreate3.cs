using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackBank.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone",
                table: "DaneUzytkownikow");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DaneUzytkownikow",
                newName: "id");

            migrationBuilder.AddColumn<float>(
                name: "balance",
                table: "DaneUzytkownikow",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "balance",
                table: "DaneUzytkownikow");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DaneUzytkownikow",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "DaneUzytkownikow",
                type: "TEXT",
                maxLength: 9,
                nullable: false,
                defaultValue: "");
        }
    }
}
