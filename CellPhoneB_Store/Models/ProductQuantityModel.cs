using System.ComponentModel.DataAnnotations;

namespace CellPhoneB_Store.Models
{
    public class ProductQuantityModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số lượng")]
        public int Quantity { get; set; }
        public long ProductId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
