using CellPhoneB_Store.Models;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CellPhoneB_Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;
		private readonly UserManager<AppUserModel> _userManager;

		public HomeController(ILogger<HomeController> logger,  DataContext context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _dataContext = context;
			_userManager = userManager;

		}

		public IActionResult Index()
        {
            var products = _dataContext.Products.Include("Category").Include("Brand").ToList();
			var sliders = _dataContext.Sliders.Where(s => s.Status == 1).ToList();
            ViewBag.Sliders = sliders;
			return View(products);
        }
		[HttpPost]
		public async Task<IActionResult> AddWishList(long Id, WishlistModel wishlist)
        {
            var user = await _userManager.GetUserAsync(User);
			var wishlistProduct = new WishlistModel
			{
				ProductId = Id,
				UserId = user.Id
			};

			_dataContext.Wishlist.Add(wishlistProduct);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Add to wishlisht Successfully" });
			}
			catch (Exception)
			{
				return StatusCode(500, "An error occurred while adding to wishlist table.");
			}
		}
		[HttpPost]
		public async Task<IActionResult> AddCompare(long Id)
		{
			var user = await _userManager.GetUserAsync(User);

			var compareProduct = new CompareModel
			{
				ProductId = Id,
				UserId = user.Id
			};

			_dataContext.Compares.Add(compareProduct);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Add to compare Successfully" });
			}
			catch (Exception)
			{
				return StatusCode(500, "An error occurred while adding to compare table.");
			}

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
