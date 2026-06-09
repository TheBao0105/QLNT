using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HopDong
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private const int PageSize = 5;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public PaginatedList<HopDong> HopDong { get; set; } = default!;

        public async Task OnGetAsync(int? pageIndex)
        {
            var query = _context.HopDongs
                .Include(h => h.NguoiThue)
                .Include(h => h.Phong)
                .OrderByDescending(h => h.NgayBatDau);

            HopDong = await PaginatedList<HopDong>.CreateAsync(
                query.AsNoTracking(),
                pageIndex ?? 1,
                PageSize
            );
        }

        public string TinhTrangThaiHopDong(DateTime ngayBatDau, DateTime? ngayKetThuc)
        {
            var today = DateTime.Today;

            if (ngayBatDau.Date > today)
            {
                return "Chưa bắt đầu";
            }

            if (ngayKetThuc.HasValue && ngayKetThuc.Value.Date < today)
            {
                return "Hết hạn";
            }

            return "Còn hạn";
        }
    }
}