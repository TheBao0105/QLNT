using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.Globalization;

namespace QLNT.Pages_ThanhToan
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ThanhToan ThanhToan { get; set; } = default!;

        public HoaDon? HoaDonDangThanhToan { get; set; }

        public string SoTienHienThi { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhToan = await _context.ThanhToans
                .Include(t => t.HoaDon)
                    .ThenInclude(h => h.HopDong)
                        .ThenInclude(hd => hd.Phong)
                .Include(t => t.HoaDon)
                    .ThenInclude(h => h.HopDong)
                        .ThenInclude(hd => hd.NguoiThue)
                .FirstOrDefaultAsync(t => t.ThanhToanId == id.Value);

            if (thanhToan == null)
            {
                return NotFound();
            }

            ThanhToan = thanhToan;
            HoaDonDangThanhToan = thanhToan.HoaDon;
            SoTienHienThi = FormatVnd(thanhToan.SoTien);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("ThanhToan.HoaDon");
            ModelState.Remove("ThanhToan.SoTien");

            if (!ModelState.IsValid)
            {
                await LoadHoaDonForInvalidPageAsync();
                return Page();
            }

            var thanhToanDb = await _context.ThanhToans
                .Include(t => t.HoaDon)
                    .ThenInclude(h => h.HopDong)
                        .ThenInclude(hd => hd.Phong)
                .Include(t => t.HoaDon)
                    .ThenInclude(h => h.HopDong)
                        .ThenInclude(hd => hd.NguoiThue)
                .FirstOrDefaultAsync(t => t.ThanhToanId == ThanhToan.ThanhToanId);

            if (thanhToanDb == null)
            {
                return NotFound();
            }

            thanhToanDb.NgayThanhToan = ThanhToan.NgayThanhToan;
            thanhToanDb.PhuongThuc = ThanhToan.PhuongThuc;
            thanhToanDb.GhiChu = ThanhToan.GhiChu;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thanh toán thành công.";

            return RedirectToPage("/HoaDon/Index");
        }

        private async Task LoadHoaDonForInvalidPageAsync()
        {
            var thanhToanDb = await _context.ThanhToans
                .Include(t => t.HoaDon)
                    .ThenInclude(h => h.HopDong)
                        .ThenInclude(hd => hd.Phong)
                .Include(t => t.HoaDon)
                    .ThenInclude(h => h.HopDong)
                        .ThenInclude(hd => hd.NguoiThue)
                .FirstOrDefaultAsync(t => t.ThanhToanId == ThanhToan.ThanhToanId);

            HoaDonDangThanhToan = thanhToanDb?.HoaDon;

            if (thanhToanDb != null)
            {
                SoTienHienThi = FormatVnd(thanhToanDb.SoTien);
            }
        }

        public string FormatVnd(decimal value)
        {
            return value.ToString("N0", new CultureInfo("vi-VN")) + " VNĐ";
        }
    }
}