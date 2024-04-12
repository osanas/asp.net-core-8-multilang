using DemoRoutes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace DemoRoutes.Controllers
{
    [Route("")]
    //[Route("{culture=en-CA}")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //[Route("{culture=en-CA}/home", Name = "HomeIndexEnglish")]
        //[Route("{culture=fr-CA}/accueil", Name = "HomeIndexFrench")]
        [Route("{culture}/home")]
        public IActionResult Index()
        {
            return View();
        }

        //[Route("{culture=fr}/politique")]
        //[Route("{culture=en}/privacy")]
        [Route("{culture}/privacy")]
        public IActionResult Privacy()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            var currentUICulture = CultureInfo.CurrentUICulture.Name;

            // Utilisez _logger ou ViewBag pour inspecter les valeurs
            _logger.LogInformation($"Culture actuelle : {currentCulture}, Culture UI actuelle : {currentUICulture}");
            ViewBag.CurrentCulture = currentCulture;
            ViewBag.CurrentUICulture = currentUICulture;
            return View();
        }

        //[Route("{culture:culture=fr}/profile")]
        //[Route("{culture:culture=en}/profile")]
        [HttpGet("{culture}/contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}