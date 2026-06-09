using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_DichVu
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DichVu DichVu { get; set; } = new DichVu();

        public IActionResult OnGet()
        {
            DichVu.TrangThai = "Đang sử dụng";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (DichVu.DonGia < 0)
            {
                ModelState.AddModelError("DichVu.DonGia", "Đơn giá không được nhỏ hơn 0.");
            }

            if (string.IsNullOrWhiteSpace(DichVu.TrangThai))
            {
                DichVu.TrangThai = "Đang sử dụng";
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DichVus.Add(DichVu);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}