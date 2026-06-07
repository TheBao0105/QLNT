using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HoaDon
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HoaDon HoaDon { get; set; } = default!;

        [BindProperty]
        public List<int> DichVuIds { get; set; } = new();

        private const decimal DonGiaDien = 3500;
        private const decimal DonGiaNuoc = 15000;

        public async Task<IActionResult> OnGetAsync()

        {
            await LoadSelectListAsync();

            HoaDon = new HoaDon
            {
                NgayLap = DateTime.Now,
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                TrangThai = "Chưa thanh toán"
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadSelectListAsync();

            var hopDong = await _context.HopDongs
                .Include(h => h.Phong)
                .FirstOrDefaultAsync(h =>
                    h.HopDongId == HoaDon.HopDongId &&
                    h.TrangThai == "Đang hiệu lực");

            if (hopDong == null)
            {
                ModelState.AddModelError("HoaDon.HopDongId", "Chỉ được lập hóa đơn cho phòng đang thuê.");
                return Page();
            }

            if (HoaDon.ChiSoDienMoi < HoaDon.ChiSoDienCu)
            {
                ModelState.AddModelError("HoaDon.ChiSoDienMoi", "Chỉ số điện mới phải lớn hơn hoặc bằng chỉ số điện cũ.");
                return Page();
            }

            if (HoaDon.ChiSoNuocMoi < HoaDon.ChiSoNuocCu)
            {
                ModelState.AddModelError("HoaDon.ChiSoNuocMoi", "Chỉ số nước mới phải lớn hơn hoặc bằng chỉ số nước cũ.");
                return Page();
            }

            var tienDichVu = await _context.DichVus
                .Where(d => DichVuIds.Contains(d.DichVuId) && d.TrangThai == "Đang sử dụng")
                .SumAsync(d => d.DonGia);

            HoaDon.TenPhong = hopDong.Phong.TenPhong;
            HoaDon.TienPhong = hopDong.GiaThue;

            HoaDon.DonGiaDien = DonGiaDien;
            HoaDon.DonGiaNuoc = DonGiaNuoc;

            HoaDon.SoDienTieuThu = HoaDon.ChiSoDienMoi - HoaDon.ChiSoDienCu;
            HoaDon.TienDien = HoaDon.SoDienTieuThu * HoaDon.DonGiaDien;

            HoaDon.SoNuocTieuThu = HoaDon.ChiSoNuocMoi - HoaDon.ChiSoNuocCu;
            HoaDon.TienNuoc = HoaDon.SoNuocTieuThu * HoaDon.DonGiaNuoc;

            HoaDon.TienDichVu = tienDichVu;

            HoaDon.TongTien =
                HoaDon.TienPhong +
                HoaDon.TienDien +
                HoaDon.TienNuoc +
                HoaDon.TienDichVu;

            if (HoaDon.NgayLap == default)
            {
                HoaDon.NgayLap = DateTime.Now;
            }

            HoaDon.TrangThai = "Chưa thanh toán";

            // Xóa lỗi validate của các field tự tính hoặc navigation property
            ModelState.Remove("HoaDon.TenPhong");
            ModelState.Remove("HoaDon.TienPhong");
            ModelState.Remove("HoaDon.TienDien");
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

            return RedirectToPage("./Index");
        }
        private async Task LoadSelectListAsync()
        {
            var hopDongsDangThue = await _context.HopDongs
                .Include(h => h.Phong)
                .Where(h => h.TrangThai == "Đang hiệu lực")
                .OrderBy(h => h.Phong.TenPhong)
                .Select(h => new
                {
                    h.HopDongId,
                    TenHienThi = h.Phong.TenPhong + " - " + h.GiaThue.ToString("N0") + " VNĐ",
                    h.GiaThue
                })
                .ToListAsync();

            ViewData["HopDongId"] = new SelectList(hopDongsDangThue, "HopDongId", "TenHienThi");
            ViewData["HopDongGiaThue"] = hopDongsDangThue.ToDictionary(h => h.HopDongId, h => h.GiaThue);

            var dichVus = await _context.DichVus
                .Where(d => d.TrangThai == "Đang sử dụng")
                .OrderBy(d => d.TenDichVu)
                .ToListAsync();

            ViewData["DichVus"] = dichVus;
            ViewData["DichVuDonGia"] = dichVus.ToDictionary(d => d.DichVuId, d => d.DonGia);
        }
    }
}