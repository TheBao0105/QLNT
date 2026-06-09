using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages.NguoiThue.ThamGiaTro
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string MaTro { get; set; } = "";

        public string ThongBao { get; set; } = "";

        public IActionResult OnPost()
        {
            var nhaTro = _context.NhaTros
                .FirstOrDefault(x => x.MaTro == MaTro);

            if (nhaTro == null)
            {
                ThongBao = "Mã trọ không tồn tại";
                return Page();
            }

            return RedirectToPage("/NguoiThue/Index");
        }
    }
}