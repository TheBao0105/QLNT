using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.ViewModels;

namespace QLNT.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public DashboardViewModel Stats { get; set; } = new();
    public int TyLeThue { get; set; } = new();
    public int TongPhong { get; set; }
    public int PhongDangThue { get; set; }
    public int PhongTrong { get; set; }

    public int HopDongHieuLuc { get; set; }
    public async Task OnGetAsync()
    {
        var today = DateTime.Today;

        TongPhong = await _context.Phongs.CountAsync();

        HopDongHieuLuc = await _context.HopDongs
            .CountAsync(h =>
                h.NgayBatDau.Date <= today &&
                (h.NgayKetThuc == null || h.NgayKetThuc.Value.Date >= today)
            );

        PhongDangThue = await _context.HopDongs
            .Where(h =>
                h.NgayBatDau.Date <= today &&
                (h.NgayKetThuc == null || h.NgayKetThuc.Value.Date >= today)
            )
            .Select(h => h.PhongId)
            .Distinct()
            .CountAsync();

        PhongTrong = TongPhong - PhongDangThue;

        if (PhongTrong < 0)
        {
            PhongTrong = 0;
        }

        if (TongPhong > 0)
        {
            TyLeThue = (int)Math.Round((double)PhongDangThue / TongPhong * 100);
        }
        else
        {
            TyLeThue = 0;
        }
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        Stats = new DashboardViewModel
        {
            TongPhong = await _context.Phongs.CountAsync(),
            PhongTrong = await _context.Phongs.CountAsync(p => p.TrangThai == "Trống"),
            PhongDangThue = await _context.Phongs.CountAsync(p => p.TrangThai == "Đang thuê"),
            TongNguoiThue = await _context.NguoiThues.CountAsync(),
            TongHopDong = await _context.HopDongs.CountAsync(),
            HopDongDangHieuLuc = await _context.HopDongs.CountAsync(h => h.TrangThai == "Đang hiệu lực"),
            TongHoaDon = await _context.HoaDons.CountAsync(),
            HoaDonChuaThanhToan = await _context.HoaDons.CountAsync(h =>
                h.TrangThai == "Chưa thanh toán" || h.TrangThai == "Thanh toán một phần"),
            HoaDonQuaHan = await _context.HoaDons.CountAsync(h =>
                h.HanThanhToan < today && h.TrangThai != "Đã thanh toán"),
            DoanhThuThangNay = await _context.ThanhToans
                .Where(t => t.NgayThanhToan >= firstDayOfMonth)
                .SumAsync(t => (decimal?)t.SoTien) ?? 0,
            TongDichVu = await _context.DichVus.CountAsync(),
            TongTaiKhoan = await _context.TaiKhoans.CountAsync()

        };
    }
}
