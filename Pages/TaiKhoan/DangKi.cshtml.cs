using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLNT.Data;

namespace QLNT.Pages.TaiKhoan
{
    public class DangKiModel : PageModel
    {
        private readonly AppDbContext _context;

        public DangKiModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string HoTen { get; set; } = "";

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string MatKhau { get; set; } = "";

        [BindProperty]
        public string VaiTro { get; set; } = "";

        public string ThongBao { get; set; } = "";

        public void OnGet(string? vaiTro)
        {
            VaiTro = vaiTro ?? "";
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(VaiTro))
            {
                ThongBao = "Vui lòng chọn vai trò ở trang chủ trước khi đăng ký.";
                return Page();
            }

            var taiKhoan = new QLNT.Models.TaiKhoan
            {
                HoTen = HoTen,
                Email = Email,
                MatKhau = MatKhau,
                VaiTro = VaiTro
            };

            _context.TaiKhoans.Add(taiKhoan);
            _context.SaveChanges();

            return RedirectToPage("/TaiKhoan/DangNhap");
        }
    }
}