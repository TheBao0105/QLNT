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
    public class IndexModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;
        private const int PageSize = 5;

        public IndexModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public string CurrentFilter { get; set; } = string.Empty;
        public PaginatedList<NguoiThue> NguoiThue { get; set; } = default!;

        public async Task OnGetAsync(string currentFilter, string searchString, int? pageIndex)
        {
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString ?? string.Empty;

            var query = from n in _context.NguoiThues
                        select n;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n =>
                    EF.Functions.Like(n.HoTen, $"%{searchString}%") ||
                    EF.Functions.Like(n.SoDienThoai ?? string.Empty, $"%{searchString}%") ||
                    EF.Functions.Like(n.CCCD ?? string.Empty, $"%{searchString}%"));
            }

            query = query.OrderBy(n => n.HoTen);
            NguoiThue = await PaginatedList<NguoiThue>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1, PageSize);
        }
    }
}
