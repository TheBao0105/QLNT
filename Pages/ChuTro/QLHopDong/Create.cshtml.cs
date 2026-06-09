using System;
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
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HopDong HopDong { get; set; } = default!;

        public IActionResult OnGet()
        {
            LoadSelectList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadSelectList();
                return Page();
            }

            HopDong.MaHopDong = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");

            _context.HopDongs.Add(HopDong);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void LoadSelectList()
        {
            ViewData["NguoiThueId"] = new SelectList(
                _context.NguoiThues,
                "NguoiThueId",
                "HoTen"
            );

            ViewData["PhongId"] = new SelectList(
                _context.Phongs,
                "PhongId",
                "TenPhong"
            );
        }
    }
}