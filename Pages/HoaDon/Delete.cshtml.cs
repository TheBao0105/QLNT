using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using Microsoft.AspNetCore.Authorization;

namespace QLNT.Pages_HoaDon
{
        [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HoaDon HoaDon { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.Phong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.NguoiThue)
                .Include(h => h.ThanhToans)
                .FirstOrDefaultAsync(h => h.HoaDonId == id.Value);

            if (hoaDon == null)
            {
                return NotFound();
            }

            HoaDon = hoaDon;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.ThanhToans)
                .FirstOrDefaultAsync(h => h.HoaDonId == HoaDon.HoaDonId);

            if (hoaDon == null)
            {
                return NotFound();
            }

            if (hoaDon.ThanhToans != null && hoaDon.ThanhToans.Any())
            {
                _context.ThanhToans.RemoveRange(hoaDon.ThanhToans);
            }

            _context.HoaDons.Remove(hoaDon);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa hóa đơn.";

            return RedirectToPage("./Index");
        }
    }
}