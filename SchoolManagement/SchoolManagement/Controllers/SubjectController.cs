using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Teachers)
                .Include(s => s.Students)
                .ToListAsync();
            return View(subjects);
        }

        // GET: Subjects/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.TeacherIds = new MultiSelectList(await _context.Teachers.ToListAsync(), "Id", "Name");
            ViewBag.StudentIds = new MultiSelectList(await _context.Students.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Class,Language,TeacherIds,StudentIds")] SubjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subject = new Subject
                {
                    Name = model.Name,
                    Class = model.Class,
                    Language = model.Language,
                    Teachers = _context.Teachers.Where(t => model.TeacherIds.Contains(t.Id)).ToList(),
                    Students = _context.Students.Where(s => model.StudentIds.Contains(s.Id)).ToList()
                };

                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TeacherIds = new MultiSelectList(await _context.Teachers.ToListAsync(), "Id", "Name", model.TeacherIds);
            ViewBag.StudentIds = new MultiSelectList(await _context.Students.ToListAsync(), "Id", "Name", model.StudentIds);
            return View(model);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Teachers)
                .Include(s => s.Students)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            var model = new SubjectViewModel
            {
                Id = subject.Id,
                Name = subject.Name,
                Class = subject.Class,
                Language = subject.Language,
                TeacherIds = subject.Teachers.Select(t => t.Id).ToList(),
                StudentIds = subject.Students.Select(s => s.Id).ToList()
            };

            ViewBag.TeacherIds = new MultiSelectList(await _context.Teachers.ToListAsync(), "Id", "Name", model.TeacherIds);
            ViewBag.StudentIds = new MultiSelectList(await _context.Students.ToListAsync(), "Id", "Name", model.StudentIds);
            return View(model);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Class,Language,TeacherIds,StudentIds")] SubjectViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var subject = await _context.Subjects
                        .Include(s => s.Teachers)
                        .Include(s => s.Students)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (subject == null)
                    {
                        return NotFound();
                    }

                    subject.Name = model.Name;
                    subject.Class = model.Class;
                    subject.Language = model.Language;
                    subject.Teachers = _context.Teachers.Where(t => model.TeacherIds.Contains(t.Id)).ToList();
                    subject.Students = _context.Students.Where(s => model.StudentIds.Contains(s.Id)).ToList();

                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["TeacherIds"] = new MultiSelectList(await _context.Teachers.ToListAsync(), "Id", "Name", model.TeacherIds);
            ViewData["StudentIds"] = new MultiSelectList(await _context.Students.ToListAsync(), "Id", "Name", model.StudentIds);
            return View(model);
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Teachers)
                .Include(s => s.Students)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
