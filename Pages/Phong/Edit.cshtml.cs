using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;
using System.Globalization;

namespace QLNT.Pages_Phong
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Phong Phong { get; set; } = default!;

        [BindProperty]
        public string GiaPhongText { get; set; } = "";

        [BindProperty]
        public IFormFile? HinhAnhUpload { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs.FirstOrDefaultAsync(m => m.PhongId == id);

            if (phong == null)
            {
                return NotFound();
            }

            Phong = phong;
            GiaPhongText = FormatVnd(Phong.GiaPhong);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Phong.GiaPhong");

            if (!TryParseVnd(GiaPhongText, out var giaPhong))
            {
                ModelState.AddModelError(nameof(GiaPhongText), "Giá phòng không đúng định dạng.");
            }

            if (giaPhong < 0)
            {
                ModelState.AddModelError(nameof(GiaPhongText), "Giá phòng không được nhỏ hơn 0.");
            }

            Phong.GiaPhong = giaPhong;

            if (string.IsNullOrWhiteSpace(Phong.TrangThai))
            {
                Phong.TrangThai = "Trống";
            }

            // Xử lý upload hình ảnh
            if (HinhAnhUpload != null && HinhAnhUpload.Length > 0)
            {
                try
                {
                    // Kiểm tra loại file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(HinhAnhUpload.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError(nameof(HinhAnhUpload), "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif).");
                    }

                    // Kiểm tra kích thước file (tối đa 5MB)
                    if (HinhAnhUpload.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(nameof(HinhAnhUpload), "Kích thước file không được vượt quá 5MB.");
                    }

                    if (!ModelState.IsValid)
                    {
                        GiaPhongText = FormatVnd(Phong.GiaPhong);
                        return Page();
                    }

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(Phong.HinhAnh))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, Phong.HinhAnh.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Tạo tên file duy nhất
                    var fileName = Path.GetFileNameWithoutExtension(HinhAnhUpload.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now.Ticks}{fileExtension}";

                    // Đường dẫn thư mục lưu ảnh
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "rooms");

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, newFileName);

                    // Lưu file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnhUpload.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn vào database
                    Phong.HinhAnh = $"/images/rooms/{newFileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(HinhAnhUpload), $"Lỗi khi upload ảnh: {ex.Message}");
                    GiaPhongText = FormatVnd(Phong.GiaPhong);
                    return Page();
                }
            }

            if (!ModelState.IsValid)
            {
                GiaPhongText = FormatVnd(Phong.GiaPhong);
                return Page();
            }

            _context.Attach(Phong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhongExists(Phong.PhongId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private string FormatVnd(decimal value)
        {
            return value.ToString("N0", new CultureInfo("vi-VN"));
        }

        private bool TryParseVnd(string? input, out decimal value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(input))
            {
                return true;
            }

            var clean = input
                .Replace(".", "")
                .Replace(",", "")
                .Replace("VNĐ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("VND", "", StringComparison.OrdinalIgnoreCase)
                .Replace("đ", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            return decimal.TryParse(
                clean,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value
            );
        }

        private bool PhongExists(int id)
        {
            return _context.Phongs.Any(e => e.PhongId == id);
        }
    }
}