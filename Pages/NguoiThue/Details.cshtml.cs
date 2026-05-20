using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_NguoiThue
{
    public class DetailsModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public DetailsModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public NguoiThue NguoiThue { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoithue = await _context.NguoiThues.FirstOrDefaultAsync(m => m.NguoiThueId == id);

            if (nguoithue is not null)
            {
                NguoiThue = nguoithue;

                return Page();
            }

            return NotFound();
        }
    }
}
