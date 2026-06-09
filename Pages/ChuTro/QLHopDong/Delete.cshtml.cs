using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HopDong
{
    public class DeleteModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public DeleteModel(QLNT.Data.AppDbContext context)
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

            var hopdong = await _context.HopDongs.FirstOrDefaultAsync(m => m.HopDongId == id);

            if (hopdong is not null)
            {
                HopDong = hopdong;

                return Page();
            }

            return NotFound();
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
                HopDong = hopdong;
                _context.HopDongs.Remove(HopDong);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
