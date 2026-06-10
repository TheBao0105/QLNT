using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using QLNT.Services;

namespace QLNT.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IEmailSender _emailSender;

    public ForgotPasswordModel(AppDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? StatusMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _context.TaiKhoans
            .FirstOrDefaultAsync(u => u.TrangThai && u.Email == Input.Email);

        if (user is null)
        {
            // Để bảo mật, không nói email có hay không
            StatusMessage = "Nếu email tồn tại, bạn sẽ nhận được mã xác nhận.";
            return Page();
        }

        // Tạo OTP 6 chữ số
        var otp = GenerateOTP();
        user.ResetToken = otp;
        user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

        await _context.SaveChangesAsync();

        // Gửi email với OTP
        try
        {
            var subject = "Mã xác nhận đặt lại mật khẩu";
            var body = $"<p>Xin chào,</p><p>Mã xác nhận đặt lại mật khẩu của bạn là <strong>{otp}</strong>. Mã có hiệu lực trong 15 phút.</p><p>Nếu bạn không yêu cầu mã này, hãy bỏ qua email này.</p>";
            await _emailSender.SendEmailAsync(Input.Email, subject, body);
        }
        catch
        {
            // Không ném lỗi để tránh tiết lộ trạng thái email cho người dùng.
        }

        // Chuyển sang trang xác nhận mã
        return RedirectToPage("./VerifyCode", new { email = Input.Email, returnUrl = ReturnUrl });
    }

    private static string GenerateOTP()
    {
        var bytes = RandomNumberGenerator.GetBytes(3);
        var otp = (bytes[0] * 10000 + bytes[1] * 100 + bytes[2]) % 1000000;
        return otp.ToString("D6");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
