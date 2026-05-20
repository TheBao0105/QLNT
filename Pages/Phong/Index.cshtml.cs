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
    public class IndexModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public IndexModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Phong> Phong { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Phong = await _context.Phongs.ToListAsync();
        }
    }
}
