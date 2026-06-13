using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNT.Migrations
{
    /// <inheritdoc />
    public partial class BoSungIdChuTroVaoDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdChuTro",
                table: "ThanhToans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdChuTro",
                table: "TaiKhoans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "Phongs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdChuTro",
                table: "Phongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdChuTro",
                table: "NguoiThues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdChuTro",
                table: "HopDongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdChuTro",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdChuTro",
                table: "DichVus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdChuTro",
                table: "ThanhToans");

            migrationBuilder.DropColumn(
                name: "IdChuTro",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "IdChuTro",
                table: "Phongs");

            migrationBuilder.DropColumn(
                name: "IdChuTro",
                table: "NguoiThues");

            migrationBuilder.DropColumn(
                name: "IdChuTro",
                table: "HopDongs");

            migrationBuilder.DropColumn(
                name: "IdChuTro",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "IdChuTro",
                table: "DichVus");

            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "Phongs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
