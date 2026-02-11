using Microsoft.AspNetCore.Mvc;

namespace HW1NoteKeeperSolution.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    //[Route("api/[controller]")]
    [Route("notes")]
    public class NotesController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];
        private static readonly Dictionary<string, Note> _note = new Dictionary<string, Note>();

        private const string GetAllNotesRouteName = "GetAllNotes";
        private const string GetNotesRouteName = "GetNoteById";   // For GET by Id
        private const string CreateNotesRouteName = "CreateNotes";  // For POST
        private const string DeleteNotesRouteName = "DeleteNotes";  // For DELETE

        [HttpGet("{id}", Name = GetNotesRouteName)]
        public ActionResult<Note> GetAWeatherForecastById(string id)
        {
            try
            {
                if (id.Contains("BadRobot"))
                {
                    throw new Exception("Error simulation");
                }

                if (_note.ContainsKey(id))
                {
                    return _note[id];
                }
            }
            catch (Exception ex)
            {
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


        private const string PatchNoteRouteName = "PatchNoteRouteName";

        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [HttpPatch("{id}", Name = PatchNoteRouteName)]
        public ActionResult Patch(string id, [FromBody] NoteUpdate noteUpdate)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Id must not be null or empty",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(id)
                });
            }

            if (!_note.TryGetValue(id, out var existing))
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(noteUpdate.Summary))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteUpdate.Summary)
                });
            }

            if (string.IsNullOrWhiteSpace(noteUpdate.Details))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteUpdate.Details)
                });
            }


            bool changed = false;


            // Update the entry with values provided, skip update where null
            if (!string.IsNullOrWhiteSpace(noteUpdate.Summary) &&
        noteUpdate.Summary != existing.Summary)
            {
                existing.Summary = noteUpdate.Summary;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(noteUpdate.Details) &&
                noteUpdate.Details != existing.Details)
            {
                existing.Details = noteUpdate.Details;
                changed = true;
            }

            if (changed)
            {
                existing.ModifiedDateUtc = DateTime.UtcNow;
                return NoContent();
            }


            return Ok(_note[id]);
        }


        /// <summary>
        /// Creates a new note entry and adds it to the list of notes
        /// </summary>
        /// <param name="CreateNotes">The weather forecast to ad</param>
        /// <response code="201">Indicates the notes added successfully</response>
        /// 
        [ProducesResponseType(type: typeof(NoteResult), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(type: typeof(ErrorResponse), statusCode: StatusCodes.Status400BadRequest)]
        [HttpPost(Name = CreateNotesRouteName)]
        public ActionResult Post([FromBody] NoteCreate noteCreate)

        {
            // Validate input
            if (noteCreate == null)  // this doesn't get trigger, instead I get  "The Summary field is required."
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input body must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteCreate)
                });
            }

            if (string.IsNullOrWhiteSpace(noteCreate.Summary))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteCreate.Summary)
                });
            }

            if (string.IsNullOrWhiteSpace(noteCreate.Details))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Input must not be null",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(noteCreate.Details)
                });
            }

            // Create note
            Note note = new Note()
            {
                Summary = noteCreate.Summary,
                Details = noteCreate.Details,
                CreatedDateUtc = DateTime.UtcNow,
                ModifiedDateUtc = null,
                Tags = null

            };

            string newNoteId = Guid.NewGuid().ToString();
            note.NoteId = newNoteId;
            _note.Add(newNoteId, note);



            // Create result
            NoteResult noteResult = new NoteResult()
            {
                NoteId = newNoteId,
                Summary = note.Summary,
                Details = note.Details,
                CreatedDateUtc = note.CreatedDateUtc,
                ModifiedDateUtc = null, //note.ModifiedDateUtc, check why 201 return a value
                Tags = note.Tags
            };


            return CreatedAtRoute(GetNotesRouteName, routeValues: new { id = newNoteId }, value: noteResult);
        }

        [HttpDelete("{id}", Name = DeleteNotesRouteName)]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Id must not be null or empty",
                    ErrorNumber = ErrorNumbers.MustNotBeNull,
                    PropertyName = nameof(id)
                });
            }

            if (!_note.ContainsKey(id))
            {
                return NotFound();
            }
            _note.Remove(id);
            return NoContent();
        }

        [HttpGet(Name = GetAllNotesRouteName)]
        public ActionResult GetAllNotes()
        {
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
