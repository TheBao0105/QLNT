namespace QLNT.ViewModels
{
    public class DashboardViewModel
    {
        public int TongPhong { get; set; }
        public int PhongTrong { get; set; }
        public int PhongDangThue { get; set; }

        public int TongNguoiThue { get; set; }

        public int TongHopDong { get; set; }
        public int HopDongDangHieuLuc { get; set; }

        public int TongHoaDon { get; set; }
        public int HoaDonChuaThanhToan { get; set; }
        public int HoaDonQuaHan { get; set; }

        public int TongThanhToan { get; set; }
        public decimal DoanhThuThangNay { get; set; }

        public int TongDichVu { get; set; }
        public int TongTaiKhoan { get; set; }
    }
}