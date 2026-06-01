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
        private const int PageSize = 5;

        public IndexModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        public string CurrentFilter { get; set; } = string.Empty;
        public string? StatusFilter { get; set; }
        public PaginatedList<Phong> Phong { get; set; } = default!;

        public async Task OnGetAsync(string currentFilter, string searchString, string? statusFilter, int? pageIndex)
        {
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;
            StatusFilter = statusFilter;

            IQueryable<Phong> phongIQ = from p in _context.Phongs
                                        select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                phongIQ = phongIQ.Where(p => p.TenPhong.Contains(searchString) || (p.TrangThai ?? string.Empty).Contains(searchString));
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                phongIQ = phongIQ.Where(p => (p.TrangThai ?? string.Empty) == StatusFilter);
            }

            phongIQ = phongIQ.OrderBy(p => p.TenPhong);
            Phong = await PaginatedList<Phong>.CreateAsync(phongIQ.AsNoTracking(), pageIndex ?? 1, PageSize);
        }
    }
}
