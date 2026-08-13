using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RandevuKurallariGuncellemesi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_Randevular_DoktorId_BaslangicZamani",
                table: "tbl_Randevular");

            migrationBuilder.DropIndex(
                name: "IX_tbl_Randevular_HastaId_BaslangicZamani",
                table: "tbl_Randevular");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Randevular_DoktorId_BaslangicZamani",
                table: "tbl_Randevular",
                columns: new[] { "DoktorId", "BaslangicZamani" });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Randevular_HastaId_BaslangicZamani",
                table: "tbl_Randevular",
                columns: new[] { "HastaId", "BaslangicZamani" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Randevu_BitisBaslangictanSonra",
                table: "tbl_Randevular",
                sql: "\"BitisZamani\" > \"BaslangicZamani\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_Randevular_DoktorId_BaslangicZamani",
                table: "tbl_Randevular");

            migrationBuilder.DropIndex(
                name: "IX_tbl_Randevular_HastaId_BaslangicZamani",
                table: "tbl_Randevular");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Randevu_BitisBaslangictanSonra",
                table: "tbl_Randevular");

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
        }
    }
}
