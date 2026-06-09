using System.ComponentModel.DataAnnotations;
namespace QLNT;

public class Phong
{
    [Key]
    public int PhongId { get; set; }

    [Required]
    [StringLength(50)]
    public string TenPhong { get; set; } = string.Empty;

    [Required]
    public decimal GiaPhong { get; set; }

    public int? DienTich { get; set; }

    [StringLength(100)]
    public string TrangThai { get; set; } = "Trống";

    public string? GhiChu { get; set; }
    public decimal Giathue { get; set; }

    // Quan hệ: 1 phòng có nhiều hợp đồng
    public ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();
}
