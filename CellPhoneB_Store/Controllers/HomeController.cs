using CellPhoneB_Store.Models;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CellPhoneB_Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,  DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public IActionResult Index()
        {
            var products = _dataContext.Products.Include("Category").Include("Brand").ToList();
			var sliders = _dataContext.Sliders.Where(s => s.Status == 1).ToList();
            ViewBag.Sliders = sliders;
			return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
		public async Task<IActionResult> Contact()
		{
			var contact = await _dataContext.Contact.FirstAsync();
			return View(contact);
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if(statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            
        }
    }
}
