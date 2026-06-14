using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace QLNT.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginModel(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public bool FacebookEnabled { get; private set; }
    public bool GoogleEnabled { get; private set; }

    private void LoadSettings()
    {
        FacebookEnabled = !string.IsNullOrWhiteSpace(_configuration["Authentication:Facebook:AppId"]) &&
                          !string.IsNullOrWhiteSpace(_configuration["Authentication:Facebook:AppSecret"]);
        GoogleEnabled = !string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientId"]) &&
                        !string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientSecret"]);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        LoadSettings();

        if (User?.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(ReturnUrl ?? "/");
        }

        var externalResult = await HttpContext.AuthenticateAsync("ExternalCookie");
        if (externalResult?.Succeeded == true && externalResult.Principal is not null)
        {
            var email = externalResult.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Không thể lấy email từ nhà cung cấp bên ngoài.");
                return Page();
            }

            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(u => u.Email == email && u.TrangThai);

            if (user is null)
            {
                user = new global::QLNT.TaiKhoan
                {
                    TenDangNhap = email,
                    MatKhau = Guid.NewGuid().ToString("N"),
                    Email = email,
                    HoTen = externalResult.Principal.FindFirstValue(ClaimTypes.Name) ?? email,
                    VaiTro = "NhanVien",
                    TrangThai = true
                };

                _context.TaiKhoans.Add(user);
                await _context.SaveChangesAsync();
            }

            await SignInUserAsync(user);
            await HttpContext.SignOutAsync("ExternalCookie");

            // THÊM MỚI: Phân luồng sau khi đăng nhập bằng Google/Facebook thành công
            if (string.IsNullOrEmpty(ReturnUrl) || ReturnUrl == "/")
            {
                if (user.VaiTro == "Admin") return RedirectToPage("/Index");
                if (user.VaiTro == "NhanVien") return RedirectToPage("/Dashboard/Index");
            }

            return LocalRedirect(ReturnUrl ?? "/");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        LoadSettings();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _context.TaiKhoans
            .FirstOrDefaultAsync(u => u.TrangThai &&
                (u.Email == Input.AccountOrEmail || u.TenDangNhap == Input.AccountOrEmail) &&
                u.MatKhau == Input.Password);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng.");
            return Page();
        }

        await SignInUserAsync(user);
        
        // THÊM MỚI: Phân luồng sau khi đăng nhập bằng tài khoản thường thành công
        if (string.IsNullOrEmpty(ReturnUrl) || ReturnUrl == "/")
        {
            if (user.VaiTro == "Admin")
            {
                return RedirectToPage("/Index");
            }
            else if (user.VaiTro == "NhanVien")
            {
                return RedirectToPage("/Dashboard/Index");
            }
        }

        return LocalRedirect(ReturnUrl ?? "/");
    }

    public IActionResult OnPostExternalLogin(string provider)
    {
        var redirectUrl = Url.Page("./Login", new { returnUrl = ReturnUrl ?? "/" }) ?? "/";
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, provider);
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
        [Display(Name = "Tài khoản hoặc email")]
        public string AccountOrEmail { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;
    }
}