using MedicalExpert.Data;
using MedicalExpert.Models;
using MedicalExpert.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public CategoriesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.Select(e => new CategoryViewModel { Id = e.Id, Name = e.Name }).OrderBy(e => e.Name).ToListAsync());
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View("CategoryForm", new CategoryViewModel()
            {
                Sides = await _context.Sides.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Description, IsSelected = false }).ToListAsync()
            });
        }
        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CategoryForm", model);
            }

            var categoryExsis = await _context.Categories.FirstOrDefaultAsync(e => e.Name == model.Name);

            if (categoryExsis != null)
            {
                ModelState.AddModelError("Name", "هذا القسم موجودة مسبقا");
                return View("CategoryForm", model);
            }

            var files = Request.Form.Files;
            var picture = files.FirstOrDefault();

            if (picture == null)
            {
                ModelState.AddModelError("Picture", "يجب اختيار صوره");
                return View("CategoryForm", model);
            }

            if (picture.Length > 1048576)
            {
                ModelState.AddModelError("Picture", "يجب ان تكون الصورة اقل من 1 ميجا");
                return View("CategoryForm", model);
            }

            using var dataStream = new MemoryStream();
            await picture.CopyToAsync(dataStream);

            await _context.Categories.AddAsync(new Category() { Name = model.Name.Trim(), Picture = dataStream.ToArray() });
            _context.SaveChanges();

            if (model.Sides != null)
            {
                var currentCategory = await _context.Categories.FirstAsync(e => e.Name == model.Name);
                var categoriesSidesList = new List<CategorySide>();

                foreach (var side in model.Sides)
                {
                    if (side.IsSelected)
                    {
                        categoriesSidesList.Add(new CategorySide { CategoryId = currentCategory.Id, SideId = side.Id });
                    }
                }

                await _context.CategoriesSides.AddRangeAsync(categoriesSidesList);
                _context.SaveChanges();
            }

            _toastNotification.AddSuccessToastMessage("تم اضافة القسم بنجاح.");

            return RedirectToAction(nameof(Index));
        }
        
        // GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var categoryExist = await _context.Categories.FindAsync(id);

            if (categoryExist == null)
                return NotFound();

            var xx = new CategoryViewModel()
            {
                Id = categoryExist.Id,
                Name = categoryExist.Name,
                Picture = categoryExist.Picture,
                Diseases = await _context.Diseases.Where(e => e.CategoryId == categoryExist.Id).Select(e => new CheckBoxViewModel { Name = e.Name }).ToListAsync(),
                Sides = await _context.CategoriesSides.Where(e => e.CategoryId == categoryExist.Id).Include(e => e.Side).Select(e => new CheckBoxViewModel { Name = e.Side.Description }).ToListAsync()
            };

            return View(xx);
        }
        
        // GET
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var categoryExist = await _context.Categories.FindAsync(id);

            if (categoryExist == null)
                return NotFound();

            var sidesList = await _context.Sides.ToListAsync();
            var sidesIds = await _context.CategoriesSides.Where(e => e.CategoryId == categoryExist.Id).Select(e => e.SideId).ToListAsync();

            return View("CategoryForm", new CategoryViewModel()
            {
                Id = categoryExist.Id,
                Name = categoryExist.Name,
                Picture = categoryExist.Picture,
                Sides = sidesList.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Description, IsSelected = sidesIds.Contains(e.Id) }).ToList()
            });
        }
        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CategoryForm", model);
            }

            var categoryExist = await _context.Categories.FirstOrDefaultAsync(e => e.Name == model.Name && e.Id != model.Id);

            if (categoryExist != null)
            {
                ModelState.AddModelError("Name", "هذا القسم موجود مسبقا.");
                return View("CategoryForm", model);
            }

            categoryExist = await _context.Categories.FindAsync(model.Id);
            categoryExist.Name = model.Name;

            var files = Request.Form.Files;
            byte[] newPicture = categoryExist.Picture;

            if (files.Any())
            {
                var picture = files.FirstOrDefault();

                if (picture.Length > 1048576)
                {
                    ModelState.AddModelError("Picture", "يجب ان تكون الصورة اقل من 1 ميجا");
                    return View("CategoryForm", model);
                }

                using var dataStream = new MemoryStream();
                await picture.CopyToAsync(dataStream);

                newPicture = dataStream.ToArray();
            }

            categoryExist.Picture = newPicture;

            var sidesIds = await _context.CategoriesSides.Where(e => e.CategoryId == categoryExist.Id).Select(e => e.SideId).ToListAsync();

            var categoriesSidesToDelete = new List<CategorySide>();
            var categoriesSidesToAdd = new List<CategorySide>();

            foreach (var side in model.Sides)
            {
                if (sidesIds.Contains(side.Id) && !side.IsSelected)
                {
                    categoriesSidesToDelete.Add(new CategorySide { CategoryId = categoryExist.Id, SideId = side.Id });
                }

                if (!sidesIds.Contains(side.Id) && side.IsSelected)
                {
                    categoriesSidesToAdd.Add(new CategorySide { CategoryId = categoryExist.Id, SideId = side.Id });
                }
            }

            _context.CategoriesSides.RemoveRange(categoriesSidesToDelete);
            _context.CategoriesSides.AddRange(categoriesSidesToAdd);
            _context.Categories.Update(categoryExist);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("تم تعديل القسم بنجاح.");

            return RedirectToAction(nameof(Index));
        }

        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var categoryExsist = await _context.Categories.FindAsync(id);

            if (categoryExsist == null)
                return NotFound();

            _context.Categories.Remove(categoryExsist);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
