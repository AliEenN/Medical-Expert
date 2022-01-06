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
    public class MedicineFormsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public MedicineFormsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.MedicineForms.Select(e => new MedicineFormViewModel { Id = e.Id, Name = e.Name }).OrderBy(e => e.Name).ToListAsync());
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View("MedicineFormView", new MedicineFormViewModel()
            {
                Medicines = await _context.Medicines.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync()
            });
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicineFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Medicines = await _context.Medicines.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync();
                return View("MedicineFormView", model);
            }

            var medicineFormExsis = await _context.MedicineForms.FirstOrDefaultAsync(e => e.Name == model.Name);

            if (medicineFormExsis != null)
            {
                ModelState.AddModelError("Name", "شكل العلاج هذا موجودة مسبقا!");
                model.Medicines = await _context.Medicines.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = false }).ToListAsync();
                return View("MedicineFormView", model);
            }

            await _context.MedicineForms.AddAsync(new MedicineForm() { Name = model.Name });
            _context.SaveChanges();

            if (model.Medicines != null)
            {
                var currentMedicineForm = await _context.MedicineForms.FirstAsync(e => e.Name == model.Name);
                var medicinesAndMedicineFormsList = new List<MedicineAndMedicineForm>();

                foreach (var medicine in model.Medicines)
                {
                    if (medicine.IsSelected)
                    {
                        medicinesAndMedicineFormsList.Add(new MedicineAndMedicineForm { MedicineFormId = currentMedicineForm.Id, MedicineId = medicine.Id });
                    }
                }

                await _context.MedicinesAndMedicineForms.AddRangeAsync(medicinesAndMedicineFormsList);
                _context.SaveChanges();
            }

            _toastNotification.AddSuccessToastMessage("تم حفظ شكل العلاج بنجاح.");

            return RedirectToAction(nameof(Index));
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineFormExsis = await _context.MedicineForms.FindAsync(id);

            if (medicineFormExsis == null)
                return NotFound();

            return View(new MedicineFormViewModel()
            {
                Id = medicineFormExsis.Id,
                Name = medicineFormExsis.Name,
                Medicines = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineFormId == medicineFormExsis.Id).Include(e => e.Medicine).Select(e => new CheckBoxViewModel { Id = e.MedicineId, Name = e.Medicine.Name, IsSelected = true }).ToListAsync()
            });
        }
        
        // GET
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineFormExsis = await _context.MedicineForms.FindAsync(id);

            if (medicineFormExsis == null)
                return NotFound();

            var medicinesList = await _context.Medicines.ToListAsync();
            var medicinesIds = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineFormId == medicineFormExsis.Id).Select(e => e.MedicineId).ToListAsync();

            return View("MedicineFormView", new MedicineFormViewModel()
            {
                Id = medicineFormExsis.Id,
                Name = medicineFormExsis.Name,
                Medicines = medicinesList.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = medicinesIds.Contains(e.Id) }).ToList()
            });
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicineFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var medicinesList = await _context.Medicines.ToListAsync();
                var medicinesIds = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineFormId == model.Id).Select(e => e.MedicineId).ToListAsync();
                model.Medicines = medicinesList.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = medicinesIds.Contains(e.Id) }).ToList();
                return View("MedicineFormView", model);
            }
            
            var isMedicineFormExsis = await _context.MedicineForms.FirstOrDefaultAsync(e => e.Name == model.Name && e.Id != model.Id);

            if (isMedicineFormExsis != null)
            {
                ModelState.AddModelError("Name", "شكل العلاج موجود مسبقا.");
                var medicinesList = await _context.Medicines.ToListAsync();
                var medicinesIds = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineFormId == model.Id).Select(e => e.MedicineId).ToListAsync();
                model.Medicines = medicinesList.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Name, IsSelected = medicinesIds.Contains(e.Id) }).ToList();
                return View("MedicineFormView", model);
            }

            isMedicineFormExsis = await _context.MedicineForms.FindAsync(model.Id);
            isMedicineFormExsis.Name = model.Name;

            if (model.Medicines != null)
            {
                var medicineIdsList = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineFormId == isMedicineFormExsis.Id).Select(e => e.MedicineId).ToListAsync();

                var medicineFormMedicinesToDelete = new List<MedicineAndMedicineForm>();
                var medicineFormMedicinesToAdd = new List<MedicineAndMedicineForm>();

                foreach (var medicine in model.Medicines)
                {
                    if (medicineIdsList.Contains(medicine.Id) && !medicine.IsSelected)
                    {
                        medicineFormMedicinesToDelete.Add(new MedicineAndMedicineForm { MedicineFormId = isMedicineFormExsis.Id, MedicineId = medicine.Id });
                    }

                    if (!medicineIdsList.Contains(medicine.Id) && medicine.IsSelected)
                    {
                        medicineFormMedicinesToAdd.Add(new MedicineAndMedicineForm { MedicineFormId = isMedicineFormExsis.Id, MedicineId = medicine.Id });
                    }
                }

                _context.MedicinesAndMedicineForms.RemoveRange(medicineFormMedicinesToDelete);
                _context.MedicinesAndMedicineForms.AddRange(medicineFormMedicinesToAdd);
            }
            
            _context.MedicineForms.Update(isMedicineFormExsis);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("تم تعديل شكل العلاج بنجاح.");
            
            return RedirectToAction(nameof(Index));
        }
        
        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var medicineFormExsis = await _context.MedicineForms.FindAsync(id);

            if (medicineFormExsis == null)
                return NotFound();

            _context.MedicineForms.Remove(medicineFormExsis);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
