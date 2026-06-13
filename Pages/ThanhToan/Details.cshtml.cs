using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_ThanhToan
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

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
    }
}