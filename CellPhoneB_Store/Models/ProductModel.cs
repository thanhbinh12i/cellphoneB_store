using System.ComponentModel.DataAnnotations;

namespace CellPhoneB_Store.Models
{
	public class ProductModel
	{
		[Key]
		public int Id { get; set; }
		[Required, MinLength(4, ErrorMessage =" Yêu cầu nhập tên Sản phẩm")]
		public string Name { get; set; }
		public string Slug { get; set; }
		[Required, MinLength(4, ErrorMessage = " Yêu cầu nhập Mô tả Sản phẩm")]
		public string Description { get; set; }
		[Required, MinLength(4, ErrorMessage = " Yêu cầu nhập tên giá Sản phẩm")]
		public decimal Price { get; set; }
		public int BrandId { get; set; }
		public int CategoryId { get; set; }
		public CategoryModel Category { get; set; }
		public BrandModel Brand { get; set; }
		public string Image { get; set; }
			}
}
