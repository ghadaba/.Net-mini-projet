using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjet.Constants;
using MiniProjet.data;
using MiniProjet.Models;

namespace MiniProjet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeDashboardViewModel
            {
                TotalArticles = await _context.Articles.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                EstConnecte = User.Identity?.IsAuthenticated == true,
                NomUtilisateur = User.Identity?.Name
            };

            if (User.IsInRole(RoleNames.Admin))
                model.RolePrincipal = RoleNames.Admin;
            else if (User.IsInRole(RoleNames.User))
                model.RolePrincipal = RoleNames.User;

            model.ArticlesParCategorie = await _context.Categories
                .Select(c => new CategoryStat
                {
                    NomCategorie = c.Name,
                    NombreArticles = c.Articles.Count
                })
                .OrderByDescending(x => x.NombreArticles)
                .ToListAsync();

            model.DerniersArticles = await _context.Articles
                .Include(a => a.Category)
                .OrderByDescending(a => a.DateCreated)
                .Take(5)
                .ToListAsync();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
