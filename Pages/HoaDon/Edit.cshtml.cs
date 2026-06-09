using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.Globalization;

namespace QLNT.Pages_HoaDon
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HoaDon HoaDon { get; set; } = default!;

        [BindProperty]
        public string TienPhongText { get; set; } = "";

        [BindProperty]
        public string TienDienText { get; set; } = "";

        [BindProperty]
        public string TienNuocText { get; set; } = "";

        [BindProperty]
        public string TienDichVuText { get; set; } = "";

        [BindProperty]
        public string TongTienText { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.HoaDons
                .FirstOrDefaultAsync(m => m.HoaDonId == id);

            if (hoadon == null)
            {
                return NotFound();
            }

            HoaDon = hoadon;

            FormatMoneyToText();
            LoadSelectLists();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("HoaDon.TienPhong");
            ModelState.Remove("HoaDon.TienDien");
            ModelState.Remove("HoaDon.TienNuoc");
            ModelState.Remove("HoaDon.TienDichVu");
            ModelState.Remove("HoaDon.TongTien");

            if (!TryParseVnd(TienPhongText, out var tienPhong))
            {
                ModelState.AddModelError(nameof(TienPhongText), "Tiền phòng không đúng định dạng.");
            }

            if (!TryParseVnd(TienDienText, out var tienDien))
            {
                ModelState.AddModelError(nameof(TienDienText), "Tiền điện không đúng định dạng.");
            }

            if (!TryParseVnd(TienNuocText, out var tienNuoc))
            {
                ModelState.AddModelError(nameof(TienNuocText), "Tiền nước không đúng định dạng.");
            }

            if (!TryParseVnd(TienDichVuText, out var tienDichVu))
            {
                ModelState.AddModelError(nameof(TienDichVuText), "Tiền dịch vụ không đúng định dạng.");
            }

            if (tienPhong < 0)
            {
                ModelState.AddModelError(nameof(TienPhongText), "Tiền phòng không được nhỏ hơn 0.");
            }

            if (tienDien < 0)
            {
                ModelState.AddModelError(nameof(TienDienText), "Tiền điện không được nhỏ hơn 0.");
            }

            if (tienNuoc < 0)
            {
                ModelState.AddModelError(nameof(TienNuocText), "Tiền nước không được nhỏ hơn 0.");
            }

            if (tienDichVu < 0)
            {
                ModelState.AddModelError(nameof(TienDichVuText), "Tiền dịch vụ không được nhỏ hơn 0.");
            }

            HoaDon.TienPhong = tienPhong;
            HoaDon.TienDien = tienDien;
            HoaDon.TienNuoc = tienNuoc;
            HoaDon.TienDichVu = tienDichVu;
            HoaDon.TongTien = tienPhong + tienDien + tienNuoc + tienDichVu;

            TongTienText = FormatVnd(HoaDon.TongTien);

            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

            _context.Attach(HoaDon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoaDonExists(HoaDon.HoaDonId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private void LoadSelectLists()
        {
            ViewData["HopDongId"] = new SelectList(
                _context.HopDongs,
                "HopDongId",
                "HopDongId",
                HoaDon.HopDongId
            );
        }

        private void FormatMoneyToText()
        {
            TienPhongText = FormatVnd(HoaDon.TienPhong);
            TienDienText = FormatVnd(HoaDon.TienDien);
            TienNuocText = FormatVnd(HoaDon.TienNuoc);
            TienDichVuText = FormatVnd(HoaDon.TienDichVu);
            TongTienText = FormatVnd(HoaDon.TongTien);
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

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.HoaDonId == id);
        }
    }
}