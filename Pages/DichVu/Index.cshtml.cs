using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_DichVu
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<DichVu> DichVu { get; set; } = default!;

        public async Task OnGetAsync()
        {
            DichVu = await _context.DichVus
                .OrderBy(d => d.TenDichVu)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}