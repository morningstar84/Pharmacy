using System.ComponentModel.DataAnnotations;
using Data.Models.Dtos;

namespace Data.Models
{
    public class Drug : PersistedEntity
    {
        public int Id { get; set; }

        [MaxLength(30)] public string Code { get; set; }

        [MaxLength(100)] public string Label { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool IsMatch(string? code, string? label)
        {
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(label)) return true;
            if (!string.IsNullOrEmpty(code)) return Code.ToLower().Contains(code.ToLower());

            if (!string.IsNullOrEmpty(label)) return Label.ToLower().Contains(label.ToLower());
            return false;
        }

        public static Drug NewInstance(DrugCreationDto dto)
        {
            return new Drug
            {
                Code = dto.Code,
                Label = dto.Label,
                Description = dto.Description,
                Price = dto.Price
            };
        }

        public static Drug NewInstance(DrugUpdateDto dto)
        {
            return new Drug
            {
                Id = dto.Id,
                Code = dto.Code,
                Label = dto.Label,
                Description = dto.Description,
                Price = dto.Price
            };
        }
    }
}