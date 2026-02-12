using Microsoft.AspNetCore.Mvc;

namespace HW1NoteKeeperSolution.Controllers
{
    /// <summary>
    /// NotesController manages all HTTP endpoints for the Notes API.
    /// Provides functionality to create, read, update, and delete notes.
    /// Each note can be tagged and categorized using AI-powered tag generation.
    /// </summary>
    [ApiController]
    //[Route("[controller]")]
    //[Route("api/[controller]")]
    [Route("notes")]
    public class NotesController : ControllerBase
    {
        /// <summary>
        /// Service for processing messages and generating tags using AI/Chat capabilities
        /// </summary>
        private readonly IChatService _chatService;

        /// <summary>
        /// Constructor that initializes the controller with the ChatService dependency
        /// </summary>
        /// <param name="chatService">The chat service instance for AI-powered tag generation</param>
        public NotesController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// In-memory dictionary storing all notes. Key is the NoteId (GUID), value is the Note object.
        /// Note: This data is ephemeral and will be lost when the application stops.
        /// In a production environment, this should be replaced with a persistent database.
        /// </summary>
        private static readonly Dictionary<string, Note> _note = new Dictionary<string, Note>();


        /// <summary>
        /// Route name constants used for CreatedAtRoute and other URL generation scenarios.
        /// These names are referenced when creating links to related resources.
        /// </summary>
        private const string GetAllNotesRouteName = "GetAllNotes";
        private const string GetNotesRouteName = "GetNoteById";   // For GET by Id
        private const string CreateNotesRouteName = "CreateNotes";  // For POST
        private const string DeleteNotesRouteName = "DeleteNotes";  // For DELETE

        /// <summary>
        /// HTTP GET endpoint to retrieve a single note by its ID.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the note to retrieve</param>
        /// <returns>
        /// 200 OK: Returns the Note object if found
        /// 404 Not Found: If the note ID does not exist
        /// 500 Internal Server Error: If an unexpected error occurs during processing
        /// </returns>
        [HttpGet("{id}", Name = GetNotesRouteName)]
        public ActionResult<Note> GetAWeatherForecastById(string id)
        {
            try
            {
                // Simulate an error condition for testing/debugging purposes
                if (id.Contains("BadRobot"))
                {
                    throw new Exception("Error simulation");
                }

                // Check if the note exists and return it
                if (_note.ContainsKey(id))
                {
                    return _note[id];
                }
            }
            catch (Exception)
            {
                // Generate a unique causality ID for error tracking and support purposes
                string causalityId = Guid.NewGuid().ToString();
                //_logger.LogCritical(exception: ex, message: "The causality ID is {id}", args: causalityId);
                // Don't return raw exception information to the caller
                // Note: An internally referenceable causality id that could be tied to internally logged information
                //       is recommended so developers can find and resolve the root cause of the issue.
                return StatusCode(StatusCodes.Status500InternalServerError, $"We are sorry experiencing technical difficulties at this time! Provide this number to tech support {causalityId}");
            }

            return NotFound();
        }



        ///// <summary>
        ///// Get a list of notes
        ///// </summary>

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<NoteCreate> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new NoteCreate
        //    {
        //        //Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        //TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //        //Details = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}


        /// <summary>
        /// Route name constant for the PATCH endpoint
        /// </summary>
        private const string PatchNoteRouteName = "PatchNoteRouteName";

