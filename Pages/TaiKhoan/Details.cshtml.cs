using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_TaiKhoan
{
    public class DetailsModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public DetailsModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public TaiKhoan TaiKhoan { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taikhoan = await _context.TaiKhoans.FirstOrDefaultAsync(m => m.TaiKhoanId == id);

            if (taikhoan is not null)
            {
                TaiKhoan = taikhoan;

                return Page();
            }

            return NotFound();
        }
    }
}
