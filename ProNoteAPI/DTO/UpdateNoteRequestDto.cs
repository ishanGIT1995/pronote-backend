using System.ComponentModel.DataAnnotations;

namespace ProNoteAPI.DTO
{
    public class UpdateNoteRequestDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
