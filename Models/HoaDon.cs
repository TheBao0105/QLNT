using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNT;

public class HoaDon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HoaDonId { get; set; }

    [Required]

    public int HopDongId { get; set; }
    public string TenPhong { get; set; } = string.Empty;
    [Required]
    public int Thang { get; set; }

    [Required]
    public int Nam { get; set; }

    [Required]
    public decimal TienPhong { get; set; }

    public decimal TienDien { get; set; }
    public decimal ChiSoDienCu { get; set; }
    public decimal ChiSoDienMoi { get; set; }

    public decimal DonGiaDien { get; set; }
    public decimal SoDienTieuThu { get; set; }
    public decimal TienNuoc { get; set; }
    public decimal ChiSoNuocCu { get; set; }
    public decimal ChiSoNuocMoi { get; set; }
    public decimal DonGiaNuoc { get; set; }
    public decimal SoNuocTieuThu { get; set; }
    public decimal TienDichVu { get; set; }

    public decimal TongTien { get; set; }

    public DateTime NgayLap { get; set; } = DateTime.Now;

    [StringLength(100)]
    public string? TrangThai { get; set; } = "Chưa thanh toán";
    public decimal DaThanhtoan { get; set; } = 0;
    public string? GhiChu { get; set; }

    public string GiaThue { get; set; } = string.Empty;
    // Navigation properties
    public HopDong? HopDong { get; set; }
    public DateTime? HanThanhToan { get; set; }
    public DateTime? NgayThanhToan { get; set; }
    public ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
