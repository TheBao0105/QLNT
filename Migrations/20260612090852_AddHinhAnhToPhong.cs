using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNT.Migrations
{
    /// <inheritdoc />
    public partial class AddHinhAnhToPhong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "Phongs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "Phongs");
        }
    }
}
