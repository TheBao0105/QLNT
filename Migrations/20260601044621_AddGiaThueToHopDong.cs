using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNT.Migrations
{
    /// <inheritdoc />
    public partial class AddGiaThueToHopDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GiaThue",
                table: "HopDongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaThue",
                table: "HopDongs");
        }
    }
}
