using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackBank.Migrations
{
    /// <inheritdoc />
    public partial class DodanoPrzelewy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Przelewy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NadawcaID = table.Column<int>(type: "INTEGER", nullable: false),
                    OdbiorcaID = table.Column<int>(type: "INTEGER", nullable: false),
                    Kwota = table.Column<float>(type: "REAL", nullable: false),
                    DataPrzelewu = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Przelewy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Przelewy_DaneUzytkownikow_NadawcaID",
                        column: x => x.NadawcaID,
                        principalTable: "DaneUzytkownikow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Przelewy_DaneUzytkownikow_OdbiorcaID",
                        column: x => x.OdbiorcaID,
                        principalTable: "DaneUzytkownikow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Przelewy_NadawcaID",
                table: "Przelewy",
                column: "NadawcaID");

            migrationBuilder.CreateIndex(
                name: "IX_Przelewy_OdbiorcaID",
                table: "Przelewy",
                column: "OdbiorcaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Przelewy");
        }
    }
}
