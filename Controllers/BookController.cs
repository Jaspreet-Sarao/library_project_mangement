using Microsoft.AspNetCore.Mvc;
using Library_Project_Management.Models;
using Library_Project_Management.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Project_Management.Controllers
{
    /// <summary>
    /// Controller for managing books in the library system
    /// </summary>
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the BookController
        /// </summary>
        /// <param name="context">Database context for library operations</param>
        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Book
        /// <summary>
        /// Displays the list of all books in the library
        /// </summary>
        /// <returns>View containing all books</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Retrieve all books from the database asynchronously
            return View(await _context.Books.ToListAsync());
        }

        // GET: Book/Details/5
        /// <summary>
        /// Shows detailed information about a specific book
        /// </summary>
        /// <param name="id">The ID of the book to display</param>
        /// <returns>Detailed view of the book or NotFound</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            // Check if ID was provided
            if (id == null)
            {
                return NotFound();
            }

            // Find the book by ID
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        /// <summary>
        /// Displays the form for adding a new book
        /// </summary>
        /// <returns>Empty book creation form</returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        /// <summary>
        /// Processes the creation of a new book
        /// </summary>
        /// <param name="book">The book data submitted from the form</param>
        /// <returns>Redirect to Index if successful, or back to form with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,Author,Genre,Available")] Book book)
        {
            // Validate the model before processing
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the new book to the context
                    _context.Add(book);
                    // Save changes to the database
                    await _context.SaveChangesAsync();
                    // Redirect to the list of books
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log and display any errors
                    ModelState.AddModelError("", $"Error saving book: {ex.Message}");
                }
            }
            // If validation fails, return to the form with the entered data
            return View(book);
        }

        // GET: Book/Edit/5
        /// <summary>
        /// Displays the form for editing an existing book
        /// </summary>
        /// <param name="id">The ID of the book to edit</param>
        /// <returns>Edit form with current book data or NotFound</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the book by ID
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Edit/5
        /// <summary>
        /// Processes updates to an existing book
        /// </summary>
        /// <param name="id">The ID of the book being edited</param>
        /// <param name="book">The updated book data</param>
        /// <returns>Redirect to Index if successful, or back to form with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Author,Genre,Available")] Book book)
        {
            // Verify the ID matches the book being edited
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Find the existing book in the database
                    var existingBook = await _context.Books.FindAsync(id);
                    if (existingBook == null)
                    {
                        return NotFound();
                    }

                    // Update only the editable properties
                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.Genre = book.Genre;
                    existingBook.Available = book.Available;

                    // Mark the entity as modified
                    _context.Update(existingBook);
                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency conflicts
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(book);
        }

        // GET: Book/Delete/5
        /// <summary>
        /// Displays confirmation for deleting a book
        /// </summary>
        /// <param name="id">The ID of the book to delete</param>
        /// <returns>Delete confirmation view or NotFound</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the book by ID
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        /// <summary>
        /// Processes the deletion of a book
        /// </summary>
        /// <param name="id">The ID of the book to delete</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the book by ID
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                // Remove the book from the context
                _context.Books.Remove(book);
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a book exists in the database
        /// </summary>
        /// <param name="id">The ID of the book to check</param>
        /// <returns>True if the book exists, false otherwise</returns>
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}