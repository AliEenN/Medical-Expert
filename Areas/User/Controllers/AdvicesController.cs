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

namespace MedicalExpert.Areas.User.Controllers
{
    [Area("User")]
    public class AdvicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public AdvicesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Advices.Select(e => new AdviceViewModel { Id = e.Id, Title = e.Title, Description = e.Description, Picture = e.Picture }).OrderBy(e => e.Title).ToListAsync());
        }

        // GET
        [HttpGet]
        public IActionResult Create()
        {
            return View("AdviceForm", new AdviceViewModel());
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdviceViewModel model)
        {
            if (!ModelState.IsValid)
                return View("AdviceForm", model);

            var existAdvice = await _context.Advices.FirstOrDefaultAsync(e => e.Title == model.Title);

            if (existAdvice != null)
            {
                ModelState.AddModelError("Title", "هذه النصيحة موجودة مسبقا");
                return View("AdviceForm", model);
            }

            var files = Request.Form.Files;

            var picture = files.FirstOrDefault();

            if (picture == null)
            {
                ModelState.AddModelError("Picture", "يجب اختيار صوره");
                return View("AdviceForm", model);
            }

            if (picture.Length > 1048576)
            {
                ModelState.AddModelError("Picture", "يجب ان تكون الصورة اقل من 1 ميجا");
                return View("AdviceForm", model);
            }

            using var dataStream = new MemoryStream();
            await picture.CopyToAsync(dataStream);

            var newAdvice = new Advice()
            {
                Title = model.Title,
                Description = model.Description,
                Picture = dataStream.ToArray()
            };

            await _context.Advices.AddAsync(newAdvice);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("تم انشاء النصيحة بنجاح");

            return RedirectToAction(nameof(Index));
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var advice = await _context.Advices.FindAsync(id);

            if (advice == null)
                return NotFound();

            var adviceVM = new AdviceViewModel()
            {
                Id = advice.Id,
                Title = advice.Title,
                Description = advice.Description,
                Picture = advice.Picture
            };

            return View(adviceVM);
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var adviceExist = await _context.Advices.FindAsync(id);

            if (adviceExist == null)
                return BadRequest();

            var adviceVM = new AdviceViewModel()
            {
                Id = adviceExist.Id,
                Title = adviceExist.Title,
                Description = adviceExist.Description,
                Picture = adviceExist.Picture
            };

            return View("AdviceForm", adviceVM);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdviceViewModel model)
        {
            if (!ModelState.IsValid)
                return View("AdviceForm", model);

            var dublictedAdvice = await _context.Advices.FirstOrDefaultAsync(e => e.Title == model.Title && e.Id != model.Id);
            var existAdvice = await _context.Advices.FindAsync(model.Id);

            if (dublictedAdvice != null)
            {
                ModelState.AddModelError("Title", "هذة النصيحة موجوده مسبقا");
                return View("AdviceForm", model);
            }

            var files = Request.Form.Files;

            byte[] nPicture = existAdvice.Picture;

            if (files.Any())
            {
                //ModelState.AddModelError("Picture", "يجب اختيار صورة");
                //return View("AdviceForm", model);

                var picture = files.FirstOrDefault();

                if (picture.Length > 1048576)
                {
                    ModelState.AddModelError("Picture", "يجب ان تكون الصورة اقل من 1 ميجا");
                    return View("AdviceForm", model);
                }

                using var dataStream = new MemoryStream();
                await picture.CopyToAsync(dataStream);

                nPicture = dataStream.ToArray();
            }

            existAdvice.Id = model.Id;
            existAdvice.Title = model.Title;
            existAdvice.Description = model.Description;
            existAdvice.Picture = nPicture;

            _context.Advices.Update(existAdvice);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("تم تعديل النصيحة بنجاح");

            return RedirectToAction(nameof(Index));
        }

        // POST
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var adviceExist = await _context.Advices.FindAsync(id);

            if (adviceExist == null)
                return NotFound();

            _context.Advices.Remove(adviceExist);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
