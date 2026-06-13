using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using System.ComponentModel.DataAnnotations;

namespace QLNT.Pages.Account;

[AllowAnonymous]
public class ResetPasswordModel : PageModel
{
    private readonly AppDbContext _context;

    public ResetPasswordModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Token { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Token))
        {
            return RedirectToPage("/Account/Login");
        }

        var user = await _context.TaiKhoans
            .FirstOrDefaultAsync(u => u.Email == Email && u.TrangThai);

        if (user is null || user.ResetToken != Token || user.ResetTokenExpiry < DateTime.UtcNow)
        {
            ErrorMessage = "Liên kết không hợp lệ hoặc đã hết hạn.";
            return Page();
        }

        return Page();
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

        if (user is null || user.ResetToken != Token || user.ResetTokenExpiry < DateTime.UtcNow)
        {
            ErrorMessage = "Liên kết không hợp lệ hoặc đã hết hạn.";
            return Page();
        }

        // Cập nhật mật khẩu mới
        user.MatKhau = Input.Password;
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _context.SaveChangesAsync();

        SuccessMessage = "Mật khẩu đã được cập nhật thành công! Vui lòng đăng nhập lại.";
        return Page();
    }

    public IActionResult OnPostLoginRedirect()
    {
        return RedirectToPage("/Account/Login", new { returnUrl = ReturnUrl });
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
