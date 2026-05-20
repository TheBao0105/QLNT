using System.ComponentModel.DataAnnotations;
namespace QLNT;

public class DichVu
{
    [Key]
    public int DichVuId { get; set; }

    [Required]
    [StringLength(100)]
    public string TenDichVu { get; set; } = string.Empty;

    [Required]
    public decimal DonGia { get; set; }

    [StringLength(50)]
    public string? DonViTinh { get; set; }

    public string? GhiChu { get; set; }
}
