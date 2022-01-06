using MedicalExpert.Data;
using MedicalExpert.Models;
using MedicalExpert.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MedicineGenresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        public MedicineGenresController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // Get
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.MedicineGenres.OrderBy(e => e.Name).Select(e => new MedicineGenreViewModel { Id =  e.Id, Name = e.Name }).ToListAsync());
        }

        // Get
        [HttpGet]
        public IActionResult Create()
        {
            return View("MedicineGenreForm", new MedicineGenreViewModel());
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicineGenreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MedicineGenreForm", model);
            }

            var medicineGenreExsis = await _context.MedicineGenres.FirstOrDefaultAsync(e => e.Name == model.Name);

            if (medicineGenreExsis != null)
            {
                ModelState.AddModelError("Name", "نوع العلاج هذا موجود مسبقا");
                return View("MedicineGenreForm", model);
            }

            await _context.MedicineGenres.AddAsync(new MedicineGenre() { Name = model.Name });
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("تم اضافة نوع الدواء بنجاح.");

            return RedirectToAction(nameof(Index));
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineGenreExsis = await _context.MedicineGenres.FindAsync(id);

            if (medicineGenreExsis == null)
                return NotFound();

            return View(new MedicineGenreViewModel()
            {
                Id = medicineGenreExsis.Id,
                Name = medicineGenreExsis.Name,
                Medicines = await _context.Medicines.Where(e => e.MedicineGenreId == id).ToListAsync()
            });
        }

        // Get
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineGenreExsis = await _context.MedicineGenres.FindAsync(id);

            if (medicineGenreExsis == null)
                return NotFound();

            return View("MedicineGenreForm", new MedicineGenreViewModel() { Id = medicineGenreExsis.Id, Name = medicineGenreExsis.Name });
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicineGenreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("MedicineGenreForm", model);
            }

            var medicineGenreExsis = await _context.MedicineGenres.FirstOrDefaultAsync(e => e.Name == model.Name && e.Id != model.Id);

            if (medicineGenreExsis != null)
            {
                ModelState.AddModelError("Name", "نوع العلاج هذا موجود مسبقا");
                return View("MedicineGenreForm", model);
            }

            medicineGenreExsis = await _context.MedicineGenres.FindAsync(model.Id);
            medicineGenreExsis.Name = model.Name;

            _context.MedicineGenres.Update(medicineGenreExsis);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("تم تعديل نوع الدواء بنجاح.");

            return RedirectToAction(nameof(Index));
        }

        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineGenreExsis = await _context.MedicineGenres.FindAsync(id);

            if (medicineGenreExsis == null)
                return NotFound();

            _context.MedicineGenres.Remove(medicineGenreExsis);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("تم حذف نوع الدواء بنجاح.");

            return Ok();
        }
    }
}
