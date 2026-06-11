using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using System.ComponentModel.DataAnnotations;

namespace QLNT.Pages.Account;

[AllowAnonymous]
public class VerifyCodeModel : PageModel
{
    private readonly AppDbContext _context;

    public VerifyCodeModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(Email))
        {
            ErrorMessage = "Thông tin không hợp lệ.";
            return Page();
        }

        var user = await _context.TaiKhoans
            .FirstOrDefaultAsync(u => u.Email == Email && u.TrangThai);

        if (user is null)
        {
            ErrorMessage = "Không tìm thấy tài khoản.";
            return Page();
        }

        // Kiểm tra OTP
        if (string.IsNullOrEmpty(user.ResetToken) || user.ResetToken != Input.Code)
        {
            ErrorMessage = "Mã xác nhận không đúng.";
            return Page();
        }

        // Kiểm tra hết hạn (15 phút)
        if (user.ResetTokenExpiry < DateTime.UtcNow)
        {
            ErrorMessage = "Mã xác nhận đã hết hạn. Vui lòng yêu cầu lại.";
            return Page();
        }

        // Chuyển sang trang đặt lại mật khẩu
        return RedirectToPage("./ResetPassword", new { email = Email, token = user.ResetToken, returnUrl = ReturnUrl });
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Mã xác nhận không được để trống")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác nhận phải là 6 chữ số")]
        [Display(Name = "Mã xác nhận")]
        public string Code { get; set; } = string.Empty;
    }
}
