using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_DichVu
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
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

            if (dichvu == null)
            {
                return NotFound();
            }

            DichVu = dichvu;
            return Page();
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
                _context.DichVus.Remove(dichvu);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}