        /// <summary>
        /// HTTP PATCH endpoint to partially update an existing note.
        /// Only the fields provided in the request body are updated; other fields remain unchanged.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the note to update</param>
        /// <param name="noteUpdate">The data containing the fields to update (Summary and/or Details)</param>
        /// <returns>
        /// 204 No Content: If the note was successfully updated
        /// 404 Not Found: If the note ID does not exist
        /// 400 Bad Request: If validation fails (null/empty ID, Summary, or Details)
        /// </returns>
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [HttpPatch("{id}", Name = PatchNoteRouteName)]
        public ActionResult Patch(string id, [FromBody] NoteUpdate noteUpdate)
        {
            // Validate that the ID is provided
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Id must not be null or empty",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(id)
                });
            }

            // Verify the note exists before attempting to update
            if (!_note.TryGetValue(id, out var existing))
            {
                return NotFound();
            }

            // Validate the Summary field
            if (string.IsNullOrWhiteSpace(noteUpdate.Summary))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteUpdate.Summary)
                });
            }

            // Validate the Details field
            if (string.IsNullOrWhiteSpace(noteUpdate.Details))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteUpdate.Details)
                });
            }

            // Track whether any changes were made
            bool changed = false;

            // Update the Summary if it has changed
            // Update the entry with values provided, skip update where null
            if (!string.IsNullOrWhiteSpace(noteUpdate.Summary) &&
        noteUpdate.Summary != existing.Summary)
            {
                existing.Summary = noteUpdate.Summary;
                changed = true;
            }

            // Update the Details if it has changed
            if (!string.IsNullOrWhiteSpace(noteUpdate.Details) &&
                noteUpdate.Details != existing.Details)
            {
                existing.Details = noteUpdate.Details;
                changed = true;
            }

            // If any changes were made, update the modification timestamp and return 204 No Content
            if (changed)
            {
                existing.ModifiedDateUtc = DateTime.UtcNow;
                return NoContent();
            }

            // Return the updated note if no changes were detected
            return Ok(_note[id]);
        }


        /// <summary>
        /// HTTP POST endpoint to create a new note.
        /// The request body must contain Summary and Details.
        /// AI-powered tags are automatically generated based on the note content.
        /// </summary>
        /// <param name="noteCreate">The note creation request containing Summary and Details</param>
        /// <returns>
        /// 201 Created: Returns the newly created NoteResult with a Location header pointing to the created resource
        /// 400 Bad Request: If validation fails (null/empty Summary or Details)
        /// </returns>
        [ProducesResponseType(type: typeof(NoteResult), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(type: typeof(ErrorResponse), statusCode: StatusCodes.Status400BadRequest)]
        [HttpPost(Name = CreateNotesRouteName)]
        public ActionResult Post([FromBody] NoteCreate noteCreate)
        {
            // Validate that the input object is not null
            // Note: This validation may not trigger due to model binding, instead the individual field validations below will catch issues
            if (noteCreate == null)  // this doesn't get trigger, instead I get  "The Summary field is required."
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input body must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteCreate)
                });
            }

            // Validate that Summary is provided and not empty
            if (string.IsNullOrWhiteSpace(noteCreate.Summary))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteCreate.Summary)
                });
            }

            // Validate that Details are provided and not empty
            if (string.IsNullOrWhiteSpace(noteCreate.Details))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteCreate.Details)
                });
            }

            // Generate tags using the AI chat service
            // Process the note details to extract and generate relevant tags
            // string textForTagging = $"{noteCreate.Summary} {noteCreate.Details}";
            // List<string> generatedTags = await _chatService.GenerateTagsAsync(textForTagging);

            var tags = _chatService.ProcessMessage($"{noteCreate.Details}").Result; // Call the service to process the message, you can await this if it's an async method

            // Create the Note object with the provided information
            Note note = new Note()
            {
                Summary = noteCreate.Summary,
                Details = noteCreate.Details,
                CreatedDateUtc = DateTime.UtcNow,
                ModifiedDateUtc = null,
                Tags = tags.ToArray()
            };

            // Generate a unique identifier (GUID) for the new note
            string newNoteId = Guid.NewGuid().ToString();
            note.NoteId = newNoteId;

            // Add the note to the in-memory repository
            _note.Add(newNoteId, note);

            // Create the response object containing the created note information
            NoteResult noteResult = new NoteResult()
            {
                NoteId = newNoteId,
                Summary = note.Summary,
                Details = note.Details,
                CreatedDateUtc = note.CreatedDateUtc,
                ModifiedDateUtc = null, //note.ModifiedDateUtc, check why 201 return a value
                Tags = note.Tags
            };

            // Return 201 Created with Location header pointing to the newly created resource
            return CreatedAtRoute(GetNotesRouteName, routeValues: new { id = newNoteId }, value: noteResult);
        }

        /// <summary>
        /// HTTP DELETE endpoint to remove a note by its ID.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the note to delete</param>
        /// <returns>
        /// 204 No Content: If the note was successfully deleted
        /// 404 Not Found: If the note ID does not exist
        /// 400 Bad Request: If the ID is null or empty
        /// </returns>
        [HttpDelete("{id}", Name = DeleteNotesRouteName)]
        public ActionResult Delete(string id)
        {
            // Validate that the ID is provided
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Id must not be null or empty",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(id)
                });
            }

            // Check if the note exists before attempting to delete
            if (!_note.ContainsKey(id))
            {
                return NotFound();
            }

            // Remove the note from the repository
            _note.Remove(id);
            return NoContent();
        }

        /// <summary>
        /// HTTP GET endpoint to retrieve all notes.
        /// </summary>
        /// <returns>
        /// 200 OK: Returns an array of all NoteResult objects currently stored
        /// </returns>
        [HttpGet(Name = GetAllNotesRouteName)]
        public ActionResult GetAllNotes()
        {
            // Retrieve all notes from the repository and map them to NoteResult objects
            return Ok((from note in _note
                       select new NoteResult()
                       {
                           NoteId = note.Value.NoteId,
                           Summary = note.Value.Summary,
                           Details = note.Value.Details,
                           CreatedDateUtc = note.Value.CreatedDateUtc,
                           ModifiedDateUtc = note.Value.ModifiedDateUtc,
                           Tags = note.Value.Tags
                       }).ToArray());
        }
    }
}
