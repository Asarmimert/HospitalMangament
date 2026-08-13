using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TeshisVeMuayeneGuncellemeleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tbl_Teshisler_AktifMi",
                table: "tbl_Teshisler",
                column: "AktifMi");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Teshisler_TeshisKodu",
                table: "tbl_Teshisler",
                column: "TeshisKodu",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_Teshisler_AktifMi",
                table: "tbl_Teshisler");

            migrationBuilder.DropIndex(
                name: "IX_tbl_Teshisler_TeshisKodu",
                table: "tbl_Teshisler");
        }
    }
}
