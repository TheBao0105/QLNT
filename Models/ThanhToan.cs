using System.ComponentModel.DataAnnotations;
namespace QLNT;

public class ThanhToan
{
    [Key]
    public int ThanhToanId { get; set; }

    [Required]
    public int HoaDonId { get; set; }

    [Required]
    public DateTime NgayThanhToan { get; set; } = DateTime.Now;

    [Required]
    public decimal SoTien { get; set; }

    [StringLength(100)]
    public string? PhuongThuc { get; set; }

    public string? GhiChu { get; set; }

    // Navigation property
    public HoaDon? HoaDon { get; set; }
}
