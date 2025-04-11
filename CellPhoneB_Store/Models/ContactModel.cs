using CellPhoneB_Store.Respository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CellPhoneB_Store.Models
{
    public class ContactModel
    {
        [Key]
        [Required(ErrorMessage = "Yêu cầu nhập tiêu đề website")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập bản đồ")]
        public string Map { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        public string LogoImg { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
