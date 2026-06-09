using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLNT.Data;

namespace QLNT.Pages.TaiKhoan
{
    public class DangNhapModel : PageModel
    {
        private readonly AppDbContext _context;

        public DangNhapModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string MatKhau { get; set; } = "";

        public string ThongBao { get; set; } = "";

        public IActionResult OnPost()
        {
            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(x => x.Email == Email && x.MatKhau == MatKhau);

            if (taiKhoan == null)
            {
                ThongBao = "Sai email hoặc mật khẩu";
                return Page();
            }

            if (taiKhoan.VaiTro == "ChuTro")
            {
                return RedirectToPage("/ChuTro/Index");
            }

            if (taiKhoan.VaiTro == "NguoiThue")
            {
                return RedirectToPage("/NguoiThue/ThamGiaTro/Index");
            }

            return RedirectToPage("/Index");
        }
    }
}