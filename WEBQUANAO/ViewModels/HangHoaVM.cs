namespace WEBQUANAO.ViewModels
{
    public class HangHoaVM
    {
        public int MaHh { get; set; }
        public string TenHH { get; set; } = null!;
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; } = null!;
        public string Hinh { get; set; } = null!;
        public string Tenloai { get; set; } = null!;
    }


    public class ChiTietHangHoaVM
    {
        public int MaHh { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; }
        public string Tenloai { get; set; }
        public string ChiTiet { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; }
    }
}
