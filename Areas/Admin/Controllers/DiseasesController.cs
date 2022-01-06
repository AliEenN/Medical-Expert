using MedicalExpert.Constants;
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
    public class DiseasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public DiseasesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Diseases.Select(e => new DiseaseViewModel { Id = e.Id, Name = e.Name }).OrderBy(e => e.Name).ToListAsync());
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View("DiseaseForm", new DiseaseViewModel()
            {
                DiseaseRisks = new List<RadioButtonViewModel>(){ 
                    new RadioButtonViewModel{ Id = DiseaseRisk.Danger, Name = DiseaseRisk.SDanger },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Difficult, Name = DiseaseRisk.SDifficult },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Simple, Name = DiseaseRisk.SSimple }
                },
                Categories = await _context.Categories.Select(e => new RadioButtonViewModel { Id = e.Id, Name = e.Name }).ToListAsync()
            });
        }
        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiseaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.Select(e => new RadioButtonViewModel { Id = e.Id, Name = e.Name }).ToListAsync();
                model.DiseaseRisks = new List<RadioButtonViewModel>(){
                    new RadioButtonViewModel{ Id = DiseaseRisk.Danger, Name = DiseaseRisk.SDanger },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Difficult, Name = DiseaseRisk.SDifficult },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Simple, Name = DiseaseRisk.SSimple }
                };
                return View("DiseaseForm", model);
            }

            var diseaseExsis = await _context.Diseases.FirstOrDefaultAsync(e => e.Name == model.Name);

            if (diseaseExsis != null)
            {
                ModelState.AddModelError("Name", "هذا المرض موجود مسبقا!");
                model.Categories = await _context.Categories.Select(e => new RadioButtonViewModel { Id = e.Id, Name = e.Name }).ToListAsync();
                model.DiseaseRisks = new List<RadioButtonViewModel>(){
                    new RadioButtonViewModel{ Id = DiseaseRisk.Danger, Name = DiseaseRisk.SDanger },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Difficult, Name = DiseaseRisk.SDifficult },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Simple, Name = DiseaseRisk.SSimple }
                };
                return View("DiseaseForm", model);
            }

            await _context.Diseases.AddAsync(new Disease() { Name = model.Name, CategoryId = model.CategoryId, DiseaseRisk = model.DiseaseRisk });
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("تم اضافة القسم بنجاح.");

            return RedirectToAction(nameof(Index));
        }
        
        // GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var diseaseExist = await _context.Diseases.FindAsync(id);

            if (diseaseExist == null)
                return NotFound();

            return View(new DiseaseViewModel()
            {
                Id = diseaseExist.Id,
                Name = diseaseExist.Name,
                CategoryId = diseaseExist.CategoryId,Category = await _context.Categories.FirstAsync(e => e.Id == diseaseExist.CategoryId)
            });
        }
        
        // GET
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var diseaseExist = await _context.Diseases.FindAsync(id);

            if (diseaseExist == null)
                return NotFound();

            return View("DiseaseForm", new DiseaseViewModel()
            {
                Id = diseaseExist.Id,
                Name = diseaseExist.Name,
                CategoryId = diseaseExist.CategoryId,
                DiseaseRisk = diseaseExist.DiseaseRisk,
                DiseaseRisks = new List<RadioButtonViewModel>(){
                    new RadioButtonViewModel{ Id = DiseaseRisk.Danger, Name = DiseaseRisk.SDanger },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Difficult, Name = DiseaseRisk.SDifficult },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Simple, Name = DiseaseRisk.SSimple }
                },
                Categories = await _context.Categories.Select(e => new RadioButtonViewModel { Id = e.Id, Name = e.Name }).ToListAsync()
            });
        }
        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DiseaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DiseaseRisks = new List<RadioButtonViewModel>(){
                    new RadioButtonViewModel{ Id = DiseaseRisk.Danger, Name = DiseaseRisk.SDanger },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Difficult, Name = DiseaseRisk.SDifficult },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Simple, Name = DiseaseRisk.SSimple }
                };
                model.Categories = await _context.Categories.Select(e => new RadioButtonViewModel { Id = e.Id, Name = e.Name }).ToListAsync();
                return View("DiseaseForm", model);
            }
            
            var diseaseExist = await _context.Diseases.FirstOrDefaultAsync(e => e.Name == model.Name && e.Id != model.Id);

            if (diseaseExist != null)
            {
                ModelState.AddModelError("Name", "هذا المرض موجود مسبقا!");
                model. DiseaseRisks = new List<RadioButtonViewModel>(){
                    new RadioButtonViewModel{ Id = DiseaseRisk.Danger, Name = DiseaseRisk.SDanger },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Difficult, Name = DiseaseRisk.SDifficult },
                    new RadioButtonViewModel{ Id = DiseaseRisk.Simple, Name = DiseaseRisk.SSimple }
                };
                model.Categories = await _context.Categories.Select(e => new RadioButtonViewModel { Id = e.Id, Name = e.Name }).ToListAsync();
                return View("DiseaseForm", model);
            }

            diseaseExist = await _context.Diseases.FindAsync(model.Id);
            diseaseExist.Name = model.Name;
            diseaseExist.CategoryId = model.CategoryId;
            diseaseExist.DiseaseRisk = model.DiseaseRisk;

            _context.Diseases.Update(diseaseExist);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("تم تعديل المرض بنجاح.");
            
            return RedirectToAction(nameof(Index));
        }

        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var diseaseExist = await _context.Diseases.FindAsync(id);

            if (diseaseExist == null)
                return NotFound();

            _context.Diseases.Remove(diseaseExist);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
