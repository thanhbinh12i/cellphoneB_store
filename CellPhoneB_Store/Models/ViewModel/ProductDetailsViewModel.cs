using System.ComponentModel.DataAnnotations;

namespace CellPhoneB_Store.Models.ViewModel
{
	public class ProductDetailsViewModel
	{
		public ProductModel ProductDetails { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập đánh giá")]
		public string Comment { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập email")]
		public string Email { get; set; }
	}
}
