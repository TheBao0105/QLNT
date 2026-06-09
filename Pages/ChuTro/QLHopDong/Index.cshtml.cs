using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HopDong
{
    public class IndexModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;
        private const int PageSize = 5;

        public IndexModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public PaginatedList<HopDong> HopDong { get; set; } = default!;

        public async Task OnGetAsync(int? pageIndex)
        {
            var query = _context.HopDongs
                .Include(h => h.NguoiThue)
                .Include(h => h.Phong)
                .OrderBy(h => h.NgayBatDau);

            HopDong = await PaginatedList<HopDong>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1, PageSize);
        }
    }
}
