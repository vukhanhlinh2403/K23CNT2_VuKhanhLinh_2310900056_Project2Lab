using Microsoft.AspNetCore.Mvc;
using WEBQUANAO.Models;
using WEBQUANAO.ViewModels;

namespace WEBQUANAO.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly QlquanaoContext db;

        public MenuLoaiViewComponent(QlquanaoContext context) => db = context;

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
