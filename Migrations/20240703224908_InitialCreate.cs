using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rent_A_Car.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BireyselKullanicilar",
                columns: table => new
                {
                    BireyselKullaniciId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", nullable: true),
                    Soyad = table.Column<string>(type: "TEXT", nullable: true),
                    Eposta = table.Column<string>(type: "TEXT", nullable: true),
                    Adres = table.Column<string>(type: "TEXT", nullable: true),
                    Telefon = table.Column<string>(type: "TEXT", nullable: true),
                    Sifre = table.Column<string>(type: "TEXT", nullable: true),
                    SifreKontrol = table.Column<string>(type: "TEXT", nullable: true),
                    KimlikNo = table.Column<string>(type: "TEXT", nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BireyselKullanicilar", x => x.BireyselKullaniciId);
                });

            migrationBuilder.CreateTable(
                name: "Iletisimler",
                columns: table => new
                {
                    IletisimId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IletisimAdSoyad = table.Column<string>(type: "TEXT", nullable: true),
                    IletisimEposta = table.Column<string>(type: "TEXT", nullable: true),
                    Telefon = table.Column<string>(type: "TEXT", nullable: true),
                    Mesaj = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iletisimler", x => x.IletisimId);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    KategoriId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KategoriAdi = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.KategoriId);
                });

            migrationBuilder.CreateTable(
                name: "KurumsalKullanicilar",
                columns: table => new
                {
                    KurumsalKullaniciId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirmaEposta = table.Column<string>(type: "TEXT", nullable: true),
                    FirmaSifre = table.Column<string>(type: "TEXT", nullable: true),
                    FirmaSifreKontrol = table.Column<string>(type: "TEXT", nullable: true),
                    FirmaAdi = table.Column<string>(type: "TEXT", nullable: true),
                    VergiNo = table.Column<string>(type: "TEXT", nullable: true),
                    VergiDairesi = table.Column<string>(type: "TEXT", nullable: true),
                    FirmaIl = table.Column<string>(type: "TEXT", nullable: true),
                    FirmaIlce = table.Column<string>(type: "TEXT", nullable: true),
                    VergiIl = table.Column<string>(type: "TEXT", nullable: true),
                    VergiIlce = table.Column<string>(type: "TEXT", nullable: true),
                    FirmaTelefon = table.Column<string>(type: "TEXT", nullable: true),
                    FaturaAdresi = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KurumsalKullanicilar", x => x.KurumsalKullaniciId);
                });

            migrationBuilder.CreateTable(
                name: "Sehirler",
                columns: table => new
                {
                    SehirId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IlAdi = table.Column<string>(type: "TEXT", nullable: true),
                    IlceAdi = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sehirler", x => x.SehirId);
                });

            migrationBuilder.CreateTable(
                name: "Araclar",
                columns: table => new
                {
                    AracId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Marka = table.Column<string>(type: "TEXT", nullable: true),
                    AracModel = table.Column<string>(type: "TEXT", nullable: true),
                    Vites = table.Column<string>(type: "TEXT", nullable: true),
                    Motor = table.Column<string>(type: "TEXT", nullable: true),
                    Koltuk = table.Column<string>(type: "TEXT", nullable: true),
                    Renk = table.Column<string>(type: "TEXT", nullable: true),
                    Resim = table.Column<string>(type: "TEXT", nullable: true),
                    Plaka = table.Column<string>(type: "TEXT", nullable: true),
                    Km = table.Column<int>(type: "INTEGER", nullable: false),
                    Musaitlik = table.Column<bool>(type: "INTEGER", nullable: true),
                    KategoriId = table.Column<int>(type: "INTEGER", nullable: false),
                    SehirId = table.Column<int>(type: "INTEGER", nullable: false),
                    AracFiyat = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Araclar", x => x.AracId);
                    table.ForeignKey(
                        name: "FK_Araclar_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "KategoriId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Araclar_Sehirler_SehirId",
                        column: x => x.SehirId,
                        principalTable: "Sehirler",
                        principalColumn: "SehirId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezervasyonlar",
                columns: table => new
                {
                    RezervasyonId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeslimTarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IadeTarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TeslimYeriId = table.Column<int>(type: "INTEGER", nullable: true),
                    IadeYeriId = table.Column<int>(type: "INTEGER", nullable: true),
                    TeslimYeriAdi = table.Column<string>(type: "TEXT", nullable: true),
                    IadeYeriAdi = table.Column<string>(type: "TEXT", nullable: true),
                    Sigorta = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AracId = table.Column<int>(type: "INTEGER", nullable: false),
                    BireyselKullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    KurumsalKullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    Fiyat = table.Column<decimal>(type: "TEXT", nullable: false),
                    SehirId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervasyonlar", x => x.RezervasyonId);
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "AracId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_BireyselKullanicilar_BireyselKullaniciId",
                        column: x => x.BireyselKullaniciId,
                        principalTable: "BireyselKullanicilar",
                        principalColumn: "BireyselKullaniciId");
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_KurumsalKullanicilar_KurumsalKullaniciId",
                        column: x => x.KurumsalKullaniciId,
                        principalTable: "KurumsalKullanicilar",
                        principalColumn: "KurumsalKullaniciId");
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_Sehirler_SehirId",
                        column: x => x.SehirId,
                        principalTable: "Sehirler",
                        principalColumn: "SehirId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_KategoriId",
                table: "Araclar",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_SehirId",
                table: "Araclar",
                column: "SehirId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_AracId",
                table: "Rezervasyonlar",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_BireyselKullaniciId",
                table: "Rezervasyonlar",
                column: "BireyselKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_KurumsalKullaniciId",
                table: "Rezervasyonlar",
                column: "KurumsalKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_SehirId",
                table: "Rezervasyonlar",
                column: "SehirId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Iletisimler");

            migrationBuilder.DropTable(
                name: "Rezervasyonlar");

            migrationBuilder.DropTable(
                name: "Araclar");

            migrationBuilder.DropTable(
                name: "BireyselKullanicilar");

            migrationBuilder.DropTable(
                name: "KurumsalKullanicilar");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "Sehirler");
        }
    }
}
