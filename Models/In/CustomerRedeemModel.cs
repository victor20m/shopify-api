using System.ComponentModel.DataAnnotations;

namespace Models.In
{
    public class CustomerRedeemModel
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public int Points { get; set; }
    }
}
