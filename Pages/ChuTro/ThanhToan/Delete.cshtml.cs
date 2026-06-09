using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_ThanhToan
{
    public class DeleteModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public DeleteModel(QLNT.Data.AppDbContext context)
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

            var thanhtoan = await _context.ThanhToans.FirstOrDefaultAsync(m => m.ThanhToanId == id);

            if (thanhtoan is not null)
            {
                ThanhToan = thanhtoan;

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

            var thanhtoan = await _context.ThanhToans.FindAsync(id);
            if (thanhtoan != null)
            {
                ThanhToan = thanhtoan;
                _context.ThanhToans.Remove(ThanhToan);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
