namespace HW1NoteKeeperSolution
{
    /// <summary>
    /// Defines standard error number constants used throughout the application for error handling and responses.
    /// </summary>
    /// <remarks>
    /// These constants are used in ErrorResponse objects to provide structured error codes that clients can use
    /// to identify and handle specific error conditions programmatically.
    /// </remarks>
    public static class ErrorNumbers
    {
        /// <summary>
        /// Represents an unknown or unspecified error.
        /// </summary>
        /// <remarks>
        /// Use this value when an error occurs that doesn't fit into any of the defined error categories.
        /// Value: 0
        /// </remarks>
        public const int Unknown = 0;

        /// <summary>
        /// Represents a successful operation with no errors.
        /// </summary>
        /// <remarks>
        /// This is typically used to indicate that a request was processed successfully.
        /// Value: 1
        /// </remarks>
        public const int Success = 1;

        /// <summary>
        /// Represents an error where a required field or input is null.
        /// </summary>
        /// <remarks>
        /// Use this when validation detects that a required property or parameter is missing or null.
        /// Value: 400
        /// </remarks>
        public const int MustNotBeNull = 400;
    }
}