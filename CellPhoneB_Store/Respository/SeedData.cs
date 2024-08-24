using CellPhoneB_Store.Models;
using Microsoft.EntityFrameworkCore;

namespace CellPhoneB_Store.Respository
{
	public class SeedData
	{
		public static void SeedingData(DataContext _context)
		{
			_context.Database.Migrate();
			if (!_context.Products.Any())
			{
				CategoryModel iphone = new CategoryModel { Name = "Iphone", Slug = "Iphone", Description = "Product of apple", Status = 1 };
				CategoryModel samsung = new CategoryModel { Name = "Samsung", Slug = "samsung", Description = "Samsung is samsung manufacturing corporation", Status = 1 };
				BrandModel apple = new BrandModel { Name = "Apple", Slug = "apple", Description = "Apple is iphone manufacturing corporation", Status = 1 };
				_context.Products.AddRange(
					new ProductModel { Name = "Iphone 15 Promax" , Slug="Iphone15prm", Description="Product of apple", Image="ip15pm.jpg", Category=iphone, Brand=apple, Price=1500}
				);
				_context.SaveChanges();
			}
		}
	}
}
