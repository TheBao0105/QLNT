using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HoaDon
{
    public class DeleteModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public DeleteModel(QLNT.Data.AppDbContext context)
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

            var hoadon = await _context.HoaDons.FirstOrDefaultAsync(m => m.HoaDonId == id);

            if (hoadon is not null)
            {
                HoaDon = hoadon;

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

            var hoadon = await _context.HoaDons.FindAsync(id);
            if (hoadon != null)
            {
                HoaDon = hoadon;
                _context.HoaDons.Remove(HoaDon);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
