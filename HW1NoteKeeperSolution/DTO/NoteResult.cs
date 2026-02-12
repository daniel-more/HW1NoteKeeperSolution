namespace HW1NoteKeeperSolution
{
    /// <summary>
    /// Data Transfer Object (DTO) for returning note data from API endpoints.
    /// This class represents the complete note information returned in API responses.
    /// </summary>
    public class NoteResult
    {
        /// <summary>
        /// Gets or sets the unique identifier for the note.
        /// </summary>
        /// <remarks>
        /// This is a GUID string that uniquely identifies the note in the system.
        /// </remarks>
        public string NoteId { get; set; }

        /// <summary>
        /// Gets or sets the summary or title of the note.
        /// </summary>
        /// <remarks>
        /// This is a brief heading that describes the note's main topic.
        /// </remarks>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the detailed content of the note.
        /// </summary>
        /// <remarks>
        /// This contains the full text or body of the note.
        /// </remarks>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the note was created (in UTC).
        /// </summary>
        /// <remarks>
        /// This is a read-only value set automatically when the note is created.
        /// </remarks>
        public DateTime CreatedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the note was last modified (in UTC).
        /// </summary>
        /// <remarks>
        /// This value is null if the note has never been modified after creation.
        /// It is updated automatically each time the note is changed.
        /// </remarks>
        public DateTime? ModifiedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets an array of tags associated with the note.
        /// </summary>
        /// <remarks>
        /// Tags are keywords extracted from the note's summary and details using AI processing.
        /// They are used for quick categorization and searching.
        /// </remarks>
        public string[] Tags { get; set; }
    }
}
