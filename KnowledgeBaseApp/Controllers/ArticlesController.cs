using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KnowledgeBaseApp.Data;
using KnowledgeBaseApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace KnowledgeBaseApp.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Тук "инжектираме" и мениджъра за потребители, за да знаем кой е логнат
        public ArticlesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Articles
        // Тук добавихме логика за ТЪРСЕНЕ (searchString)
        public async Task<IActionResult> Index(string searchString)
        {
            var articles = _context.Articles.Include(a => a.Author).Include(a => a.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s => s.Title.Contains(searchString) || s.Content.Contains(searchString));
            }

            return View(await articles.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null) return NotFound();

            // Увеличаваме брояча на преглежданията
            article.ViewCount++;
            _context.Update(article);
            await _context.SaveChangesAsync();

            return View(article);
        }

        // GET: Articles/Create
        [Authorize] // Само логнати могат да виждат тази страница
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Articles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CategoryId")] Article article)
        {
            // Махаме AuthorId от проверката на модела, защото ние ще го зададем ръчно
            ModelState.Remove("Author");
            ModelState.Remove("AuthorId");

            if (ModelState.IsValid)
            {
                // Взимаме ID-то на текущия потребител
                article.AuthorId = _userManager.GetUserId(User);
                article.CreatedOn = DateTime.Now;
                article.ViewCount = 0;

                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();

            // Проверка: Дали текущият потребител е авторът?
            var currentUserId = _userManager.GetUserId(User);
            if (article.AuthorId != currentUserId)
            {
                return Forbid(); // Забранено, ако не си авторът
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", article.CategoryId);
            return View(article);
        }

        // POST: Articles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CategoryId,CreatedOn,ViewCount,AuthorId")] Article article)
        {
            if (id != article.Id) return NotFound();

            ModelState.Remove("Author");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null) return NotFound();

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}