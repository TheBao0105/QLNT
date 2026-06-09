using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages.ChuTro.QuanLyNhaTro
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public NhaTro? NhaTro { get; set; }

        public IActionResult OnGet(int id)
        {
            NhaTro = _context.NhaTros.FirstOrDefault(x => x.NhaTroId == id);

            if (NhaTro == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}