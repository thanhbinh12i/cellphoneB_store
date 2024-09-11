using System.ComponentModel.DataAnnotations;

namespace CellPhoneB_Store.Models
{
	public class UserModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập username")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập Email"), EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password),Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		public string Password { get; set; }

	}
}
