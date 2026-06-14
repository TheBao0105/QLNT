using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace QLNT.Pages_HopDong
{
        [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HopDong HopDong { get; set; } = new HopDong();

        public string HienThiTienCoc { get; set; } = "";

        public string HienThiGiaThue { get; set; } = "";

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

            HienThiTienCoc = FormatVnd(HopDong.TienCoc);
            HienThiGiaThue = FormatVnd(HopDong.GiaThue);

            LoadSelectLists();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("HopDong.TienCoc");
            ModelState.Remove("HopDong.GiaThue");
            ModelState.Remove("HopDong.TrangThai");
            ModelState.Remove("HopDong.Phong");
            ModelState.Remove("HopDong.NguoiThue");

            var tienCocInput = Request.Form["TienCoc"].ToString();
            var giaThueInput = Request.Form["GiaThue"].ToString();

            HienThiTienCoc = tienCocInput;
            HienThiGiaThue = giaThueInput;

            if (!TryParseVnd(tienCocInput, out decimal tienCoc))
            {
                ModelState.AddModelError("TienCoc", "Tiền cọc không hợp lệ");
            }

            if (!TryParseVnd(giaThueInput, out decimal giaThue))
            {
                ModelState.AddModelError("GiaThue", "Giá thuê không hợp lệ");
            }

            HopDong.TienCoc = tienCoc;
            HopDong.GiaThue = giaThue;
            HopDong.TrangThai = TinhTrangThaiHopDong(HopDong.NgayBatDau, HopDong.NgayKetThuc);

            if (HopDong.NgayKetThuc.HasValue &&
                HopDong.NgayKetThuc.Value.Date < HopDong.NgayBatDau.Date)
            {
                ModelState.AddModelError("HopDong.NgayKetThuc", "Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

            var hopDongCu = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.HopDongId == HopDong.HopDongId);

            if (hopDongCu == null)
            {
                return NotFound();
            }

            hopDongCu.PhongId = HopDong.PhongId;
            hopDongCu.NguoiThueId = HopDong.NguoiThueId;
            hopDongCu.NgayBatDau = HopDong.NgayBatDau;
            hopDongCu.NgayKetThuc = HopDong.NgayKetThuc;
            hopDongCu.TienCoc = HopDong.TienCoc;
            hopDongCu.GiaThue = HopDong.GiaThue;
            hopDongCu.TrangThai = HopDong.TrangThai;
            hopDongCu.GhiChu = HopDong.GhiChu;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public string TinhTrangThaiHopDong(DateTime ngayBatDau, DateTime? ngayKetThuc)
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

        public string FormatVnd(decimal value)
        {
            return value.ToString("N0", new CultureInfo("vi-VN"));
        }

        private bool TryParseVnd(string? valueFromForm, out decimal value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(valueFromForm))
            {
                return false;
            }

            var cleanValue = valueFromForm
                .Replace("VNĐ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("VND", "", StringComparison.OrdinalIgnoreCase)
                .Replace("₫", "")
                .Replace(" ", "")
                .Replace(".", "")
                .Replace(",", "")
                .Trim();

            return decimal.TryParse(cleanValue, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private void LoadSelectLists()
        {
            ViewData["PhongId"] = new SelectList(
                _context.Phongs.AsNoTracking().OrderBy(p => p.TenPhong),
                "PhongId",
                "TenPhong",
                HopDong.PhongId
            );

            ViewData["NguoiThueId"] = new SelectList(
                _context.NguoiThues.AsNoTracking().OrderBy(n => n.HoTen),
                "NguoiThueId",
                "HoTen",
                HopDong.NguoiThueId
            );
        }
    }
}