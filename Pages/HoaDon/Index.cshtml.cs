using Microsoft.AspNetCore.Mvc.RazorPages;
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


        public string CurrentFilter { get; set; } = string.Empty;
        public string? StatusFilter { get; set; }


        public async Task OnGetAsync(string currentFilter, string searchString, string? statusFilter, string action)
        {

            if (action == "search")
            {
                CurrentFilter = searchString ?? string.Empty;
            }
            else
            {
                CurrentFilter = currentFilter ?? string.Empty;
            }

            StatusFilter = statusFilter;

            var query = _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.Phong)
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.NguoiThue)
                .Include(h => h.ThanhToans)
                .AsQueryable();

            // 1. Tìm kiếm theo Tên phòng HOẶC Tên người thuê
            if (!string.IsNullOrEmpty(CurrentFilter))
            {
                query = query.Where(h => h.HopDong.Phong.TenPhong.Contains(CurrentFilter) 
                                      || h.HopDong.NguoiThue.HoTen.Contains(CurrentFilter));
            }

            // 2. Lọc theo Trạng thái hóa đơn (So sánh chuỗi trực tiếp, loại bỏ hoàn toàn ??)
            if (!string.IsNullOrWhiteSpace(StatusFilter))
            {
                var today = DateTime.Today;

                if (StatusFilter == "Quá hạn")
                {
                    // Lấy những thằng DB ghi chữ "Quá hạn" HOẶC những thằng ghi "Chưa thanh toán" nhưng lỡ hạn đóng tiền so với hôm nay (2026)
                    query = query.Where(h => h.TrangThai == "Quá hạn" 
                                          || (h.TrangThai == "Chưa thanh toán" && h.HanThanhToan < today));
                }
                else if (StatusFilter == "Chưa thanh toán")
                {
                    // Chỉ lấy những thằng ghi chữ "Chưa thanh toán" và ngày hạn vẫn lớn hơn hoặc bằng hôm nay
                    query = query.Where(h => h.TrangThai == "Chưa thanh toán" && h.HanThanhToan >= today);
                }
                else
                {
                    // Các trạng thái khác (Đã thanh toán, Thanh toán một phần...) thì khớp chuẩn theo chuỗi chữ
                    query = query.Where(h => h.TrangThai == StatusFilter);
                }
            }
            // Thực hiện xuất dữ liệu và sắp xếp
            HoaDonList = await query
                .OrderByDescending(h => h.Nam)
                .ThenByDescending(h => h.Thang)
                .ThenByDescending(h => h.NgayLap)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}