using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_HoaDon
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<HoaDon> HoaDonList { get; set; } = new List<HoaDon>();

        [BindProperty]
        public HoaDon HoaDonMoi { get; set; } = new();

        [BindProperty]
        public List<int> DichVuIds { get; set; } = new();
        public IList<HopDong> DanhSachHopDong { get; set; } = new List<HopDong>();
        private const decimal DonGiaDien = 3500;
        private const decimal DonGiaNuoc = 15000;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();

            HoaDonMoi = new HoaDon
            {
                NgayLap = DateTime.Now,
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                DonGiaDien = DonGiaDien,
                DonGiaNuoc = DonGiaNuoc,
                TrangThai = "Chưa thanh toán"
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDataAsync();

            var hopDong = await _context.HopDongs
     .Include(h => h.Phong)
     .Include(h => h.NguoiThue)
     .FirstOrDefaultAsync(h =>
         h.HopDongId == HoaDonMoi.HopDongId &&
         (
             h.TrangThai == "Còn hạn" ||
          h.TrangThai == "Đang thuê" ||
          h.TrangThai == "Đang hiệu lực"
         ));

            if (hopDong == null)
            {
                ModelState.AddModelError("HoaDonMoi.HopDongId", "Chỉ được lập hóa đơn cho phòng đang thuê.");
                return Page();
            }

            if (HoaDonMoi.ChiSoDienMoi < HoaDonMoi.ChiSoDienCu)
            {
                ModelState.AddModelError("HoaDonMoi.ChiSoDienMoi", "Chỉ số điện mới phải lớn hơn hoặc bằng chỉ số điện cũ.");
                return Page();
            }

            if (HoaDonMoi.ChiSoNuocMoi < HoaDonMoi.ChiSoNuocCu)
            {
                ModelState.AddModelError("HoaDonMoi.ChiSoNuocMoi", "Chỉ số nước mới phải lớn hơn hoặc bằng chỉ số nước cũ.");
                return Page();
            }

            var tienDichVu = await _context.DichVus
                .Where(d => DichVuIds.Contains(d.DichVuId) && d.TrangThai == "Đang sử dụng")
                .SumAsync(d => d.DonGia);

            HoaDonMoi.TenPhong = hopDong.Phong.TenPhong;
            HoaDonMoi.TienPhong = hopDong.GiaThue;

            HoaDonMoi.DonGiaDien = DonGiaDien;
            HoaDonMoi.DonGiaNuoc = DonGiaNuoc;

            HoaDonMoi.SoDienTieuThu = HoaDonMoi.ChiSoDienMoi - HoaDonMoi.ChiSoDienCu;
            HoaDonMoi.TienDien = HoaDonMoi.SoDienTieuThu * HoaDonMoi.DonGiaDien;

            HoaDonMoi.SoNuocTieuThu = HoaDonMoi.ChiSoNuocMoi - HoaDonMoi.ChiSoNuocCu;
            HoaDonMoi.TienNuoc = HoaDonMoi.SoNuocTieuThu * HoaDonMoi.DonGiaNuoc;

            HoaDonMoi.TienDichVu = tienDichVu;

            HoaDonMoi.TongTien =
                HoaDonMoi.TienPhong +
                HoaDonMoi.TienDien +
                HoaDonMoi.TienNuoc +
                HoaDonMoi.TienDichVu;

            HoaDonMoi.NgayLap = DateTime.Now;
            HoaDonMoi.TrangThai = "Chưa thanh toán";

            ModelState.Remove("HoaDonMoi.TenPhong");
            ModelState.Remove("HoaDonMoi.TienPhong");
            ModelState.Remove("HoaDonMoi.SoDienTieuThu");
            ModelState.Remove("HoaDonMoi.TienDien");
            ModelState.Remove("HoaDonMoi.DonGiaDien");
            ModelState.Remove("HoaDonMoi.SoNuocTieuThu");
            ModelState.Remove("HoaDonMoi.TienNuoc");
            ModelState.Remove("HoaDonMoi.DonGiaNuoc");
            ModelState.Remove("HoaDonMoi.TienDichVu");
            ModelState.Remove("HoaDonMoi.TongTien");
            ModelState.Remove("HoaDonMoi.TrangThai");
            ModelState.Remove("HoaDonMoi.HopDong");
            ModelState.Remove("HoaDonMoi.ThanhToans");

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            _context.HoaDons.Add(HoaDonMoi);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo hóa đơn thành công.";

            return RedirectToPage("./Index");
        }

        private async Task LoadDataAsync()
        {
            HoaDonList = await _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.Phong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.NguoiThue)
                .Include(h => h.ThanhToans)
                .OrderByDescending(h => h.Nam)
                .ThenByDescending(h => h.Thang)
                .ToListAsync();

            var hopDongsDangThue = await _context.HopDongs
      .Include(h => h.Phong)
      .Include(h => h.NguoiThue)
      .Where(h => h.TrangThai == "Còn hạn" || h.TrangThai == "Đang thuê" || h.TrangThai == "Đang hiệu lực")
      .OrderBy(h => h.Phong.TenPhong)
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
      .ToListAsync();

            ViewData["HopDongId"] = new SelectList(
                hopDongsDangThue,
                "HopDongId",
                "TenHienThi"
            );

            var dichVus = await _context.DichVus
                .Where(d => d.TrangThai == "Đang sử dụng")
                .OrderBy(d => d.TenDichVu)
                .ToListAsync();

            ViewData["DichVus"] = dichVus;
        }
    }
}