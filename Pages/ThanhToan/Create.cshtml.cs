using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_ThanhToan
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
        ViewData["HoaDonId"] = new SelectList(_context.HoaDons, "HoaDonId", "HoaDonId");
            return Page();
        }

        [BindProperty]
        public ThanhToan ThanhToan { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ThanhToans.Add(ThanhToan);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
