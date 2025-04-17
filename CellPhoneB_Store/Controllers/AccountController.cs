using CellPhoneB_Store.Models;
using CellPhoneB_Store.Models.ViewModel;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CellPhoneB_Store.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
        private readonly DataContext _dataContext;

        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
            _dataContext = context;
        }
        public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl});
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
				if (result.Succeeded)
				{
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Invalid UserName or password");
			}
			return View(loginVM);
		}
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName = user.UserName, Email = user.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
				if (result.Succeeded)
				{
					TempData["success"] = "Đăng ký thành công";
					return Redirect("/account/login");
				}
				foreach(IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}
		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);	
		}
		public async Task<IActionResult> History()
		{
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); 
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();
            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }
        public async Task<IActionResult> CancelOrder(string ordercode)
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var order = await _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
                order.status = 2;
                _dataContext.Update(order);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return BadRequest("An error occurred while canceling the order.");
            }
            return RedirectToAction("History", "Account");
        }
    }
}
