using LibraryManagementProject.Models;
using LibraryManagementProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementProject.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class BooksApiController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public BooksApiController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: api/BooksApi
            [HttpGet]
            public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
            {
                return await _context.Books
                    .Select(b => new BookDto
                    {
                        BookId = b.BookId,
                        Title = b.Title,
                        Author = b.Author,
                        Status = b.Available ? "Available" : "Borrowed"
                    })
                    .ToListAsync();
            }

            // GET: api/BooksApi/5
            [HttpGet("{id}")]
            public async Task<ActionResult<Book>> GetBook(int id)
            {
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return NotFound();
                }

                return book;
            }

            // GET: api/BooksApi/genre/{genre}
            [HttpGet("genre/{genre}")]
            public async Task<ActionResult<IEnumerable<Book>>> GetBooksByGenre(string genre)
            {
                return await _context.Books
                    .Where(b => b.Genre.ToLower() == genre.ToLower())
                    .ToListAsync();
            }

            // POST: api/BooksApi
            [HttpPost]
            public async Task<ActionResult<Book>> PostBook(Book book)
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBook", new { id = book.BookId }, book);
            }

            // PUT: api/BooksApi/5
            [HttpPut("{id}")]
            public async Task<IActionResult> PutBook(int id, Book book)
            {
                if (id != book.BookId)
                {
                    return BadRequest();
                }

                _context.Entry(book).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }

            // DELETE: api/BooksApi/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteBook(int id)
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool BookExists(int id)
            {
                return _context.Books.Any(e => e.BookId == id);
            }
        }
    }