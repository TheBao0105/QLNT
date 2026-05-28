using Microsoft.EntityFrameworkCore;
using QLNT.Data;

namespace QLNT.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Phong> Phongs { get; set; }
    public DbSet<NguoiThue> NguoiThues { get; set; }
    public DbSet<HopDong> HopDongs { get; set; }
    public DbSet<HoaDon> HoaDons { get; set; }
    public DbSet<ThanhToan> ThanhToans { get; set; }
    public DbSet<DichVu> DichVus { get; set; }
    public DbSet<TaiKhoan> TaiKhoans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Phong
        modelBuilder.Entity<Phong>()
            .Property(p => p.GiaPhong)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Phong>()
            .Property(p => p.DienTich)
            .HasColumnType("decimal(18,2)");

        // HopDong
        modelBuilder.Entity<HopDong>()
            .Property(h => h.TienCoc)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<HopDong>()
            .HasOne(h => h.Phong)
            .WithMany(p => p.HopDongs)
            .HasForeignKey(h => h.PhongId);

        modelBuilder.Entity<HopDong>()
            .HasOne(h => h.NguoiThue)
            .WithMany(n => n.HopDongs)
            .HasForeignKey(h => h.NguoiThueId);

        // HoaDon
        modelBuilder.Entity<HoaDon>()
            .Property(h => h.TienPhong)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<HoaDon>()
            .Property(h => h.TienDien)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<HoaDon>()
            .Property(h => h.TienNuoc)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<HoaDon>()
            .Property(h => h.TienDichVu)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<HoaDon>()
            .Property(h => h.TongTien)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<HoaDon>()
            .HasOne(h => h.HopDong)
            .WithMany(hd => hd.HoaDons)
            .HasForeignKey(h => h.HopDongId);

        // ThanhToan
        modelBuilder.Entity<ThanhToan>()
            .Property(t => t.SoTien)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ThanhToan>()
            .HasOne(t => t.HoaDon)
            .WithMany(h => h.ThanhToans)
            .HasForeignKey(t => t.HoaDonId);

        // DichVu
        modelBuilder.Entity<DichVu>()
            .Property(d => d.DonGia)
            .HasColumnType("decimal(18,2)");

        // TaiKhoan
        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.TenDangNhap)
            .IsUnique();
    }
}