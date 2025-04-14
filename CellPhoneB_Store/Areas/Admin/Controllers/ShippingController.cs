using CellPhoneB_Store.Models;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CellPhoneB_Store.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Shipping")]
	[Authorize]
	public class ShippingController : Controller
	{
		private readonly DataContext _dataContext;
		public ShippingController(DataContext context)
		{
			_dataContext = context;
		}
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var shippingList = await _dataContext.Shippings.ToListAsync();
            ViewBag.Shippings = shippingList;
            return View();
        }
		[HttpPost]
		[Route("StoreShipping")]
		public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string ward, string district, string city, decimal price)
		{
            shippingModel.City = city;
            shippingModel.District = district;
            shippingModel.Ward = ward;
            shippingModel.Price = price;
			try
			{
				var existingShipping = await _dataContext.Shippings.AnyAsync(x => x.City == city && x.District == district && x.Ward == ward);
				if (existingShipping)
				{
					return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp" });
				}
				_dataContext.Shippings.Add(shippingModel);
				await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm shipping thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while adding shipping.");
            }
        }
        public async Task<IActionResult> Delete(int Id)
        {
            ShippingModel shipping = await _dataContext.Shippings.FindAsync(Id);

            _dataContext.Shippings.Remove(shipping);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Shipping đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
