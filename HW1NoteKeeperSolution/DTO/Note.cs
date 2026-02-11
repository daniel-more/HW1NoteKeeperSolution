namespace HW1NoteKeeperSolution
{
    /// <summary>
    /// Represents a note entity in the system.
    /// This class is used to store and retrieve note data from the repository.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Gets or sets the unique identifier for the note.
        /// </summary>
        /// <remarks>
        /// This is typically a GUID string generated when the note is created.
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
        /// This value is set automatically when the note is created and should not be modified afterward.
        /// </remarks>
        public DateTime CreatedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the note was last modified (in UTC).
        /// </summary>
        /// <remarks>
        /// This value is null until the note is modified. It is updated each time the note is changed.
        /// </remarks>
        public DateTime? ModifiedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets an array of tags associated with the note.
        /// </summary>
        /// <remarks>
        /// Tags are keywords extracted from the note's summary and details for quick categorization and search.
        /// </remarks>
        public string[] Tags { get; set; }
    }
}