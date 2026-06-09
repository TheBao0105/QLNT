using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.Globalization;

namespace QLNT.Pages_Phong
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Phong Phong { get; set; } = default!;

        [BindProperty]
        public string GiaPhongText { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs.FirstOrDefaultAsync(m => m.PhongId == id);

            if (phong == null)
            {
                return NotFound();
            }

            Phong = phong;
            GiaPhongText = FormatVnd(Phong.GiaPhong);

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
                GiaPhongText = FormatVnd(Phong.GiaPhong);
                return Page();
            }

            _context.Attach(Phong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhongExists(Phong.PhongId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private string FormatVnd(decimal value)
        {
            return value.ToString("N0", new CultureInfo("vi-VN"));
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

        private bool PhongExists(int id)
        {
            return _context.Phongs.Any(e => e.PhongId == id);
        }
    }
}