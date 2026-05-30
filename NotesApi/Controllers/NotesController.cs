using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApi.Data;
using NotesApi.Models;
using System.Security.Claims;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("/notes")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = GetUserId();
            var notes = _context.Notes.Where(n => n.UserId == userId).ToList();
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = GetUserId();
            var note = _context.Notes.FirstOrDefault(n => n.Id == id && n.UserId == userId);

            if (note == null) return NotFound();

            return Ok(note);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Note newNote)
        {
            var userId = GetUserId();

            newNote.UserId = userId;

            _context.Notes.Add(newNote);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = newNote.Id }, newNote);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Note updatedNote)
        {
            var userId = GetUserId();

            var note = _context.Notes.FirstOrDefault(n => n.Id == id);

            if (note == null) return NotFound();

            if (note.UserId != userId) return StatusCode(403);

            note.Content = updatedNote.Content;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userId = GetUserId();

            var note = _context.Notes.FirstOrDefault(n => n.Id == id);

            if (note == null) return NotFound();

            if (note.UserId != userId) return StatusCode(403);

            _context.Notes.Remove(note);
            _context.SaveChanges();

            return NoContent();
        }
    }
}