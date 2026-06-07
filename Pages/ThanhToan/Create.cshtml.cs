using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.Globalization;

namespace QLNT.Pages_ThanhToan
{
    public class CreateModel : PageModel
    {

        [BindProperty]
        public string SoTienHienThi { get; set; } = "";
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ThanhToan ThanhToan { get; set; } = new();

        public HoaDon? HoaDonDangThanhToan { get; set; }

        public async Task<IActionResult> OnGetAsync(int hoaDonId)
        {
            HoaDonDangThanhToan = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.NguoiThue)
                .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

            if (HoaDonDangThanhToan == null)
            {
                return NotFound();
            }

            if (HoaDonDangThanhToan.TrangThai == "Đã thanh toán")
            {
                return RedirectToPage("/HoaDon/Index");
            }

            ThanhToan = new ThanhToan
            {
                HoaDonId = hoaDonId,
                SoTien = HoaDonDangThanhToan.TongTien,
                NgayThanhToan = DateTime.Today,
                PhuongThuc = "Tiền mặt"
            };

            SoTienHienThi = HoaDonDangThanhToan.TongTien.ToString("N0", new CultureInfo("vi-VN")) + " VNĐ";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            HoaDonDangThanhToan = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.NguoiThue)
                .FirstOrDefaultAsync(h => h.HoaDonId == ThanhToan.HoaDonId);
            ModelState.Remove("SoTienHienThi");
            if (HoaDonDangThanhToan == null)
            {
                return NotFound();
            }

            if (HoaDonDangThanhToan.TrangThai == "Đã thanh toán")
            {
                ModelState.AddModelError("", "Hóa đơn này đã được thanh toán.");
                return Page();
            }

            ThanhToan.SoTien = HoaDonDangThanhToan.TongTien;

            if (string.IsNullOrWhiteSpace(ThanhToan.PhuongThuc))
            {
                ModelState.AddModelError("ThanhToan.PhuongThuc", "Vui lòng chọn phương thức thanh toán.");
                return Page();
            }

            if (ThanhToan.NgayThanhToan == default)
            {
                ThanhToan.NgayThanhToan = DateTime.Now;
            }

            ModelState.Remove("ThanhToan.HoaDon");
            ModelState.Remove("ThanhToan.SoTien");
            ModelState.Remove("SoTienHienThi");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ThanhToans.Add(ThanhToan);

            HoaDonDangThanhToan.TrangThai = "Đã thanh toán";

            await _context.SaveChangesAsync();

            return RedirectToPage("/HoaDon/Index");
        }
    }
}