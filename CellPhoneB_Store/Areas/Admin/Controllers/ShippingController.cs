using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
		public IActionResult Index()
		{
			return View();
		}
	}
}
