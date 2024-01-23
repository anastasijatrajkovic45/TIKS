using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agencije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojTelefona = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencije", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aktivnosti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cena = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aktivnosti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recenzije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Korisnik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Komentar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ocena = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzije", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Putovanja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojNocenja = table.Column<int>(type: "int", nullable: false),
                    Prevoz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cena = table.Column<int>(type: "int", nullable: false),
                    AgencijaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Putovanja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Putovanja_Agencije_AgencijaId",
                        column: x => x.AgencijaId,
                        principalTable: "Agencije",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AktivnostPutovanje",
                columns: table => new
                {
                    AktivnostiId = table.Column<int>(type: "int", nullable: false),
                    PutovanjeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AktivnostPutovanje", x => new { x.AktivnostiId, x.PutovanjeId });
                    table.ForeignKey(
                        name: "FK_AktivnostPutovanje_Aktivnosti_AktivnostiId",
                        column: x => x.AktivnostiId,
                        principalTable: "Aktivnosti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AktivnostPutovanje_Putovanja_PutovanjeId",
                        column: x => x.PutovanjeId,
                        principalTable: "Putovanja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PutovanjeRecenzija",
                columns: table => new
                {
                    PutovanjeId = table.Column<int>(type: "int", nullable: false),
                    RecenzijeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PutovanjeRecenzija", x => new { x.PutovanjeId, x.RecenzijeId });
                    table.ForeignKey(
                        name: "FK_PutovanjeRecenzija_Putovanja_PutovanjeId",
                        column: x => x.PutovanjeId,
                        principalTable: "Putovanja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PutovanjeRecenzija_Recenzije_RecenzijeId",
                        column: x => x.RecenzijeId,
                        principalTable: "Recenzije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojTelefona = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojOsoba = table.Column<int>(type: "int", nullable: false),
                    PutovanjeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Putovanja_PutovanjeId",
                        column: x => x.PutovanjeId,
                        principalTable: "Putovanja",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AktivnostPutovanje_PutovanjeId",
                table: "AktivnostPutovanje",
                column: "PutovanjeId");

            migrationBuilder.CreateIndex(
                name: "IX_Putovanja_AgencijaId",
                table: "Putovanja",
                column: "AgencijaId");

            migrationBuilder.CreateIndex(
                name: "IX_PutovanjeRecenzija_RecenzijeId",
                table: "PutovanjeRecenzija",
                column: "RecenzijeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_PutovanjeId",
                table: "Rezervacije",
                column: "PutovanjeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AktivnostPutovanje");

            migrationBuilder.DropTable(
                name: "PutovanjeRecenzija");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropTable(
                name: "Aktivnosti");

            migrationBuilder.DropTable(
                name: "Recenzije");

            migrationBuilder.DropTable(
                name: "Putovanja");

            migrationBuilder.DropTable(
                name: "Agencije");
        }
    }
}
