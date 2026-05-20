using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_Phong
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
            return Page();
        }

        [BindProperty]
        public Phong Phong { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Phongs.Add(Phong);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
