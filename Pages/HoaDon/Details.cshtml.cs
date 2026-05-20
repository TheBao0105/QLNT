using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HoaDon
{
    public class DetailsModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public DetailsModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public HoaDon HoaDon { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.HoaDons.FirstOrDefaultAsync(m => m.HoaDonId == id);

            if (hoadon is not null)
            {
                HoaDon = hoadon;

                return Page();
            }

            return NotFound();
        }
    }
}
