using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_NguoiThue
{
    public class IndexModel : PageModel
    {
        private readonly QLNT.Data.AppDbContext _context;
        private const int PageSize = 5;

        public IndexModel(QLNT.Data.AppDbContext context)
        {
            _context = context;
        }

        // Tự động bind dữ liệu từ Form mà không cần sửa tham số OnGetAsync
        [BindProperty(SupportsGet = true)] public string? SearchHoTen { get; set; }
        [BindProperty(SupportsGet = true)] public string? SearchSdt { get; set; }
        [BindProperty(SupportsGet = true)] public string? SearchEmail { get; set; }
        [BindProperty(SupportsGet = true)] public string? SearchCccd { get; set; }
        [BindProperty(SupportsGet = true)] public string? SearchDiaChi { get; set; }


        public string CurrentFilter { get; set; } = string.Empty;
        public PaginatedList<NguoiThue> NguoiThue { get; set; } = default!;

        // Hàm OnGetAsync giữ nguyên 3 tham số gốc ban đầu của bạn
        public async Task OnGetAsync(string currentFilter, string searchString, int? pageIndex)
        {
            // Reset về trang 1 nếu có bất kỳ hành động tìm kiếm nào
            if (!string.IsNullOrEmpty(SearchHoTen) || !string.IsNullOrEmpty(SearchSdt) || 
                !string.IsNullOrEmpty(SearchEmail) || !string.IsNullOrEmpty(SearchCccd) || !string.IsNullOrEmpty(SearchDiaChi))
            {
                pageIndex = 1;
            }

            var query = from n in _context.NguoiThues
                        select n;

            // Tách thành các điều kiện lọc riêng biệt bằng câu lệnh Like gốc của bạn
            if (!string.IsNullOrEmpty(SearchHoTen))
                query = query.Where(n => EF.Functions.Like(n.HoTen, $"%{SearchHoTen}%"));

            if (!string.IsNullOrEmpty(SearchSdt))
                query = query.Where(n => n.SoDienThoai == SearchSdt);

            if (!string.IsNullOrEmpty(SearchEmail))
                query = query.Where(n => EF.Functions.Like(n.Email ?? string.Empty, $"%{SearchEmail}%"));

            if (!string.IsNullOrEmpty(SearchCccd))
                query = query.Where(n => EF.Functions.Like(n.CCCD ?? string.Empty, $"%{SearchCccd}%"));

            if (!string.IsNullOrEmpty(SearchDiaChi))
                query = query.Where(n => EF.Functions.Like(n.DiaChi ?? string.Empty, $"%{SearchDiaChi}%"));

            query = query.OrderBy(n => n.HoTen);
            NguoiThue = await PaginatedList<NguoiThue>.CreateAsync(query.AsNoTracking(), pageIndex ?? 1, PageSize);
        }
    }
}