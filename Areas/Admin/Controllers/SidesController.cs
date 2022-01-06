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
    public class SidesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public SidesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sides.Select(e => new SideViewModel { Id = e.Id, Description = e.Description }).OrderBy(e => e.Description).ToListAsync());
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View("SideForm", new SideViewModel()
            {
                Categories = await _context.Categories.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync()
            });
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SideViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SideForm", model);
            }

            var sideExsis = await _context.Sides.FirstOrDefaultAsync(e => e.Description == model.Description.Trim());

            if (sideExsis != null)
            {
                ModelState.AddModelError("Description", "هذا العرض موجود مسبقا");
                return View("SideForm", model);
            }

            await _context.Sides.AddAsync(new Side() { Description = model.Description.Trim() });
            _context.SaveChanges();

            if (model.Categories != null)
            {
                var currentSide = await _context.Sides.FirstAsync(e => e.Description == model.Description);
                var categoriesSides = new List<CategorySide>();

                foreach (var category in model.Categories)
                {
                    if (category.IsSelected)
                    {
                        categoriesSides.Add(new CategorySide { CategoryId = category.Id, SideId = currentSide.Id });
                    }
                }

                await _context.CategoriesSides.AddRangeAsync(categoriesSides);
                _context.SaveChanges();
            }

            _toastNotification.AddSuccessToastMessage("تم اضافة العرض بنجاح.");

            return RedirectToAction(nameof(Index));
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var sideExist = await _context.Sides.FindAsync(id);

            if (sideExist == null)
                return NotFound();

            return View(new SideViewModel()
            {
                Id = sideExist.Id,
                Description = sideExist.Description,
                Categories = await _context.CategoriesSides.Where(e => e.SideId == sideExist.Id).Include(e => e.Category).Select(e => new CheckBoxViewModel { Name = e.Category.Name }).ToListAsync()
            });
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var sideExist = await _context.Sides.FindAsync(id);

            if (sideExist == null)
                return NotFound();

            var categoriesList = await _context.Categories.ToListAsync();
            var categoriesIds = await _context.CategoriesSides.Where(e => e.SideId == sideExist.Id).Select(e => e.CategoryId).ToListAsync();
            
            return View("SideForm", new SideViewModel()
            {
                Id = sideExist.Id,
                Description = sideExist.Description,
                Categories = categoriesList.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = categoriesIds.Contains(e.Id) }).ToList()
            });
        }
        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SideViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SideForm", model);
            }

            var sideExist = await _context.Sides.FirstOrDefaultAsync(e => e.Description == model.Description.Trim() && e.Id != model.Id);

            if (sideExist != null)
            {
                ModelState.AddModelError("Description", "هذا العرض موجود مسبقا.");
                return View("SideForm", model);
            }

            sideExist = await _context.Sides.FindAsync(model.Id);
            sideExist.Description = model.Description;

            var categoriesIds = await _context.CategoriesSides.Where(e => e.SideId == model.Id).Select(e => e.CategoryId).ToListAsync();

            var categoriesSidesToDelete = new List<CategorySide>();
            var categoriesSidesToAdd = new List<CategorySide>();

            foreach (var category in model.Categories)
            {
                if (categoriesIds.Contains(category.Id) && !category.IsSelected)
                {
                    categoriesSidesToDelete.Add(new CategorySide { CategoryId = category.Id, SideId = sideExist.Id });
                }

                if (!categoriesIds.Contains(category.Id) && category.IsSelected)
                {
                    categoriesSidesToAdd.Add(new CategorySide { CategoryId = category.Id, SideId = sideExist.Id });
                }
            }

            _context.RemoveRange(categoriesSidesToDelete);
            _context.AddRange(categoriesSidesToAdd);
            _context.Sides.Update(sideExist);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("تم تعديل العرض بنجاح.");

            return RedirectToAction(nameof(Index));
        }
        
        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var sideExist = await _context.Sides.FindAsync(id);

            if (sideExist == null)
                return NotFound();

            _context.Sides.Remove(sideExist);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
