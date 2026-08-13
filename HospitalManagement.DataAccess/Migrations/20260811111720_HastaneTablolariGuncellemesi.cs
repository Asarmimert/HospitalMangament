using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HospitalManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class HastaneTablolariGuncellemesi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.CreateTable(
                name: "tbl_Departmanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Departmanlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Ilaclar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Ilaclar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_KullaniciHesaplari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Eposta = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ParolaHash = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Rol = table.Column<int>(type: "integer", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_KullaniciHesaplari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Teshisler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeshisKodu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TeshisAdi = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Teshisler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Doktorlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciHesabiId = table.Column<int>(type: "integer", nullable: false),
                    DepartmanId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    TelefonNumarasi = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    UzmanlikAlani = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Doktorlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Doktorlar_tbl_Departmanlar_DepartmanId",
                        column: x => x.DepartmanId,
                        principalTable: "tbl_Departmanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_Doktorlar_tbl_KullaniciHesaplari_KullaniciHesabiId",
                        column: x => x.KullaniciHesabiId,
                        principalTable: "tbl_KullaniciHesaplari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Hastalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciHesabiId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    KimlikNumarasi = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    DogumTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    TelefonNumarasi = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Adres = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Hastalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Hastalar_tbl_KullaniciHesaplari_KullaniciHesabiId",
                        column: x => x.KullaniciHesabiId,
                        principalTable: "tbl_KullaniciHesaplari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Sekreterler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciHesabiId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TelefonNumarasi = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Sekreterler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Sekreterler_tbl_KullaniciHesaplari_KullaniciHesabiId",
                        column: x => x.KullaniciHesabiId,
                        principalTable: "tbl_KullaniciHesaplari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Randevular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DoktorId = table.Column<int>(type: "integer", nullable: false),
                    HastaId = table.Column<int>(type: "integer", nullable: false),
                    OlusturanSekreterId = table.Column<int>(type: "integer", nullable: false),
                    BaslangicZamani = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BitisZamani = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    IptalNedeni = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Randevular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Randevular_tbl_Doktorlar_DoktorId",
                        column: x => x.DoktorId,
                        principalTable: "tbl_Doktorlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_Randevular_tbl_Hastalar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "tbl_Hastalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_Randevular_tbl_Sekreterler_OlusturanSekreterId",
                        column: x => x.OlusturanSekreterId,
                        principalTable: "tbl_Sekreterler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Muayeneler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RandevuId = table.Column<int>(type: "integer", nullable: false),
                    HastaSikayeti = table.Column<string>(type: "text", nullable: false),
                    DoktorDegerlendirmesi = table.Column<string>(type: "text", nullable: false),
                    DoktorNotlari = table.Column<string>(type: "text", nullable: true),
                    MuayeneTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Muayeneler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Muayeneler_tbl_Randevular_RandevuId",
                        column: x => x.RandevuId,
                        principalTable: "tbl_Randevular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_MuayeneTeshisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MuayeneId = table.Column<int>(type: "integer", nullable: false),
                    TeshisId = table.Column<int>(type: "integer", nullable: false),
                    DoktorNotu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_MuayeneTeshisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_MuayeneTeshisleri_tbl_Muayeneler_MuayeneId",
                        column: x => x.MuayeneId,
                        principalTable: "tbl_Muayeneler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_MuayeneTeshisleri_tbl_Teshisler_TeshisId",
                        column: x => x.TeshisId,
                        principalTable: "tbl_Teshisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Receteler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MuayeneId = table.Column<int>(type: "integer", nullable: false),
                    HastaId = table.Column<int>(type: "integer", nullable: false),
                    DoktorId = table.Column<int>(type: "integer", nullable: false),
                    ReceteTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GenelNotlar = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Receteler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Receteler_tbl_Doktorlar_DoktorId",
                        column: x => x.DoktorId,
                        principalTable: "tbl_Doktorlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_Receteler_tbl_Hastalar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "tbl_Hastalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_Receteler_tbl_Muayeneler_MuayeneId",
                        column: x => x.MuayeneId,
                        principalTable: "tbl_Muayeneler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_ReceteIcerik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceteId = table.Column<int>(type: "integer", nullable: false),
                    IlacId = table.Column<int>(type: "integer", nullable: false),
                    KullanimTalimatlari = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    KullanimSuresi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Miktar = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_ReceteIcerik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_ReceteIcerik_tbl_Ilaclar_IlacId",
                        column: x => x.IlacId,
                        principalTable: "tbl_Ilaclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_ReceteIcerik_tbl_Receteler_ReceteId",
                        column: x => x.ReceteId,
                        principalTable: "tbl_Receteler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Departmanlar_Ad",
                table: "tbl_Departmanlar",
                column: "Ad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Doktorlar_DepartmanId",
                table: "tbl_Doktorlar",
                column: "DepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Doktorlar_KullaniciHesabiId",
                table: "tbl_Doktorlar",
                column: "KullaniciHesabiId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Hastalar_KimlikNumarasi",
                table: "tbl_Hastalar",
                column: "KimlikNumarasi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Hastalar_KullaniciHesabiId",
                table: "tbl_Hastalar",
                column: "KullaniciHesabiId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Ilaclar_Ad",
                table: "tbl_Ilaclar",
                column: "Ad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_KullaniciHesaplari_Eposta",
                table: "tbl_KullaniciHesaplari",
                column: "Eposta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Muayeneler_RandevuId",
                table: "tbl_Muayeneler",
                column: "RandevuId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_MuayeneTeshisleri_MuayeneId_TeshisId",
                table: "tbl_MuayeneTeshisleri",
                columns: new[] { "MuayeneId", "TeshisId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_MuayeneTeshisleri_TeshisId",
                table: "tbl_MuayeneTeshisleri",
                column: "TeshisId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Randevular_DoktorId_BaslangicZamani",
                table: "tbl_Randevular",
                columns: new[] { "DoktorId", "BaslangicZamani" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Randevular_HastaId_BaslangicZamani",
                table: "tbl_Randevular",
                columns: new[] { "HastaId", "BaslangicZamani" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Randevular_OlusturanSekreterId",
                table: "tbl_Randevular",
                column: "OlusturanSekreterId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ReceteIcerik_IlacId",
                table: "tbl_ReceteIcerik",
                column: "IlacId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ReceteIcerik_ReceteId",
                table: "tbl_ReceteIcerik",
                column: "ReceteId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Receteler_DoktorId_ReceteTarihi",
                table: "tbl_Receteler",
                columns: new[] { "DoktorId", "ReceteTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Receteler_HastaId_ReceteTarihi",
                table: "tbl_Receteler",
                columns: new[] { "HastaId", "ReceteTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Receteler_MuayeneId",
                table: "tbl_Receteler",
                column: "MuayeneId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Sekreterler_KullaniciHesabiId",
                table: "tbl_Sekreterler",
                column: "KullaniciHesabiId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_MuayeneTeshisleri");

            migrationBuilder.DropTable(
                name: "tbl_ReceteIcerik");

            migrationBuilder.DropTable(
                name: "tbl_Teshisler");

            migrationBuilder.DropTable(
                name: "tbl_Ilaclar");

            migrationBuilder.DropTable(
                name: "tbl_Receteler");

            migrationBuilder.DropTable(
                name: "tbl_Muayeneler");

            migrationBuilder.DropTable(
                name: "tbl_Randevular");

            migrationBuilder.DropTable(
                name: "tbl_Doktorlar");

            migrationBuilder.DropTable(
                name: "tbl_Hastalar");

            migrationBuilder.DropTable(
                name: "tbl_Sekreterler");

            migrationBuilder.DropTable(
                name: "tbl_Departmanlar");

            migrationBuilder.DropTable(
                name: "tbl_KullaniciHesaplari");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoktorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    DoktorFirstName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DoktorLastName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoktorId);
                    table.ForeignKey(
                        name: "FK_Doctors_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_DepartmentId",
                table: "Doctors",
                column: "DepartmentId");
        }
    }
}
