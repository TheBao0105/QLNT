using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HopDong
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public HopDong HopDong { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopdong = await _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .FirstOrDefaultAsync(m => m.HopDongId == id);

            if (hopdong == null)
            {
                return NotFound();
            }

            HopDong = hopdong;
            return Page();
        }
    }
}