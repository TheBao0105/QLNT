using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace QLNT.Pages_HopDong
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
        public InputModel Input { get; set; } = new();

        [BindProperty]
        public string? FormAction { get; set; }

        public List<Phong> DanhSachPhong { get; set; } = new();

        public SelectList NguoiThueSelectList { get; set; } = default!;

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng chọn phòng.")]
            public int? PhongId { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn người thuê.")]
            public int? NguoiThueId { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu.")]
            [DataType(DataType.Date)]
            public DateTime NgayBatDau { get; set; } = DateTime.Today;

            [DataType(DataType.Date)]
            public DateTime? NgayKetThuc { get; set; }

            public decimal TienCoc { get; set; }

            public decimal GiaThue { get; set; }

            public string TrangThai { get; set; } = "Còn hạn";

            public string? GhiChu { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Input.NgayBatDau = DateTime.Today;
            Input.TrangThai = TinhTrangThaiHopDong(Input.NgayBatDau, Input.NgayKetThuc);

            await LoadDataAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDataAsync();

            Input.TrangThai = TinhTrangThaiHopDong(Input.NgayBatDau, Input.NgayKetThuc);

            await DongBoTienTheoPhongAsync();


            ModelState.Remove("Input.TienCoc");
            ModelState.Remove("Input.GiaThue");
            ModelState.Remove("Input.TrangThai");

            if (FormAction == "preview")
            {
                ModelState.Clear();
                return Page();
            }

            if (!Input.PhongId.HasValue)
            {
                ModelState.AddModelError("Input.PhongId", "Vui lòng chọn phòng.");
            }

            if (!Input.NguoiThueId.HasValue)
            {
                ModelState.AddModelError("Input.NguoiThueId", "Vui lòng chọn người thuê.");
            }

            if (Input.NgayKetThuc.HasValue &&
                Input.NgayKetThuc.Value.Date < Input.NgayBatDau.Date)
            {
                ModelState.AddModelError("Input.NgayKetThuc", "Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var phongDuocChon = await _context.Phongs
                .FirstOrDefaultAsync(p => p.PhongId == Input.PhongId!.Value);

            if (phongDuocChon == null)
            {
                ModelState.AddModelError("Input.PhongId", "Phòng không tồn tại.");
                return Page();
            }
            //chỉ được lập hợp đồng cho phòng đang trống hoặc đã thuê nhưng hợp đồng đã hết hạn
            if (phongDuocChon.TrangThai == "Đã thuê")
            {
                ModelState.AddModelError("Input.PhongId", "Phòng này đã được thuê.");
                return Page();
            }

            Input.TienCoc = phongDuocChon.GiaPhong;
            Input.GiaThue = phongDuocChon.GiaPhong;
            Input.TrangThai = TinhTrangThaiHopDong(Input.NgayBatDau, Input.NgayKetThuc);

            var hopDong = new HopDong
            {
                PhongId = Input.PhongId.Value,
                NguoiThueId = Input.NguoiThueId!.Value,
                NgayBatDau = Input.NgayBatDau,
                NgayKetThuc = Input.NgayKetThuc,
                TienCoc = Input.TienCoc,
                GiaThue = Input.GiaThue,
                TrangThai = Input.TrangThai,
                GhiChu = Input.GhiChu
            };

            _context.HopDongs.Add(hopDong);

            if (hopDong.TrangThai == "Còn hạn")
            {
                phongDuocChon.TrangThai = "Đã thuê";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm hợp đồng thành công.";

            return RedirectToPage("./Index");
        }

        private async Task DongBoTienTheoPhongAsync()
        {
            Input.TienCoc = 0;
            Input.GiaThue = 0;

            if (!Input.PhongId.HasValue)
            {
                return;
            }

            var phong = await _context.Phongs
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PhongId == Input.PhongId.Value);

            if (phong == null)
            {
                return;
            }

            Input.TienCoc = phong.GiaPhong;
            Input.GiaThue = phong.GiaPhong;
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
            if (value <= 0)
            {
                return "";
            }

            return value.ToString("N0", new CultureInfo("vi-VN"));
        }

        private async Task LoadDataAsync()
        {
            DanhSachPhong = await _context.Phongs
                .AsNoTracking()
                .OrderBy(p => p.TenPhong)
                .ToListAsync();

            var nguoiThues = await _context.NguoiThues
                .AsNoTracking()
                .OrderBy(n => n.HoTen)
                .ToListAsync();

            NguoiThueSelectList = new SelectList(
                nguoiThues,
                "NguoiThueId",
                "HoTen",
                Input.NguoiThueId
            );
        }
    }
}