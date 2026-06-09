using System.ComponentModel.DataAnnotations;

namespace QLNT;

public class HopDong
{
    [Key]
    public int HopDongId { get; set; }

    [Required]
    [StringLength(50)]
    public string MaHopDong { get; set; } = "";

    [Required]
    public int PhongId { get; set; }

    [Required]
    public int NguoiThueId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime NgayBatDau { get; set; }

    [DataType(DataType.Date)]
    public DateTime? NgayKetThuc { get; set; }

    [Required]
    public decimal TienCoc { get; set; }

    [Required]
    public decimal GiaThue { get; set; }

    [StringLength(100)]
    public string? TrangThai { get; set; } = "Còn hiệu lực";

    public string? GhiChu { get; set; }

    public Phong? Phong { get; set; }

    public NguoiThue? NguoiThue { get; set; }

    public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}