using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HoaDon
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<HoaDon> HoaDonList { get; set; } = new List<HoaDon>();

        public async Task OnGetAsync()
        {
            HoaDonList = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.Phong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.NguoiThue)
                .Include(h => h.ThanhToans)
                .OrderByDescending(h => h.Nam)
                .ThenByDescending(h => h.Thang)
                .ThenByDescending(h => h.NgayLap)
                .ToListAsync();
        }
    }
}