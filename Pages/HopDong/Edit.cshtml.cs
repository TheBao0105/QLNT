using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HopDong
{
    public class EditModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public EditModel(QLNT.Data.AppDbContext context)
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

            var hopdong =  await _context.HopDongs.FirstOrDefaultAsync(m => m.HopDongId == id);
            if (hopdong == null)
            {
                return NotFound();
            }
            HopDong = hopdong;
           ViewData["NguoiThueId"] = new SelectList(_context.NguoiThues, "NguoiThueId", "HoTen");
           ViewData["PhongId"] = new SelectList(_context.Phongs, "PhongId", "TenPhong");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(HopDong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HopDongExists(HopDong.HopDongId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool HopDongExists(int id)
        {
            return _context.HopDongs.Any(e => e.HopDongId == id);
        }
    }
}
