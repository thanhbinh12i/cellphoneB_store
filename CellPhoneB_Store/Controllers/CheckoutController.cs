﻿using CellPhoneB_Store.Areas.Admin.Repository;
using CellPhoneB_Store.Models;
using CellPhoneB_Store.Respository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CellPhoneB_Store.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IEmailSender _emailSender;
		public CheckoutController(DataContext context, IEmailSender emailSender)
		{
			_dataContext = context;
			_emailSender = emailSender;
		}
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Checkout()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if(userEmail == null)
			{
				return RedirectToAction("Login", "Account");
			}
			else
			{
				var orderCode = Guid.NewGuid().ToString();
				var orderItem = new OrderModel();
				orderItem.OrderCode = orderCode;
				orderItem.UserName = userEmail;
				orderItem.status = 1;
				orderItem.CreateDate = DateTime.Now;
				_dataContext.Add(orderItem);
				_dataContext.SaveChanges();
				List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var item in cartItems)
                {
					var orderdetails = new OrderDetails();
					orderdetails.UserName = userEmail;
					orderdetails.OrderCode = orderCode;
					orderdetails.ProductId = item.ProductId;
					orderdetails.Price = item.Price;
					orderdetails.Quantity = item.Quantity;
					_dataContext.Add(orderdetails);
					_dataContext.SaveChanges();
                }
				HttpContext.Session.Remove("Cart");

				var receiver = "nptbinh17092004@gmail.com";

                var subject = "Đặt hàng thành công";
				var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé.";

				await _emailSender.SendEmailAsync(receiver, subject, message);
				TempData["success"] = "Check out thành công, vui lòng chờ duyệt đơn hàng";
				return RedirectToAction("Index","Cart");
			}
			return View();
		}
	}
}
