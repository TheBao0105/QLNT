using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace QLNT.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _context;

    public RegisterModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(ReturnUrl ?? "/");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingUser = await _context.TaiKhoans
            .FirstOrDefaultAsync(u => u.TenDangNhap == Input.TenDangNhap || u.Email == Input.Email);

        if (existingUser is not null)
        {
            if (existingUser.TenDangNhap == Input.TenDangNhap)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập đã tồn tại.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email đã được sử dụng.");
            }
            return Page();
        }

        var user = new global::QLNT.TaiKhoan
        {
            TenDangNhap = Input.TenDangNhap,
            Email = Input.Email,
            MatKhau = Input.Password,
            HoTen = Input.TenDangNhap,
            VaiTro = "NhanVien",
            TrangThai = true
        };

        _context.TaiKhoans.Add(user);
        await _context.SaveChangesAsync();

        await SignInUserAsync(user);

        return LocalRedirect(ReturnUrl ?? "/");
    }

    private async Task SignInUserAsync(global::QLNT.TaiKhoan user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.TaiKhoanId.ToString()),
            new Claim(ClaimTypes.Name, user.HoTen ?? user.Email ?? user.TenDangNhap)
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        if (!string.IsNullOrEmpty(user.VaiTro))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.VaiTro));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
