using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNT.Migrations
{
    /// <inheritdoc />
    public partial class AddNhaTro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NhaTroId",
                table: "Phongs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NhaTros",
                columns: table => new
                {
                    NhaTroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhaTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoTang = table.Column<int>(type: "int", nullable: false),
                    SoPhong = table.Column<int>(type: "int", nullable: false),
                    MaTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaTros", x => x.NhaTroId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_NhaTroId",
                table: "Phongs",
                column: "NhaTroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Phongs_NhaTros_NhaTroId",
                table: "Phongs",
                column: "NhaTroId",
                principalTable: "NhaTros",
                principalColumn: "NhaTroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Phongs_NhaTros_NhaTroId",
                table: "Phongs");

            migrationBuilder.DropTable(
                name: "NhaTros");

            migrationBuilder.DropIndex(
                name: "IX_Phongs_NhaTroId",
                table: "Phongs");

            migrationBuilder.DropColumn(
                name: "NhaTroId",
                table: "Phongs");
        }
    }
}
