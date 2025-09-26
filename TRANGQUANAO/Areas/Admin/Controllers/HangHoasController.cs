using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TRANGQUANAO.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace TRANGQUANAO.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HangHoasController : Controller
    {
        private readonly TrangquanaoContext _context;
        private readonly IWebHostEnvironment _environment;

        public HangHoasController(TrangquanaoContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/HangHoas
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1; // Trang hiện tại (mặc định là 1)
            int pageSize = 10;         // Số lượng bản ghi hiển thị trên 1 trang

            var data = _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaNccNavigation)
                .OrderBy(h => h.MaHh); // nên sắp xếp để phân trang ổn định

            var pagedData = data.ToPagedList(pageNumber, pageSize);

            return View(pagedData); // Trả về IPagedList<HangHoa>
        }

        // GET: Admin/HangHoas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var hangHoa = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);

            if (hangHoa == null) return NotFound();

            return View(hangHoa);
        }

        // GET: Admin/HangHoas/Create
        public IActionResult Create()
        {
            LoadDropDownList();
            return View();
        }

        // POST: Admin/HangHoas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HangHoa hangHoa, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                // Upload ảnh nếu có
                if (Hinh != null && Hinh.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "Hinh/HangHoa");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(Hinh.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Hinh.CopyToAsync(stream);
                    }

                    hangHoa.Hinh = fileName;
                }

                _context.Add(hangHoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropDownList(hangHoa.MaLoai, hangHoa.MaNcc);
            return View(hangHoa);
        }

        // GET: Admin/HangHoas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa == null) return NotFound();

            LoadDropDownList(hangHoa.MaLoai, hangHoa.MaNcc);
            return View(hangHoa);
        }

        // POST: Admin/HangHoas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HangHoa hangHoa, IFormFile? Hinh)
        {
            if (id != hangHoa.MaHh) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu có upload ảnh mới → lưu & xóa ảnh cũ
                    if (Hinh != null && Hinh.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "Hinh/HangHoa");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string fileName = Guid.NewGuid() + Path.GetExtension(Hinh.FileName);
                        string filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Hinh.CopyToAsync(stream);
                        }

                        // Xóa ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(hangHoa.Hinh))
                        {
                            string oldFilePath = Path.Combine(uploadsFolder, hangHoa.Hinh);
                            if (System.IO.File.Exists(oldFilePath))
                                System.IO.File.Delete(oldFilePath);
                        }

                        hangHoa.Hinh = fileName;
                    }

                    _context.Update(hangHoa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HangHoaExists(hangHoa.MaHh)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadDropDownList(hangHoa.MaLoai, hangHoa.MaNcc);
            return View(hangHoa);
        }

        // GET: Admin/HangHoas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hangHoa = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);

            if (hangHoa == null) return NotFound();

            return View(hangHoa);
        }

        // POST: Admin/HangHoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa != null)
            {
                // Xóa ảnh trong thư mục wwwroot nếu tồn tại
                if (!string.IsNullOrEmpty(hangHoa.Hinh))
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "Hinh/HangHoa");
                    string oldFilePath = Path.Combine(uploadsFolder, hangHoa.Hinh);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                _context.HangHoas.Remove(hangHoa);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadDropDownList(int? selectedLoai = null, string? selectedNcc = null)
        {
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "TenLoai", selectedLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenCongTy", selectedNcc);
        }

        private bool HangHoaExists(int id)
        {
            return _context.HangHoas.Any(e => e.MaHh == id);
        }
    }
}
