using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNT.Migrations
{
    /// <inheritdoc />
    public partial class AddResetTokenToTaiKhoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenPhong",
                table: "HopDongs");

            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "TaiKhoans",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "TaiKhoans",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "TaiKhoans");

            migrationBuilder.AddColumn<string>(
                name: "TenPhong",
                table: "HopDongs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
