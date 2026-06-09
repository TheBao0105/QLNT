using System.ComponentModel.DataAnnotations;

namespace QLNT;

public class NhaTro
{
    [Key]
    public int NhaTroId { get; set; }

    [Required]
    public string TenNhaTro { get; set; } = "";

    [Required]
    public string DiaChi { get; set; } = "";

    [Required]
    public int SoTang { get; set; }

    [Required]
    public int SoPhong { get; set; }

    public string? MaTro { get; set; }

    public string? MoTa { get; set; }

    public ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}