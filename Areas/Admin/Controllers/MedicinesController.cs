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
    public class MedicinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public MedicinesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Medicines.Select(e => new MedicineViewModel { Id = e.Id, Name = e.Name }).OrderBy(e => e.Name).ToListAsync());
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View("MedicineForm", new MedicineViewModel()
            {
                Diseases = await _context.Diseases.OrderBy(e => e.Name).Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync(),
                MedicineForms = await _context.MedicineForms.OrderBy(e => e.Name).Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync(),
                MedicineGenres = await _context.MedicineGenres.Select(e => new RadioButtonViewModel { Id = e.Id, Name = e.Name }).ToListAsync()            
            });
        }
        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Diseases = await _context.Diseases.OrderBy(e => e.Name).Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync();
                model.MedicineForms = await _context.MedicineForms.OrderBy(e => e.Name).Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync();
                return View("MedicineForm", model);
            }

            var medicineExsis = await _context.Medicines.FirstOrDefaultAsync(e => e.Name == model.Name);

            if (medicineExsis != null)
            {
                ModelState.AddModelError("Name", "هذا العلاج موجودة مسبقا");
                model.Diseases = await _context.Diseases.OrderBy(e => e.Name).Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync();
                model.MedicineForms = await _context.MedicineForms.OrderBy(e => e.Name).Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync();
                return View("MedicineForm", model);
            }

            await _context.Medicines.AddAsync(new Medicine() { Name = model.Name, MedicineDose = model.MedicineDose, MedicineGenreId = model.MedicineGenreId });
            _context.SaveChanges();

            var currentMedicine = await _context.Medicines.FirstAsync(e => e.Name == model.Name);

            if (model.Diseases != null)
            {
                var diseasesMedicinesList = new List<DiseaseMedicine>();

                foreach (var diseas in model.Diseases)
                {
                    if (diseas.IsSelected)
                    {
                        diseasesMedicinesList.Add(new DiseaseMedicine { MedicineId = currentMedicine.Id, DiseaseId = diseas.Id });
                    }
                }

                await _context.DiseasesMedicines.AddRangeAsync(diseasesMedicinesList);
                _context.SaveChanges();
            }

            if (model.MedicineForms != null)
            {
                var medicinesAndMedicineFormsList = new List<MedicineAndMedicineForm>();

                foreach (var medicineForms in model.MedicineForms)
                {
                    if (medicineForms.IsSelected)
                    {
                        medicinesAndMedicineFormsList.Add(new MedicineAndMedicineForm { MedicineId = currentMedicine.Id, MedicineFormId = medicineForms.Id });   
                    }
                }

                await _context.MedicinesAndMedicineForms.AddRangeAsync(medicinesAndMedicineFormsList);
                _context.SaveChanges();
            }

            _toastNotification.AddSuccessToastMessage("تم اضافة العلاج بنجاح.");

            return RedirectToAction(nameof(Index));
        }
        
        // GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineExsis = await _context.Medicines.FindAsync(id);

            if (medicineExsis == null)
                return NotFound();

            return View(new MedicineViewModel()
            {
                Id = medicineExsis.Id,
                Name = medicineExsis.Name,
                MedicineDose = medicineExsis.MedicineDose,
                Diseases = await _context.DiseasesMedicines.Where(e => e.MedicineId == medicineExsis.Id).Include(e => e.Disease).Select(e => new CheckBoxViewModel { Name = e.Disease.Name }).ToListAsync(),
                MedicineForms = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineId == medicineExsis.Id).Include(e => e.MedicineForm).Select(e => new CheckBoxViewModel { Id = e.MedicineFormId, Name = e.MedicineForm.Name }).ToListAsync(),
                MedicineGenre = await _context.MedicineGenres.FindAsync(medicineExsis.MedicineGenreId)
            });
        }
        
        // GET
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineExsis = await _context.Medicines.FindAsync(id);

            if (medicineExsis == null)
                return NotFound();

            var diseasesList = await _context.Diseases.ToListAsync();
            var diseasesIds = await _context.DiseasesMedicines.Where(e => e.MedicineId == medicineExsis.Id).Select(e => e.DiseaseId).ToListAsync();

            var medicineFormsList = await _context.MedicineForms.ToListAsync();
            var medicineFormsIds = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineId == medicineExsis.Id).Select(e => e.MedicineFormId).ToListAsync();

            return View("MedicineForm", new MedicineViewModel()
            {
                Id = medicineExsis.Id,
                Name = medicineExsis.Name,
                MedicineDose = medicineExsis.MedicineDose,
                MedicineGenreId = medicineExsis.MedicineGenreId,
                Diseases = diseasesList.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = diseasesIds.Contains(e.Id) }).ToList(),
                MedicineForms = medicineFormsList.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = medicineFormsIds.Contains(e.Id) }).ToList(),
                MedicineGenres = await _context.MedicineGenres.Select(e => new RadioButtonViewModel { Id = e.Id, Name = e.Name }).ToListAsync()
            });
        }
        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.MedicineGenre = await _context.MedicineGenres.FindAsync(model.MedicineGenreId);
                return View("MedicineForm", model);
            }

            var medicineExsis = await _context.Medicines.FirstOrDefaultAsync(e => e.Name == model.Name && e.Id != model.Id);
            
            if (medicineExsis != null)
            {
                ModelState.AddModelError("Name", "هذا الدواء موجود مسبقا.");
                model.MedicineGenre = await _context.MedicineGenres.FindAsync(model.MedicineGenreId);
                return View("MedicineForm", model);
            }

            medicineExsis = await _context.Medicines.FindAsync(model.Id);
            medicineExsis.Name = model.Name;
            medicineExsis.MedicineDose = model.MedicineDose;
            medicineExsis.MedicineGenreId = model.MedicineGenreId;

            var medicineFormsIds = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineId == medicineExsis.Id).Select(e => e.MedicineFormId).ToListAsync();

            var medicinesAndMedicineFormsToDelete = new List<MedicineAndMedicineForm>();
            var medicinesAndMedicineFormsToAdd = new List<MedicineAndMedicineForm>();

            foreach (var medicineForms in model.MedicineForms)
            {
                if (medicineFormsIds.Contains(medicineForms.Id) && !medicineForms.IsSelected)
                {
                    medicinesAndMedicineFormsToDelete.Add(new MedicineAndMedicineForm { MedicineId = medicineExsis.Id, MedicineFormId = medicineForms.Id });
                }

                if (!medicineFormsIds.Contains(medicineForms.Id) && medicineForms.IsSelected)
                {
                    medicinesAndMedicineFormsToAdd.Add(new MedicineAndMedicineForm { MedicineId = medicineExsis.Id, MedicineFormId = medicineForms.Id });
                }
            }

            var diseasesIds = await _context.DiseasesMedicines.Where(e => e.MedicineId == medicineExsis.Id).Select(e => e.DiseaseId).ToListAsync();

            var diseasesMedicinesToDelete = new List<DiseaseMedicine>();
            var diseasesMedicinesToAdd = new List<DiseaseMedicine>();

            foreach (var disease in model.Diseases)
            {
                if (diseasesIds.Contains(disease.Id) && !disease.IsSelected)
                {
                    diseasesMedicinesToDelete.Add(new DiseaseMedicine { MedicineId = medicineExsis.Id, DiseaseId = disease.Id });
                }

                if (!diseasesIds.Contains(disease.Id) && disease.IsSelected)
                {
                    diseasesMedicinesToAdd.Add(new DiseaseMedicine { MedicineId = medicineExsis.Id, DiseaseId = disease.Id });
                }
            }

            _context.MedicinesAndMedicineForms.RemoveRange(medicinesAndMedicineFormsToDelete);
            _context.MedicinesAndMedicineForms.AddRange(medicinesAndMedicineFormsToAdd);
            _context.DiseasesMedicines.RemoveRange(diseasesMedicinesToDelete);
            _context.DiseasesMedicines.AddRange(diseasesMedicinesToAdd);
            _context.Medicines.Update(medicineExsis);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("تم تعديل الدواء بنجاح.");
            
            return RedirectToAction(nameof(Index));
        }
        
        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineExsis = await _context.Medicines.FindAsync(id);

            if (medicineExsis == null)
                return NotFound();

            _context.Medicines.Remove(medicineExsis);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
