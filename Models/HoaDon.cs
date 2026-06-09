using System.ComponentModel.DataAnnotations;
namespace QLNT;

public class HoaDon
{
    public int HoaDonId { get; set; }

    [Required]
    public int HopDongId { get; set; }

    [Required]
    public int Thang { get; set; }

    [Required]
    public int Nam { get; set; }

    [Required]
    public decimal TienPhong { get; set; }

    public decimal TienDien { get; set; }

    public decimal TienNuoc { get; set; }

    public decimal TienDichVu { get; set; }

    public decimal TongTien { get; set; }

    public DateTime NgayLap { get; set; } = DateTime.Now;

    [StringLength(100)]
    public string? TrangThai { get; set; } = "Chưa thanh toán";

    public string? GhiChu { get; set; }

    // Navigation properties
    public HopDong? HopDong { get; set; }

    public ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
