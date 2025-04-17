using CellPhoneB_Store.Models;
using CellPhoneB_Store.Models.ViewModel;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CellPhoneB_Store.Controllers
{
	public class CartController : Controller
	{
		private readonly DataContext _dataContext;
		public CartController(DataContext context)
		{
			_dataContext = context;	
		}
		public async Task<IActionResult> Index()
		{
			List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart")
				?? new List<CartItemModel>();
			var shippingPriceCookie = Request.Cookies["ShippingPrice"];
			decimal shippingPrice = 0;
			if(shippingPriceCookie != null)
			{
				var shippingPriceJson = shippingPriceCookie;
				shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
			}
            var CouponName = Request.Cookies["CouponName"];
            var couponTitle = Request.Cookies["CouponTitle"];
            var coupon = await _dataContext.Coupons
                .FirstOrDefaultAsync(x => x.Name == CouponName);
            int percent = coupon?.PercentSale ?? 0;
            decimal GrandTotal = 0;
            if(percent > 0)
            {
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price) * (100 - percent) / 100;
            }else
            {
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price);
            }
            CartItemViewModel cartVM = new()
			{
				CartItems = cartItems,
				GrandTotal = GrandTotal,
				ShippingPrice = shippingPrice,
                CouponCode = couponTitle,
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
        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string tinh, string phuong)
        {

            var existingShipping = await _dataContext.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

            decimal shippingPrice = 0; // Set mặc định giá tiền

            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                //Set mặc định giá tiền nếu ko tìm thấy
                shippingPrice = 50000;
            }
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true // using HTTPS
                };

                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                //
                Console.WriteLine($"Error adding shipping price cookie: {ex.Message}");
            }
            return Json(new { shippingPrice });
        }
        [HttpPost]
        [Route("Cart/RemoveShippingCookie")]
        public IActionResult RemoveShippingCookie()
        {
            Response.Cookies.Delete("ShippingPrice");
            return Json(new { success = true });
        }
        [HttpPost]
        [Route("Cart/GetCoupon")]
        public async Task<IActionResult> GetCoupon(CouponModel couponModel, string coupon_value)
        {
            var validCoupon = await _dataContext.Coupons
                .FirstOrDefaultAsync(x => x.Name == coupon_value && x.Quantity >= 1);

            string couponName = validCoupon.Name;
            string couponTitle = validCoupon.Name + " | " + validCoupon?.Description;

            if (couponTitle != null)
            {
                TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;
                int daysRemaining = remainingTime.Days;

                if (daysRemaining >= 0)
                {
                    try
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict // Kiểm tra tính tương thích trình duyệt
                        };

                        Response.Cookies.Append("CouponName", couponName, cookieOptions);
                        Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);

                        return Ok(new { success = true, message = "Coupon applied successfully" });
                    }
                    catch (Exception ex)
                    {
                        //trả về lỗi 
                        Console.WriteLine($"Error adding apply coupon cookie: {ex.Message}");
                        return Ok(new { success = false, message = "Coupon applied failed" });
                    }
                }
                else
                {

                    return Ok(new { success = false, message = "Coupon has expired" });
                }

            }
            else
            {
                return Ok(new { success = false, message = "Coupon not existed" });
            }

            return Json(new { CouponTitle = couponTitle });
        }
    }
}
