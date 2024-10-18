using CellPhoneB_Store.Models;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CellPhoneB_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userWithRoles = await (from u in _dataContext.Users
                                       join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                       join r in _dataContext.Roles on ur.RoleId equals r.Id
                                       select new { User = u, RoleName = r.Name }).ToListAsync();
            return View(userWithRoles);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var role = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(role, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email);
                    var userId = createUser.Id;
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, _roleManager.FindByIdAsync(user.RoleId).Result.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(createUserResult);
                    return View(user);
                }


            }
            else
            {
                TempData["error"] = "Model error";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            var role = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(role, "Id", "Name");
            return View(user);
        }

        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "Xóa user thành công";
            return RedirectToAction("Index");
        }
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, AppUserModel userModel)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                user.UserName = userModel.UserName;
                user.Email = userModel.Email;
                user.PhoneNumber = userModel.PhoneNumber;
                user.RoleId = userModel.RoleId;
                var updateUserResult = await _userManager.UpdateAsync(user);
                if (updateUserResult.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(updateUserResult);
                    return View(user);
                }
            }
            var role = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(role, "Id", "Name");
            TempData["error"] = "Model validation failed";
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage).ToList());
            var errorMessage = string.Join("\n", errors);
            return View(user);
        }
    }
}
