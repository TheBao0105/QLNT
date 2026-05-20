using System.ComponentModel.DataAnnotations;
namespace QLNT;

public class TaiKhoan
{
    [Key]
    public int TaiKhoanId { get; set; }

    [Required]
    [StringLength(50)]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string MatKhau { get; set; } = string.Empty;

    [StringLength(100)]
    public string? HoTen { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? VaiTro { get; set; } = "NhanVien";

    public bool TrangThai { get; set; } = true;
}
