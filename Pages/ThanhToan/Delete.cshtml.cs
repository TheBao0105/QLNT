using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_ThanhToan
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ThanhToan ThanhToan { get; set; } = default!;

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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var thanhToan = await _context.ThanhToans
                .FirstOrDefaultAsync(t => t.ThanhToanId == ThanhToan.ThanhToanId);

            if (thanhToan == null)
            {
                return NotFound();
            }

            var hoaDonId = thanhToan.HoaDonId;

            _context.ThanhToans.Remove(thanhToan);
            await _context.SaveChangesAsync();

            var conThanhToanKhac = await _context.ThanhToans
                .AnyAsync(t => t.HoaDonId == hoaDonId);

            var hoaDon = await _context.HoaDons
                .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

            if (hoaDon != null && !conThanhToanKhac)
            {
                hoaDon.TrangThai = "Chưa thanh toán";
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Đã xóa thanh toán.";

            return RedirectToPage("/HoaDon/Index");
        }
    }
}