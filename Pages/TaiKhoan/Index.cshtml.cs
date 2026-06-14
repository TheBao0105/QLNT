using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using Microsoft.AspNetCore.Authorization;

namespace QLNT.Pages_TaiKhoan
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;

        public IndexModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<TaiKhoan> TaiKhoan { get;set; } = default!;

        public async Task OnGetAsync()
        {
            TaiKhoan = await _context.TaiKhoans.ToListAsync();
        }
    }
}
