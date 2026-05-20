using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HoaDon
{
    public class CreateModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public CreateModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["HopDongId"] = new SelectList(_context.HopDongs, "HopDongId", "HopDongId");
            return Page();
        }

        [BindProperty]
        public HoaDon HoaDon { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.HoaDons.Add(HoaDon);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
