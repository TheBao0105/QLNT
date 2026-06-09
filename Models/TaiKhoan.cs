using System.ComponentModel.DataAnnotations;

namespace QLNT.Models
{
    public class TaiKhoan
    {
        public int Id { get; set; }

        public string HoTen { get; set; } = "";

        public string Email { get; set; } = "";

        public string MatKhau { get; set; } = "";

        public string VaiTro { get; set; } = "";
    }
}