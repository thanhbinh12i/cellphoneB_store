using System.ComponentModel.DataAnnotations;

namespace CellPhoneB_Store.Models
{
	public class CouponModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên coupon")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập mô tả")]
		public string Description { get; set; }
		public DateTime DateStart { get; set; }
		public DateTime DateExpired { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập phần trăm giảm giá")]
        public int PercentSale { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số lượng")]
		public int Quantity { get; set; }
		public int Status { get; set; }

	}
}
