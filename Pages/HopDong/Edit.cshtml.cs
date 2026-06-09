using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace QLNT.Pages_HopDong
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HopDong HopDong { get; set; } = new HopDong();

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập tiền cọc")]
        public string TienCocText { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập giá thuê")]
        public string GiaThueText { get; set; } = "";

        [BindProperty]
        public string TrangThaiText { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.HopDongId == id);

            if (hopDong == null)
            {
                return NotFound();
            }

            HopDong = hopDong;

            HopDong.TrangThai = TinhTrangThaiHopDong(HopDong.NgayBatDau, HopDong.NgayKetThuc);
            TrangThaiText = HopDong.TrangThai;

            TienCocText = FormatVnd(HopDong.TienCoc);
            GiaThueText = FormatVnd(HopDong.GiaThue);

            LoadSelectLists();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("HopDong.TienCoc");
            ModelState.Remove("HopDong.GiaThue");
            ModelState.Remove("HopDong.TrangThai");

            if (!TryParseVnd(TienCocText, out decimal tienCoc))
            {
                ModelState.AddModelError("TienCocText", "Tiền cọc không hợp lệ");
            }

            if (!TryParseVnd(GiaThueText, out decimal giaThue))
            {
                ModelState.AddModelError("GiaThueText", "Giá thuê không hợp lệ");
            }

            HopDong.TienCoc = tienCoc;
            HopDong.GiaThue = giaThue;
            HopDong.TrangThai = TinhTrangThaiHopDong(HopDong.NgayBatDau, HopDong.NgayKetThuc);
            TrangThaiText = HopDong.TrangThai;

            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

            _context.Attach(HopDong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HopDongExists(HopDong.HopDongId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private string TinhTrangThaiHopDong(DateTime ngayBatDau, DateTime? ngayKetThuc)
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

        private string FormatVnd(decimal value)
        {
            return value.ToString("N0", new CultureInfo("vi-VN"));
        }

        private bool TryParseVnd(string? text, out decimal value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var cleanText = text
                .Replace("VNĐ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("VND", "", StringComparison.OrdinalIgnoreCase)
                .Replace("₫", "")
                .Replace(" ", "")
                .Replace(".", "")
                .Replace(",", "")
                .Trim();

            return decimal.TryParse(cleanText, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private void LoadSelectLists()
        {
            ViewData["PhongId"] = new SelectList(
                _context.Phongs.AsNoTracking(),
                "PhongId",
                "TenPhong",
                HopDong.PhongId
            );

            ViewData["NguoiThueId"] = new SelectList(
                _context.NguoiThues.AsNoTracking(),
                "NguoiThueId",
                "HoTen",
                HopDong.NguoiThueId
            );
        }

        private bool HopDongExists(int id)
        {
            return _context.HopDongs.Any(e => e.HopDongId == id);
        }
    }
}