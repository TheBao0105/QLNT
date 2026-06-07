using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_DichVu
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
            DichVu = new DichVu();
            {
                DichVu.TrangThai = "Đang sử dụng";
            }
            return Page();
        }

        [BindProperty]
        public DichVu DichVu { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrWhiteSpace(DichVu.TrangThai))
            {
                DichVu.TrangThai = "Đang sử dụng";
            }

            _context.DichVus.Add(DichVu);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
