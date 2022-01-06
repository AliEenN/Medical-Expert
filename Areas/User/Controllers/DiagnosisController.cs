using MedicalExpert.Constants;
using MedicalExpert.Data;
using MedicalExpert.Models;
using MedicalExpert.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Areas.User.Controllers
{
    [Area("User")]
    public class DiagnosisController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DiagnosisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(new DiagnosisViewModel() { Categories = await _context.Categories.OrderBy(e => e.Name).ToListAsync() });
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> DefineSides(int? id)
        {
            if (id == null)
                BadRequest();

            var categoryExist = await _context.Categories.FindAsync(id);

            if (categoryExist == null)
                return NotFound();

            var sides = await _context.CategoriesSides.Include(e => e.Side).Where(e => e.CategoryId == categoryExist.Id).Select(e => e.Side).ToListAsync();

            return View(new DiagnosisDefineSidesViewModel() { CategoryId = categoryExist.Id, Category = categoryExist, SidesCheckBoxes = sides.Select(e => new CheckBoxViewModel { Id = e.Id, Name = e.Description, IsSelected = false }).ToList() });
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> DefineSides(DiagnosisDefineSidesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Category = await _context.Categories.FindAsync(model.CategoryId);
                return View(model);
            }

            var category = await _context.Categories.FindAsync(model.CategoryId);
            var countAllSides = model.SidesCheckBoxes.Count;
            var countChecedSides = model.SidesCheckBoxes.Where(e => e.IsSelected).ToList().Count;

            if (countChecedSides == 0)
            {
                ModelState.AddModelError("SidesCheckBoxes", "لم يتم تحديد اي اعراض حتي الان.");
                model.Category = category;
                return View(model);
            }

            if (countChecedSides <= 5)
            {
                ModelState.AddModelError("SidesCheckBoxes", "لم يتم تحديد مرضك بعد ,يرجي اختيار اعراض اكثر ,ويفضل زيارة طبيبك.");
                model.Category = category;
                return View(model);
            }

            var diseasesList = await _context.Diseases.Where(e => e.CategoryId == category.Id).ToListAsync();

            if (diseasesList == null)
            {
                ModelState.AddModelError("SidesCheckBoxes", "لم يتم تحديد مرضك ,ربما انت بخير ,ويفضل زيارة طبيبك.");
                model.Category = category;
                return View(model);
            }

            List<Disease> specificDiseasesList = new List<Disease>();

            if (countChecedSides <= 15)
            {
                specificDiseasesList = diseasesList.Where(e => e.DiseaseRisk == DiseaseRisk.Simple).ToList();
            }
            else if (countChecedSides <= 25)
            {
                specificDiseasesList = diseasesList.Where(e => e.DiseaseRisk == DiseaseRisk.Difficult).ToList();
                if (specificDiseasesList.Count == 0)
                {
                    specificDiseasesList = diseasesList.Where(e => e.DiseaseRisk == DiseaseRisk.Simple).ToList();
                }
            }
            else
            {
                specificDiseasesList = diseasesList.Where(e => e.DiseaseRisk == DiseaseRisk.Danger).ToList();
                if (specificDiseasesList.Count == 0)
                {
                    specificDiseasesList = diseasesList.Where(e => e.DiseaseRisk == DiseaseRisk.Difficult).ToList();
                }
            }

            if (specificDiseasesList.Count == 0)
            {
                ModelState.AddModelError("SidesCheckBoxes", "لم يتم تحديد مرضك ,عليك زيارة طبيبك.");
                model.Category = category;
                return View(model);
            }

            List<Disease> moreSpecificDiseasesList = new List<Disease>();

            if (countChecedSides <= 10)
            {
                moreSpecificDiseasesList.Add(specificDiseasesList.First());
            }
            else if (countChecedSides <= 15)
            {
                moreSpecificDiseasesList.Add(specificDiseasesList.First());
                if (specificDiseasesList.Count > 1)
                {
                    moreSpecificDiseasesList.Add(specificDiseasesList.Last());
                }
            }
            else if (countChecedSides <= 20)
            {
                moreSpecificDiseasesList.Add(specificDiseasesList.First());
            }
            else if (countChecedSides <= 25)
            {
                moreSpecificDiseasesList.Add(specificDiseasesList.First());
                if (specificDiseasesList.Count > 1)
                {
                    moreSpecificDiseasesList.Add(specificDiseasesList.Last());
                }
            }
            else if (countChecedSides <= 30)
            {
                moreSpecificDiseasesList.Add(specificDiseasesList.First());
            }
            else
            {
                moreSpecificDiseasesList.Add(specificDiseasesList.First());
                if (specificDiseasesList.Count > 1)
                {
                    moreSpecificDiseasesList.Add(specificDiseasesList.Last());
                }
            }

            DiseasAndMedicineViewModel diseasAndMedicineViewModel = new DiseasAndMedicineViewModel();
            var medicineList = new List<Medicine>();

            foreach (var disease in moreSpecificDiseasesList)
            {
                medicineList.AddRange(await _context.DiseasesMedicines.Where(e => e.DiseaseId == disease.Id).Include(e => e.Medicine).Select(e => e.Medicine).ToListAsync());
            }

            if (medicineList.Count == 0)
            {
                diseasAndMedicineViewModel.NotFoundMedicines = "لتأكيد هذا التشخيص المبدئي واعطائك العلاج ,يجب عليك زيارة طبيب " + category.Name + ".";
            }
            else
            {
                medicineList = medicineList.GroupBy(e => e.Id).Select(g => g.First()).ToList();

                foreach (var medicine in medicineList)
                {
                    medicine.MedicineForms = await _context.MedicinesAndMedicineForms.Where(e => e.MedicineId == medicine.Id).Include(e => e.MedicineForm).Select(e => e.MedicineForm).ToListAsync();
                    medicine.MedicineGenre = await _context.MedicineGenres.FindAsync(medicine.MedicineGenreId);
                }

                diseasAndMedicineViewModel.Medicines = medicineList;
            }

            diseasAndMedicineViewModel.Diseases = moreSpecificDiseasesList;

            return View("DiagnosisResults", diseasAndMedicineViewModel);
        }
    }
}
