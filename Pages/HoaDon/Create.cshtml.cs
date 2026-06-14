using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using Microsoft.AspNetCore.Authorization;

namespace QLNT.Pages_HoaDon
{
        [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HoaDon HoaDon { get; set; } = new();

        [BindProperty]
        public List<int> DichVuIds { get; set; } = new();

        private const decimal DonGiaDien = 3500;
        private const decimal DonGiaNuoc = 15000;

        public async Task<IActionResult> OnGetAsync()
        {
            HoaDon = new HoaDon
            {
                NgayLap = DateTime.Now,
                HanThanhToan = DateTime.Now.AddDays(7),
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                DonGiaDien = DonGiaDien,
                DonGiaNuoc = DonGiaNuoc,
                TrangThai = "Chưa thanh toán"
            };

            await LoadDataAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDataAsync();

            var hopDong = await _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .FirstOrDefaultAsync(h =>
                    h.HopDongId == HoaDon.HopDongId &&
                    (
                        h.TrangThai == "Còn hạn" ||
                        h.TrangThai == "Đang thuê" ||
                        h.TrangThai == "Đang hiệu lực"
                    ));

            if (hopDong == null)
            {
                ModelState.AddModelError("HoaDon.HopDongId", "Chỉ được lập hóa đơn cho phòng đang thuê.");
                return Page();
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

            var daCoHoaDon = await _context.HoaDons.AnyAsync(h =>
                h.HopDongId == HoaDon.HopDongId &&
                h.Thang == HoaDon.Thang &&
                h.Nam == HoaDon.Nam);

            if (daCoHoaDon)
            {
                ModelState.AddModelError("HoaDon.Thang", "Hợp đồng này đã có hóa đơn trong tháng/năm đã chọn.");
            }

            HoaDon.TenPhong = hopDong.Phong?.TenPhong;
            HoaDon.TienPhong = hopDong.GiaThue;

            HoaDon.DonGiaDien = DonGiaDien;
            HoaDon.DonGiaNuoc = DonGiaNuoc;

            HoaDon.SoDienTieuThu = HoaDon.ChiSoDienMoi - HoaDon.ChiSoDienCu;
            HoaDon.TienDien = HoaDon.SoDienTieuThu * HoaDon.DonGiaDien;

            HoaDon.SoNuocTieuThu = HoaDon.ChiSoNuocMoi - HoaDon.ChiSoNuocCu;
            HoaDon.TienNuoc = HoaDon.SoNuocTieuThu * HoaDon.DonGiaNuoc;

            HoaDon.TienDichVu = await _context.DichVus
                .Where(d => DichVuIds.Contains(d.DichVuId) && d.TrangThai == "Đang sử dụng")
                .SumAsync(d => d.DonGia);

            HoaDon.TongTien =
                HoaDon.TienPhong +
                HoaDon.TienDien +
                HoaDon.TienNuoc +
                HoaDon.TienDichVu;

            if (HoaDon.NgayLap == default)
            {
                HoaDon.NgayLap = DateTime.Now;
            }

            if (HoaDon.HanThanhToan == null)
            {
                HoaDon.HanThanhToan = HoaDon.NgayLap.AddDays(7);
            }

            HoaDon.TrangThai = "Chưa thanh toán";

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
            ModelState.Remove("HoaDon.HopDong");
            ModelState.Remove("HoaDon.ThanhToans");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.HoaDons.Add(HoaDon);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo hóa đơn thành công.";

            return RedirectToPage("./Index");
        }

        private async Task LoadDataAsync()
        {
            var hopDongsDangThueRaw = await _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .Where(h =>
                    h.TrangThai == "Còn hạn" ||
                    h.TrangThai == "Đang thuê" ||
                    h.TrangThai == "Đang hiệu lực")
                .OrderBy(h => h.Phong.TenPhong)
                .ToListAsync();

            var hopDongsDangThue = hopDongsDangThueRaw
                .Select(h => new
                {
                    h.HopDongId,
                    TenHienThi =
                        h.Phong.TenPhong
                        + " - "
                        + h.NguoiThue.HoTen
                        + " - "
                        + h.GiaThue.ToString("N0")
                        + " VNĐ"
                })
                .ToList();

            ViewData["HopDongId"] = new SelectList(
                hopDongsDangThue,
                "HopDongId",
                "TenHienThi",
                HoaDon.HopDongId
            );

            var dichVus = await _context.DichVus
                .Where(d => d.TrangThai == "Đang sử dụng")
                .OrderBy(d => d.TenDichVu)
                .ToListAsync();

            ViewData["DichVus"] = dichVus;
        }
    }
}