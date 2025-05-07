using Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs;
using MyBackedApi.Services;

namespace MyBackedApi.Controllers
{
    [Authorize(Roles = "STUDENT")]
    [ApiController]
    [Route("api/notes")]
    public class NotesController : BaseApiController
    {
        private readonly NotesService _service;

        public NotesController(NotesService service)
        {
            _service = service;
        }

        [HttpGet("get-notes")]
        public async Task<ActionResult<List<NoteDto>>> GetNotes()
        {
            var studentId = GetUserIdFromToken();
            return Ok(await _service.GetNotesForStudent(studentId));
        }

        [HttpPost("create-note")]
        public async Task<ActionResult<NoteDto>> AddNote([FromBody] CreateNoteDto dto)
        {
            var studentId = GetUserIdFromToken();
            return Ok(await _service.CreateNote(dto, studentId));
        }

        [HttpDelete("delete-note/{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            await _service.DeleteNote(id);
            return Ok(new BaseResponseEmpty()
            {
                Message = "Note deleted!"
            });
        }

        [HttpPut("update-note")]
        public async Task<ActionResult<NoteDto>> UpdateNote([FromBody] NoteDto dto)
        {
            return Ok(await _service.UpdateNote(dto));
        }
    }

}
