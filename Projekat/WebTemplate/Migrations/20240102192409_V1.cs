using System;
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
                name: "Rezervacije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Smestaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumOd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrojOsoba = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
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
                    AgencijaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Korisnici_Agencije_AgencijaId",
                        column: x => x.AgencijaId,
                        principalTable: "Agencije",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Putovanja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojNocenja = table.Column<int>(type: "int", nullable: false),
                    Prevoz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cena = table.Column<int>(type: "int", nullable: false),
                    AgencijaId = table.Column<int>(type: "int", nullable: true),
                    RezervacijaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Putovanja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Putovanja_Agencije_AgencijaId",
                        column: x => x.AgencijaId,
                        principalTable: "Agencije",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Putovanja_Rezervacije_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacije",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KorisnikRezervacija",
                columns: table => new
                {
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    RezervacijaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikRezervacija", x => new { x.KorisnikId, x.RezervacijaId });
                    table.ForeignKey(
                        name: "FK_KorisnikRezervacija_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorisnikRezervacija_Rezervacije_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recenzije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Komentar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ocena = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recenzije_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
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

            migrationBuilder.CreateIndex(
                name: "IX_AktivnostPutovanje_PutovanjeId",
                table: "AktivnostPutovanje",
                column: "PutovanjeId");

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_AgencijaId",
                table: "Korisnici",
                column: "AgencijaId");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikRezervacija_RezervacijaId",
                table: "KorisnikRezervacija",
                column: "RezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Putovanja_AgencijaId",
                table: "Putovanja",
                column: "AgencijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Putovanja_RezervacijaId",
                table: "Putovanja",
                column: "RezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_KorisnikId",
                table: "Recenzije",
                column: "KorisnikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AktivnostPutovanje");

            migrationBuilder.DropTable(
                name: "KorisnikRezervacija");

            migrationBuilder.DropTable(
                name: "Recenzije");

            migrationBuilder.DropTable(
                name: "Aktivnosti");

            migrationBuilder.DropTable(
                name: "Putovanja");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropTable(
                name: "Agencije");
        }
    }
}
