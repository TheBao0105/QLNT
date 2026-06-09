using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLNT;
using QLNT.Data;
using System.Globalization;

namespace QLNT.Pages_Phong
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Phong Phong { get; set; } = new Phong();

        [BindProperty]
        public string GiaPhongText { get; set; } = "";

        public IActionResult OnGet()
        {
            Phong.TrangThai = "Trống";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Phong.GiaPhong");

            if (!TryParseVnd(GiaPhongText, out var giaPhong))
            {
                ModelState.AddModelError(nameof(GiaPhongText), "Giá phòng không đúng định dạng.");
            }

            if (giaPhong < 0)
            {
                ModelState.AddModelError(nameof(GiaPhongText), "Giá phòng không được nhỏ hơn 0.");
            }

            Phong.GiaPhong = giaPhong;

            if (string.IsNullOrWhiteSpace(Phong.TrangThai))
            {
                Phong.TrangThai = "Trống";
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Phongs.Add(Phong);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool TryParseVnd(string? input, out decimal value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(input))
            {
                return true;
            }

            var clean = input
                .Replace(".", "")
                .Replace(",", "")
                .Replace("VNĐ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("VND", "", StringComparison.OrdinalIgnoreCase)
                .Replace("đ", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            return decimal.TryParse(
                clean,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value
            );
        }
    }
}