namespace HW1NoteKeeperSolution
{
    /// <summary>
    /// Data Transfer Object (DTO) for updating an existing note.
    /// This class is used to receive note update requests from API clients.
    /// </summary>
    /// <remarks>
    /// Only the Summary and Details fields can be updated.
    /// Client-immutable fields like NoteId, CreatedDateUtc, ModifiedDateUtc, and Tags are not included
    /// as these are managed by the server.
    /// </remarks>
    public class NoteUpdate
    {
        //public string NoteId { get; set; }

        /// <summary>
        /// Gets or sets the updated summary of the note.
        /// </summary>
        /// <remarks>
        /// This property updates the brief title or heading of the note.
        /// </remarks>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the updated detailed content of the note.
        /// </summary>
        /// <remarks>
        /// This property updates the full text or body of the note.
        /// </remarks>
        public string Details { get; set; }

    }
}
