using Microsoft.AspNetCore.Identity;

namespace CellPhoneB_Store.Models
{
	public class AppUserModel : IdentityUser
	{
		public string Occupation { get; set; }
	}
}
