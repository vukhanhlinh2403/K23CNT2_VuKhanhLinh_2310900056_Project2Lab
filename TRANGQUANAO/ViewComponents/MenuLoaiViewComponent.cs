using Microsoft.AspNetCore.Mvc;
using TRANGQUANAO.ViewModels;
using TRANGQUANAO.Models;
using TRANGQUANAO.ViewModels;

namespace WEBQUANAO.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly TrangquanaoContext db;

        public MenuLoaiViewComponent(TrangquanaoContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                Tenloai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(p => p.Tenloai);

            return View(data);
        }
    }
}
