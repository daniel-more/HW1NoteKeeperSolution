using System.ComponentModel.DataAnnotations;

namespace HW1NoteKeeperSolution
{
    /// <summary>
    /// Data Transfer Object (DTO) for creating a new note.
    /// This class is used to receive note creation requests from API clients.
    /// </summary>
    public class NoteCreate
    {

        /// <summary>
        /// Gets or sets the summary of the note.
        /// </summary>
        /// <remarks>
        /// This property is required and must be between 1 and 60 characters in length.
        /// It provides a brief title or heading for the note.
        /// </remarks>
        [Required]
        [MinLength(length: 1)]
        [MaxLength(length: 60)]
        public string? Summary { get; set; } = null!;

        /// <summary>
        /// Gets or sets the detailed content of the note.
        /// </summary>
        /// <remarks>
        /// This property is required and must be between 1 and 1024 characters in length.
        /// It contains the full text or body of the note.
        /// </remarks>
        [Required]
        [MinLength(length: 1)]
        [MaxLength(length: 1024)]
        public string? Details { get; set; } = null!;

        //public List<string> Tags { get; set; } = null!;

    }
}
