using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_Phong
{
    public class DetailsModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public DetailsModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public Phong Phong { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs.FirstOrDefaultAsync(m => m.PhongId == id);

            if (phong is not null)
            {
                Phong = phong;

                return Page();
            }

            return NotFound();
        }
    }
}
