using System.ComponentModel.DataAnnotations;

namespace QLNT;

public class HopDong
{
    [Key]
    public int HopDongId { get; set; }

    [Required]
    public int PhongId { get; set; }

    [Required]
    public int NguoiThueId { get; set; }

    [Required]
    public DateTime NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    [Required]
    public decimal TienCoc { get; set; }

    [StringLength(100)]
    public string? TrangThai { get; set; } = "Còn hiệu lực";

    public string? GhiChu { get; set; }

    // Navigation properties
    public Phong? Phong { get; set; }

    public NguoiThue? NguoiThue { get; set; }

    // Quan hệ: 1 hợp đồng có nhiều hóa đơn
    public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
