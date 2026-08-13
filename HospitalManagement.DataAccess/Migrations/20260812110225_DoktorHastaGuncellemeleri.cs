using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DoktorHastaGuncellemeleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_Doktorlar_DepartmanId",
                table: "tbl_Doktorlar");

            migrationBuilder.AddColumn<DateTime>(
                name: "GuncellenmeTarihi",
                table: "tbl_Hastalar",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Hastalar_AktifMi",
                table: "tbl_Hastalar",
                column: "AktifMi");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Doktorlar_DepartmanId_AktifMi",
                table: "tbl_Doktorlar",
                columns: new[] { "DepartmanId", "AktifMi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_Hastalar_AktifMi",
                table: "tbl_Hastalar");

            migrationBuilder.DropIndex(
                name: "IX_tbl_Doktorlar_DepartmanId_AktifMi",
                table: "tbl_Doktorlar");

            migrationBuilder.DropColumn(
                name: "GuncellenmeTarihi",
                table: "tbl_Hastalar");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Doktorlar_DepartmanId",
                table: "tbl_Doktorlar",
                column: "DepartmanId");
        }
    }
}
