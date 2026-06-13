using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HopDong
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<HopDong> HopDong { get; set; } = new List<HopDong>();

        public int? CurrentIdFilter { get; set; }
        public string? StatusFilter { get; set; }

        public async Task OnGetAsync(string? StatusFilter)
        {
            this.StatusFilter = StatusFilter;

            var today = DateTime.Today;

            var query = _context.HopDongs
                .Include(h => h.NguoiThue)
                .Include(h => h.Phong)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(StatusFilter))
            {
                if (StatusFilter == "Chưa bắt đầu")
                {
                    query = query.Where(h => h.NgayBatDau.Date > today);
                }
                else if (StatusFilter == "Hết hạn")
                {
                    query = query.Where(h => h.NgayKetThuc.HasValue && h.NgayKetThuc.Value.Date < today);
                }
                else if (StatusFilter == "Còn hạn")
                {
                    query = query.Where(h => h.NgayBatDau.Date <= today 
                                          && (!h.NgayKetThuc.HasValue || h.NgayKetThuc.Value.Date >= today));
                }
            }
            HopDong = await query
                .OrderByDescending(h => h.NgayBatDau)
                .AsNoTracking()
                .ToListAsync();
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