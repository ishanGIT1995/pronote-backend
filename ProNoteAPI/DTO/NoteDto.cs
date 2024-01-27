using System.ComponentModel.DataAnnotations;

namespace ProNoteAPI.DTO
{
    public class NoteDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
