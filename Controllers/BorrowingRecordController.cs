using Library_Project_Management.Data;
using Library_Project_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Project_Management.Controllers
{
    /// <summary>
    /// Controller for managing book borrowing records
    /// </summary>
    public class BorrowingRecordController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the BorrowingRecordController
        /// </summary>
        /// <param name="context">Database context for library operations</param>
        public BorrowingRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BorrowingRecord
        /// <summary>
        /// Displays all borrowing records
        /// </summary>
        /// <returns>View with list of all borrowing records</returns>
        public async Task<IActionResult> Index()
        {
            // Get all records with related book and member data
            var borrowingRecords = await _context.Borrowing_Records
                .Include(br => br.Book)
                .Include(br => br.Member)
                .ToListAsync();

            return View(borrowingRecords);
        }

        // GET: BorrowingRecord/Details/5
        /// <summary>
        /// Shows details of a specific borrowing record
        /// </summary>
        /// <param name="id">ID of the borrowing record</param>
        /// <returns>Detail view or NotFound if record doesn't exist</returns>
        public async Task<IActionResult> Details(int? id)
        {
            // Check if ID was provided
            if (id == null)
            {
                return NotFound();
            }

            // Find record with related data
            var borrowingRecord = await _context.Borrowing_Records
                .Include(br => br.Book)
                .Include(br => br.Member)
                .FirstOrDefaultAsync(m => m.RecordId == id);

            if (borrowingRecord == null)
            {
                return NotFound();
            }

            return View(borrowingRecord);
        }

        // GET: BorrowingRecord/Create
        /// <summary>
        /// Shows the form to create a new borrowing record
        /// </summary>
        /// <returns>Create view with dropdowns for available books and members</returns>
        public IActionResult Create()
        {
            // Prepare dropdown lists
            ViewBag.Books = _context.Books.Where(b => b.Available).ToList();
            ViewBag.Members = _context.Members.ToList();
            return View();
        }

        // POST: BorrowingRecord/Create
        /// <summary>
        /// Processes the creation of a new borrowing record
        /// </summary>
        /// <param name="borrowingRecord">The borrowing record data from form</param>
        /// <returns>Redirect to Index if successful, or back to form with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecordId,BookId,MemberId,BorrowDate,DueDate,Returned,LateFee")] Borrowing_Record borrowingRecord)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Mark book as unavailable when borrowed
                    var book = await _context.Books.FindAsync(borrowingRecord.BookId);
                    if (book != null)
                    {
                        book.Available = false;
                    }

                    // Set borrow date to current time
                    borrowingRecord.BorrowDate = DateTime.Now;

                    _context.Add(borrowingRecord);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log and show error if save fails
                    ModelState.AddModelError("", $"Error saving record: {ex.Message}");
                }
            }

            // If we got here, something failed - reload dropdowns
            ViewBag.Books = await _context.Books.Where(b => b.Available).ToListAsync();
            ViewBag.Members = await _context.Members.ToListAsync();
            return View(borrowingRecord);
        }

        // GET: BorrowingRecord/Edit/5
        /// <summary>
        /// Shows the edit form for a borrowing record
        /// </summary>
        /// <param name="id">ID of record to edit</param>
        /// <returns>Edit view or NotFound</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowingRecord = await _context.Borrowing_Records.FindAsync(id);
            if (borrowingRecord == null)
            {
                return NotFound();
            }

            return View(borrowingRecord);
        }

        // POST: BorrowingRecord/Edit/5
        /// <summary>
        /// Processes updates to a borrowing record
        /// </summary>
        /// <param name="id">ID of record being edited</param>
        /// <param name="borrowingRecord">Updated record data</param>
        /// <returns>Redirect to Index or back to form with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecordId,BookId,MemberId,BorrowDate,DueDate,Returned,LateFee")] Borrowing_Record borrowingRecord)
        {
            if (id != borrowingRecord.RecordId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if return status changed
                    var originalRecord = await _context.Borrowing_Records
                        .AsNoTracking()
                        .FirstOrDefaultAsync(br => br.RecordId == id);

                    if (originalRecord != null && originalRecord.Returned != borrowingRecord.Returned)
                    {
                        // Update book availability if return status changed
                        var book = await _context.Books.FindAsync(borrowingRecord.BookId);
                        if (book != null)
                        {
                            book.Available = !borrowingRecord.Returned;
                        }
                    }

                    _context.Update(borrowingRecord);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowingRecordExists(borrowingRecord.RecordId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(borrowingRecord);
        }

        // GET: BorrowingRecord/Delete/5
        /// <summary>
        /// Shows confirmation page for deleting a record
        /// </summary>
        /// <param name="id">ID of record to delete</param>
        /// <returns>Delete confirmation view or NotFound</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get record with related data for display
            var borrowingRecord = await _context.Borrowing_Records
                .Include(br => br.Book)
                .Include(br => br.Member)
                .FirstOrDefaultAsync(m => m.RecordId == id);

            if (borrowingRecord == null)
            {
                return NotFound();
            }

            return View(borrowingRecord);
        }

        // POST: BorrowingRecord/Delete/5
        /// <summary>
        /// Actually deletes a borrowing record
        /// </summary>
        /// <param name="id">ID of record to delete</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowingRecord = await _context.Borrowing_Records.FindAsync(id);
            if (borrowingRecord != null)
            {
                // If book wasn't returned, mark it as available again
                if (!borrowingRecord.Returned)
                {
                    var book = await _context.Books.FindAsync(borrowingRecord.BookId);
                    if (book != null)
                    {
                        book.Available = true;
                    }
                }

                _context.Borrowing_Records.Remove(borrowingRecord);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a borrowing record exists
        /// </summary>
        /// <param name="id">Record ID to check</param>
        /// <returns>True if record exists, false otherwise</returns>
        private bool BorrowingRecordExists(int id)
        {
            return _context.Borrowing_Records.Any(e => e.RecordId == id);
        }
    }
}