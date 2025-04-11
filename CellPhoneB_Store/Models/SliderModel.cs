using CellPhoneB_Store.Respository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CellPhoneB_Store.Models
{
	public class SliderModel
	{
		[Key]
		public int Id	{ get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên slider")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập mô tả")]
		public string Description { get; set; }
		public int? Status { get; set; }
		public string Image {  get; set; }
		[NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set; }
	}
}
