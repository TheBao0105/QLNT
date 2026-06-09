using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages.ChuTro.TaoNhaTro
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NhaTro NhaTro { get; set; } = new();

        public string MaTroMoi { get; set; } = "";

        public IActionResult OnPost()
        {
            NhaTro.MaTro = "TRO" + DateTime.Now.ToString("yyyyMMddHHmmss");

            _context.NhaTros.Add(NhaTro);
            _context.SaveChanges();

            return RedirectToPage("/ChuTro/QLNhaTro/Index", new { id = NhaTro.NhaTroId });
        }
    }
}