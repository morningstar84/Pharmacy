using System.ComponentModel.DataAnnotations;

namespace Data.Models.Dtos
{
    public class DrugUpdateDto : DrugCreationDto
    {
        [Required] public int Id { get; set; }
    }
}