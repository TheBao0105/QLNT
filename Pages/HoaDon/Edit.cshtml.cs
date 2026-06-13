using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public string TienPhongText { get; set; } = "";
        public string TienDienText { get; set; } = "";
        public string TienNuocText { get; set; } = "";
        public string TienDichVuText { get; set; } = "";
        public string TongTienText { get; set; } = "";

        private const decimal DonGiaDien = 3500;
        private const decimal DonGiaNuoc = 15000;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.Phong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.NguoiThue)
                .Include(h => h.ThanhToans)
                .FirstOrDefaultAsync(h => h.HoaDonId == id.Value);

            if (hoaDon == null)
            {
                return NotFound();
            }

            if (hoaDon.TrangThai == "Đã thanh toán")
            {
                TempData["Success"] = "Hóa đơn đã thanh toán, không nên sửa.";
                return RedirectToPage("./Index");
            }

            HoaDon = hoaDon;
            FormatMoneyToText();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var hoaDonDb = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.Phong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.NguoiThue)
                .Include(h => h.ThanhToans)
                .FirstOrDefaultAsync(h => h.HoaDonId == HoaDon.HoaDonId);

            if (hoaDonDb == null)
            {
                return NotFound();
            }

            if (hoaDonDb.TrangThai == "Đã thanh toán")
            {
                TempData["Success"] = "Hóa đơn đã thanh toán, không nên sửa.";
                return RedirectToPage("./Index");
            }

            if (HoaDon.Thang < 1 || HoaDon.Thang > 12)
            {
                ModelState.AddModelError("HoaDon.Thang", "Tháng phải từ 1 đến 12.");
            }

            if (HoaDon.Nam < 2000)
            {
                ModelState.AddModelError("HoaDon.Nam", "Năm không hợp lệ.");
            }

            if (HoaDon.ChiSoDienMoi < HoaDon.ChiSoDienCu)
            {
                ModelState.AddModelError("HoaDon.ChiSoDienMoi", "Chỉ số điện mới phải lớn hơn hoặc bằng chỉ số điện cũ.");
            }

            if (HoaDon.ChiSoNuocMoi < HoaDon.ChiSoNuocCu)
            {
                ModelState.AddModelError("HoaDon.ChiSoNuocMoi", "Chỉ số nước mới phải lớn hơn hoặc bằng chỉ số nước cũ.");
            }

            ModelState.Remove("HoaDon.HopDong");
            ModelState.Remove("HoaDon.ThanhToans");
            ModelState.Remove("HoaDon.TenPhong");
            ModelState.Remove("HoaDon.TienPhong");
            ModelState.Remove("HoaDon.DonGiaDien");
            ModelState.Remove("HoaDon.SoDienTieuThu");
            ModelState.Remove("HoaDon.TienDien");
            ModelState.Remove("HoaDon.DonGiaNuoc");
            ModelState.Remove("HoaDon.SoNuocTieuThu");
            ModelState.Remove("HoaDon.TienNuoc");
            ModelState.Remove("HoaDon.TienDichVu");
            ModelState.Remove("HoaDon.TongTien");
            ModelState.Remove("HoaDon.TrangThai");

            if (!ModelState.IsValid)
            {
                HoaDon.HopDong = hoaDonDb.HopDong;
                HoaDon.TienPhong = hoaDonDb.TienPhong;
                HoaDon.TienDien = hoaDonDb.TienDien;
                HoaDon.TienNuoc = hoaDonDb.TienNuoc;
                HoaDon.TienDichVu = hoaDonDb.TienDichVu;
                HoaDon.TongTien = hoaDonDb.TongTien;
                FormatMoneyToText();
                return Page();
            }

            hoaDonDb.Thang = HoaDon.Thang;
            hoaDonDb.Nam = HoaDon.Nam;
            hoaDonDb.NgayLap = HoaDon.NgayLap == default ? hoaDonDb.NgayLap : HoaDon.NgayLap;
            hoaDonDb.HanThanhToan = HoaDon.HanThanhToan;
            hoaDonDb.ChiSoDienCu = HoaDon.ChiSoDienCu;
            hoaDonDb.ChiSoDienMoi = HoaDon.ChiSoDienMoi;
            hoaDonDb.ChiSoNuocCu = HoaDon.ChiSoNuocCu;
            hoaDonDb.ChiSoNuocMoi = HoaDon.ChiSoNuocMoi;
            hoaDonDb.GhiChu = HoaDon.GhiChu;

            hoaDonDb.DonGiaDien = DonGiaDien;
            hoaDonDb.DonGiaNuoc = DonGiaNuoc;

            hoaDonDb.SoDienTieuThu = hoaDonDb.ChiSoDienMoi - hoaDonDb.ChiSoDienCu;
            hoaDonDb.TienDien = hoaDonDb.SoDienTieuThu * hoaDonDb.DonGiaDien;

            hoaDonDb.SoNuocTieuThu = hoaDonDb.ChiSoNuocMoi - hoaDonDb.ChiSoNuocCu;
            hoaDonDb.TienNuoc = hoaDonDb.SoNuocTieuThu * hoaDonDb.DonGiaNuoc;

            hoaDonDb.TongTien =
                hoaDonDb.TienPhong +
                hoaDonDb.TienDien +
                hoaDonDb.TienNuoc +
                hoaDonDb.TienDichVu;

            hoaDonDb.TrangThai = "Chưa thanh toán";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật hóa đơn thành công.";

            return RedirectToPage("./Index");
        }

        private void FormatMoneyToText()
        {
            TienPhongText = FormatVnd(HoaDon.TienPhong);
            TienDienText = FormatVnd(HoaDon.TienDien);
            TienNuocText = FormatVnd(HoaDon.TienNuoc);
            TienDichVuText = FormatVnd(HoaDon.TienDichVu);
            TongTienText = FormatVnd(HoaDon.TongTien);
        }

        public string FormatVnd(decimal value)
        {
            return value.ToString("N0", new CultureInfo("vi-VN")) + " VNĐ";
        }
    }
}