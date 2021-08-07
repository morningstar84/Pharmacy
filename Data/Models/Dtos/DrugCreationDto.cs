using System.ComponentModel.DataAnnotations;

namespace Data.Models.Dtos
{
    public class DrugCreationDto
    {
        [Required]
        public string Token { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 0, ErrorMessage = "Drug code must be at maximum 30 chars")]
        public string Code { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "Drug label must be at maximum 100 chars")]
        public string Label { get; set; }

        [Required] public string Description { get; set; }

        [Required] [Range(0, double.MaxValue)] public double Price { get; set; }
    }
}