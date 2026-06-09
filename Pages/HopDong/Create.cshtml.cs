using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace QLNT.Pages_HopDong
{
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
        public string TienCocText { get; set; } = "";

        [BindProperty]
        public string GiaThueText { get; set; } = "";

        [BindProperty]
        public string TrangThaiText { get; set; } = "";

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

            public string? GhiChu { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Input.NgayBatDau = DateTime.Today;

            TrangThaiText = TinhTrangThaiHopDong(Input.NgayBatDau, Input.NgayKetThuc);

            await LoadDataAsync();

            TienCocText = "";
            GiaThueText = "";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDataAsync();

            TrangThaiText = TinhTrangThaiHopDong(Input.NgayBatDau, Input.NgayKetThuc);

            if (!Input.PhongId.HasValue)
            {
                TienCocText = "";
                GiaThueText = "";
            }
            else
            {
                var phong = await _context.Phongs
                    .FirstOrDefaultAsync(p => p.PhongId == Input.PhongId.Value);

                if (phong == null)
                {
                    ModelState.AddModelError("Input.PhongId", "Phòng không tồn tại.");
                    TienCocText = "";
                    GiaThueText = "";
                }
                else
                {
                    TienCocText = FormatVnd(phong.GiaPhong);
                    GiaThueText = FormatVnd(phong.GiaPhong);
                }
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

            var hopDong = new HopDong
            {
                PhongId = Input.PhongId.Value,
                NguoiThueId = Input.NguoiThueId!.Value,
                NgayBatDau = Input.NgayBatDau,
                NgayKetThuc = Input.NgayKetThuc,
                TienCoc = phongDuocChon.GiaPhong,
                GiaThue = phongDuocChon.GiaPhong,
                TrangThai = TrangThaiText,
                GhiChu = Input.GhiChu
            };

            _context.HopDongs.Add(hopDong);

            if (hopDong.TrangThai == "Còn hạn")
            {
                phongDuocChon.TrangThai = "Đã thuê";
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private string TinhTrangThaiHopDong(DateTime ngayBatDau, DateTime? ngayKetThuc)
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

        private string FormatVnd(decimal value)
        {
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