using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using Microsoft.AspNetCore.Authorization;

namespace QLNT.Pages_HopDong
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopdong = await _context.HopDongs.FindAsync(id);

            if (hopdong != null)
            {
                _context.HopDongs.Remove(hopdong);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}