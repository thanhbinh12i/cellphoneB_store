using CellPhoneB_Store.Models;
using CellPhoneB_Store.Models.ViewModel;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CellPhoneB_Store.Controllers
{
	public class CartController : Controller
	{
		private readonly DataContext _dataContext;
		public CartController(DataContext context)
		{
			_dataContext = context;	
		}
		public IActionResult Index()
		{
			List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart")
				?? new List<CartItemModel>();
			CartItemViewModel cartVM = new()
			{
				CartItems = cartItems,
				GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
			};
			return View(cartVM);
		}
		public async Task<IActionResult> Add(long id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(id);
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart")
				?? new List<CartItemModel>();
			CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new CartItemModel(product));
			}
			else
			{
				cartItem.Quantity += 1;
			}
			HttpContext.Session.SetJson("Cart", cart);
			TempData["success"] = "Add Item to Cart Successfully";
			return Redirect(Request.Headers["Referer"].ToString());
		}

		public async Task<IActionResult> Decrease(long id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if(cartItem.Quantity > 1)
			{
				--cartItem.Quantity;
			}
			else
			{
				cart.RemoveAll(p => p.ProductId == id);
			}
			if(cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}
            TempData["success"] = "Decrease Item Quantity Successfully";
            return RedirectToAction("Index");
		}
		public async Task<IActionResult> Increase(long id)
		{
            ProductModel product = await _dataContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem.Quantity >= 1 && product.Quantity > cartItem.Quantity)
			{
				++cartItem.Quantity;
                TempData["success"] = "Increase Product to cart Sucessfully! ";
            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["success"] = "Maximum Product Quantity to cart Sucessfully! ";
            }
            if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}
            TempData["success"] = "Increase Item Quantity Successfully";
            return RedirectToAction("Index");
		}
		public async Task<IActionResult> Remove(long id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			cart.RemoveAll(p => p.ProductId == id);
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}
            TempData["success"] = "Remove item in cart Successfully";
            return RedirectToAction("Index");
		}
		public async Task<IActionResult> Clear()
		{
			HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear cart Successfully";
            return RedirectToAction("Index");
		}
	}
}
