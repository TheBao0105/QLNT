using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QLNT;
using QLNT.Data;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace QLNT.Pages_Phong
{
        [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Phong Phong { get; set; } = new Phong();

        [BindProperty]
        public string GiaPhongText { get; set; } = "";

        [BindProperty]
        public IFormFile? HinhAnhUpload { get; set; }

        public IActionResult OnGet()
        {
            Phong.TrangThai = "Trống";
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
                        return Page();
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

                    // Lưu đường dẫn vào database
                    Phong.HinhAnh = $"/images/rooms/{newFileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(HinhAnhUpload), $"Lỗi khi upload ảnh: {ex.Message}");
                    return Page();
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Phongs.Add(Phong);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
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
    }
}