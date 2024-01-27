using System.ComponentModel.DataAnnotations;

namespace ProNoteAPI.Models.Domain
{
    public class Note

    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

    }
}
