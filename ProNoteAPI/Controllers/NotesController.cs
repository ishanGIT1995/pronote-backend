using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProNoteAPI.Data;
using ProNoteAPI.DTO;
using ProNoteAPI.Models.Domain;

namespace ProNoteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NoteDbContext _noteDbContext;

        public NotesController(NoteDbContext noteDbContext)
        {
            _noteDbContext = noteDbContext;
        }


        // Get All Notes

        [HttpGet]
        [Route("getAllNotes")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            // Get Data from Database to Domain Models
            var notesDomain = await _noteDbContext.Notes.ToListAsync();

             // Map Domain models to DTOs
             var notesDto = new List<NoteDto>();
            foreach (var noteDomain in notesDomain)
            {
                notesDto.Add(new NoteDto
                {
                    Id = noteDomain.Id,
                    Title = noteDomain.Title,
                    Description = noteDomain.Description,

                });

            }

            // Return DTOs
            return Ok(notesDto);
        }

        // Get Note using Id

        [HttpGet]
        [Route("{id:Guid}")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public async Task<ActionResult<Note>> GetNote(Guid id)
        {
            // Get Note Domain model from Database
            var noteDomain = await _noteDbContext.Notes.FindAsync(id);

            if (noteDomain == null)
            {
                return NotFound("note not found");
            }

            // Map Note Domain models to Note DTO
            var noteDto = new NoteDto
            {
                Id = noteDomain.Id,
                Title = noteDomain.Title,
                Description = noteDomain.Description,

            };

            // Return DTO back to client
            return Ok(noteDto);
        }



        // Create Note

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<Note>> CreateNote(AddNoteRequestDto addNoteRequestDto)
        {
            // Map AddNoteRequestDto DTO to Domain Model
            var noteDomainModel = new Note
            {
                Title = addNoteRequestDto.Title,
                Description = addNoteRequestDto.Description,
            };

            //  Use Domain Model to Create Note
            await _noteDbContext.Notes.AddAsync(noteDomainModel);
            await _noteDbContext.SaveChangesAsync();

            // Map Domain model back to DTO
            var noteDto = new NoteDto
            {
                Id = noteDomainModel.Id,
                Title = noteDomainModel.Title,
                Description= noteDomainModel.Description,

            };

            return CreatedAtAction(nameof(GetNote), new { id = noteDto.Id }, noteDto);
        }


        // Update Note
        [HttpPut]
        [Route("{id:Guid}")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<Note>> UpdateNote(UpdateNoteRequestDto updateNoteRequestDto , Guid id)
        {
            // check if  Note exists in the database  
            var existingNote = await _noteDbContext.Notes.FindAsync(id);

            if (existingNote == null)
            {
                return NotFound("Note was not found");
            }

            // Map DTO to Domain model
            existingNote.Title = updateNoteRequestDto.Title;
            existingNote.Description = updateNoteRequestDto.Description;

            try
            {
                await _noteDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            // convert Domain model to Dto
            var noteDto = new NoteDto
            {
                Id = existingNote.Id,
                Title = existingNote.Title,
                Description = existingNote.Description,

            };

            return Ok(noteDto);
        }


        // Delete a Note
        [HttpDelete]
        [Route("{id:Guid}")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Note>> DeleteNote(Guid id)
        {
            if (_noteDbContext.Notes == null)
            {
                return NotFound();
                
            }
            // noteToDelete
            var noteToDelete = await _noteDbContext.Notes.FindAsync(id);
            if (noteToDelete == null)
            {
                return NotFound();
                
            }

            _noteDbContext.Notes.Remove(noteToDelete);
            await _noteDbContext.SaveChangesAsync();

            return Ok(noteToDelete);


        }

        // Delete  Multiple Notes
        [HttpDelete]
        [Route("delete-multiple")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<Note>>> DeleteMultipleNotes([FromBody] DeleteMultipleRequestDto deleteMultipleRequestDto)
        {
            if (deleteMultipleRequestDto == null || deleteMultipleRequestDto.RecordIds == null || deleteMultipleRequestDto.RecordIds.Count == 0)
            {
                return BadRequest("No record IDs provided for deletion.");
            }

            var notesToDelete = await _noteDbContext.Notes
                .Where(note => deleteMultipleRequestDto.RecordIds.Contains(note.Id))
                .ToListAsync();

            if (notesToDelete == null || !notesToDelete.Any())
            {
                return NotFound("No matching records found for deletion.");
            }

            _noteDbContext.Notes.RemoveRange(notesToDelete);
            await _noteDbContext.SaveChangesAsync();

            return Ok(notesToDelete);
        }







    }
}
