using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_DichVu
{
    public class DeleteModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public DeleteModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DichVu DichVu { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichvu = await _context.DichVus.FirstOrDefaultAsync(m => m.DichVuId == id);

            if (dichvu is not null)
            {
                DichVu = dichvu;

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

            var dichvu = await _context.DichVus.FindAsync(id);
            if (dichvu != null)
            {
                DichVu = dichvu;
                _context.DichVus.Remove(DichVu);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
