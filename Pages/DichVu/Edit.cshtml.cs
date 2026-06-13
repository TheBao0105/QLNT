using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QLNT;
using QLNT.Data;

namespace QLNT.Pages_DichVu
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DichVu DichVu { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichvu = await _context.DichVus.FirstOrDefaultAsync(m => m.DichVuId == id);

            if (dichvu == null)
            {
                return NotFound();
            }

            DichVu = dichvu;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (DichVu.DonGia < 0)
            {
                ModelState.AddModelError("DichVu.DonGia", "Đơn giá không được nhỏ hơn 0.");
            }

            if (string.IsNullOrWhiteSpace(DichVu.TrangThai))
            {
                DichVu.TrangThai = "Đang sử dụng";
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(DichVu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DichVuExists(DichVu.DichVuId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool DichVuExists(int id)
        {
            return _context.DichVus.Any(e => e.DichVuId == id);
        }
    }
}