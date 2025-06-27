
using LibraryManagementProject.Data;
using LibraryManagementProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementProject.Controllers
{
    /// <summary>
    /// Controller for managing borrowing records in the library
    /// </summary>
    public class BorrowingRecordController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the BorrowingRecordController
        /// </summary>
        /// <param name="context">Database context</param>
        public BorrowingRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays all borrowing records
        /// </summary>
        /// <returns>List view of records</returns>
        public async Task<IActionResult> Index()
        {
            var borrowingRecords = await _context.Borrowing_Records
                .Include(br => br.Book)
                .Include(br => br.Member)
                .ToListAsync();

            return View(borrowingRecords);
        }

        /// <summary>
        /// Shows details of a specific borrowing record
        /// </summary>
        /// <param name="id">ID of the borrowing record</param>
        /// <returns>Detail view or NotFound</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.Borrowing_Records
                .Include(br => br.Book)
                .Include(br => br.Member)
                .FirstOrDefaultAsync(br => br.RecordId == id);

            if (record == null) return NotFound();

            return View(record);
        }

        /// <summary>
        /// Shows form to create a new borrowing record
        /// </summary>
        /// <returns>Create view</returns>
        public IActionResult Create()
        {
            ViewBag.Books = _context.Books.Where(b => b.Available).ToList();
            ViewBag.Members = _context.Members.ToList();
            return View();
        }

        /// <summary>
        /// Handles submission of new borrowing record
        /// </summary>
        /// <param name="record">The borrowing record model</param>
        /// <returns>Redirect to index if successful, otherwise view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,MemberId,DueDate")] Borrowing_Record record)
        {
            if (ModelState.IsValid)
            {
                var book = await _context.Books.FindAsync(record.BookId);
                if (book != null) book.Available = false;

                record.BorrowDate = DateTime.Now;
                record.Returned = false;
                record.LateFee = 0;

                _context.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Books = await _context.Books.Where(b => b.Available).ToListAsync();
            ViewBag.Members = await _context.Members.ToListAsync();
            return View(record);
        }

        /// <summary>
        /// Displays form to edit a borrowing record
        /// </summary>
        /// <param name="id">ID of the borrowing record</param>
        /// <returns>Edit view or NotFound</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.Borrowing_Records.FindAsync(id);
            if (record == null) return NotFound();

            return View(record);
        }

        /// <summary>
        /// Handles submission of edits to a borrowing record
        /// </summary>
        /// <param name="id">ID of the record</param>
        /// <param name="record">Updated record</param>
        /// <returns>Redirect or edit view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecordId,BookId,MemberId,BorrowDate,DueDate,Returned,LateFee")] Borrowing_Record record)
        {
            if (id != record.RecordId) return NotFound();

            if (ModelState.IsValid)
            {
                var original = await _context.Borrowing_Records.AsNoTracking().FirstOrDefaultAsync(br => br.RecordId == id);

                if (original != null && original.Returned != record.Returned)
                {
                    var book = await _context.Books.FindAsync(record.BookId);
                    if (book != null) book.Available = !record.Returned;
                }

                _context.Update(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(record);
        }

        /// <summary>
        /// Shows confirmation page for deleting a borrowing record
        /// </summary>
        /// <param name="id">ID of the record</param>
        /// <returns>Delete view or NotFound</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.Borrowing_Records
                .Include(br => br.Book)
                .Include(br => br.Member)
                .FirstOrDefaultAsync(br => br.RecordId == id);

            if (record == null) return NotFound();

            return View(record);
        }

        /// <summary>
        /// Handles deletion of a borrowing record
        /// </summary>
        /// <param name="id">ID of the record</param>
        /// <returns>Redirect to index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.Borrowing_Records.FindAsync(id);

            if (record != null)
            {
                if (!record.Returned)
                {
                    var book = await _context.Books.FindAsync(record.BookId);
                    if (book != null) book.Available = true;
                }

                _context.Borrowing_Records.Remove(record);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a borrowing record exists
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns>True if exists, false otherwise</returns>
        private bool BorrowingRecordExists(int id)
        {
            return _context.Borrowing_Records.Any(e => e.RecordId == id);
        }
    }
}
