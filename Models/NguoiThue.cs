using System.ComponentModel.DataAnnotations;
namespace QLNT;

public class NguoiThue
{
    [Key]
    public int NguoiThueId { get; set; }

    [Required]
    [StringLength(100)]
    public string HoTen { get; set; } = string.Empty;

    [StringLength(20)]
    public string? SoDienThoai { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? CCCD { get; set; }

    public DateTime? NgaySinh { get; set; }

    [StringLength(200)]
    public string? DiaChi { get; set; }

    // Quan hệ: 1 người thuê có nhiều hợp đồng
    public ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();
}
