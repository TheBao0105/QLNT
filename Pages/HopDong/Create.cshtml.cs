using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HopDong
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
        ViewData["NguoiThueId"] = new SelectList(_context.NguoiThues, "NguoiThueId", "HoTen");
        ViewData["PhongId"] = new SelectList(_context.Phongs, "PhongId", "TenPhong");
            return Page();
        }

        [BindProperty]
        public HopDong HopDong { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.HopDongs.Add(HopDong);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
