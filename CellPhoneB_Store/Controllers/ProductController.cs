using CellPhoneB_Store.Models;
using CellPhoneB_Store.Models.ViewModel;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CellPhoneB_Store.Controllers
{
	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		public ProductController(DataContext context)
		{
			_dataContext = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Details(int Id)
		{
			if(Id == null)
			{
				return RedirectToAction("Index");
			}
			var productById = _dataContext.Products.Include(p => p.Ratings).Where(p => p.Id == Id).FirstOrDefault();

            var relatedProducts = await _dataContext.Products
            .Where(p => p.CategoryId == productById.CategoryId && p.Id != productById.Id)
            .Take(4)
            .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;
			var viewModel = new ProductDetailsViewModel
			{
				ProductDetails = productById,
			};
            return View(viewModel);
		}
		public async Task<IActionResult> Search(string searchTerm)
		{
			var products = await _dataContext.Products.Where(p => p.Name.Contains(searchTerm)).ToListAsync();
			ViewBag.Keyword = searchTerm;

			return View(products);
		}
		public async Task<IActionResult> CommentProduct(RatingModel rating)
		{
			if (ModelState.IsValid)
			{

				var ratingEntity = new RatingModel
				{
					ProductId = rating.ProductId,
					Name = rating.Name,
					Email = rating.Email,
					Comment = rating.Comment,
					Star = rating.Star

				};

				_dataContext.Ratings.Add(ratingEntity);
				await _dataContext.SaveChangesAsync();

				TempData["success"] = "Thêm đánh giá thành công";

				return Redirect(Request.Headers["Referer"]);
			}
			else
			{
				TempData["error"] = "Model có một vài thứ đang lỗi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);

				return RedirectToAction("Detail", new { id = rating.ProductId });
			}

			return Redirect(Request.Headers["Referer"]);
		}
	}
}
