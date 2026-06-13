using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.ViewModels;

namespace QLNT.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public DashboardViewModel Dashboard { get; set; } = new();
        // Thêm 2 dòng này vào ngay dưới biến Dashboard của bạn để hết lỗi đỏ ChartData
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<decimal> ChartData { get; set; } = new List<decimal>();

        [BindProperty(SupportsGet = true)]
        public int SelectedMonth { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedYear { get; set; }

        public async Task OnGetAsync()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            Dashboard = new DashboardViewModel
            {
                TongPhong = await _context.Phongs.CountAsync(),

                PhongTrong = await _context.Phongs
                    .CountAsync(p => p.TrangThai == "Trống"),

                PhongDangThue = await _context.Phongs
                    .CountAsync(p => p.TrangThai == "Đã thuê"),

                TongNguoiThue = await _context.NguoiThues.CountAsync(),

                TongHopDong = await _context.HopDongs.CountAsync(),

                HopDongDangHieuLuc = await _context.HopDongs
                    .CountAsync(h => h.TrangThai == "Còn hạn"),

                TongHoaDon = await _context.HoaDons.CountAsync(),

                HoaDonChuaThanhToan = await _context.HoaDons
                    .CountAsync(h => h.TrangThai == "Chưa thanh toán"
                                  || h.TrangThai == "Thanh toán một phần"),

                HoaDonQuaHan = await _context.HoaDons
                    .CountAsync(h => h.HanThanhToan < today
                                  && h.TrangThai != "Đã thanh toán"),

                TongThanhToan = await _context.ThanhToans.CountAsync(),

                DoanhThuThangNay = await _context.ThanhToans
                    .Where(t => t.NgayThanhToan >= firstDayOfMonth)
                    .SumAsync(t => (decimal?)t.SoTien) ?? 0,

                TongDichVu = await _context.DichVus.CountAsync(),

                TongTaiKhoan = await _context.TaiKhoans.CountAsync()
            };

            // Thiết lập dữ liệu cho biểu đồ
            ChartLabels = new List<string> { "Phòng trống", "Phòng đang thuê" };
            ChartData = new List<decimal> { Dashboard.PhongTrong, Dashboard.PhongDangThue };
        }
    }
}