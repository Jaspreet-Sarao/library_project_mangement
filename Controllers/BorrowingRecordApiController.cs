using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementProject.Data;
using LibraryManagementProject.Models;

namespace LibraryManagementProject.Controllers

    {
        [Route("api/[controller]")]
        [ApiController]
        public class BorrowingRecordsApiController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public BorrowingRecordsApiController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: api/BorrowingRecordsApi
            [HttpGet]
            public async Task<ActionResult<IEnumerable<BorrowingRecordDto>>> GetBorrowing_Records()
            {
                return await _context.Borrowing_Records
                    .Include(br => br.Book)
                    .Include(br => br.Member)
                    .Select(br => new BorrowingRecordDto
                    {
                        RecordId = br.RecordId,
                        BookTitle = br.Book.Title,
                        MemberName = br.Member.FullName,
                        DueDate = br.DueDate,
                        Status = br.Returned ? "Returned" : "Borrowed",
                        LateFee = br.LateFee
                    })
                    .ToListAsync();
            }

            // GET: api/BorrowingRecordsApi/5
            [HttpGet("{id}")]
            public async Task<ActionResult<Borrowing_Record>> GetBorrowing_Record(int id)
            {
                var borrowing_Record = await _context.Borrowing_Records
                    .Include(br => br.Book)
                    .Include(br => br.Member)
                    .FirstOrDefaultAsync(br => br.RecordId == id);

                if (borrowing_Record == null)
                {
                    return NotFound();
                }

                return borrowing_Record;
            }

            // GET: api/BorrowingRecordsApi/overdue
            [HttpGet("overdue")]
            public async Task<ActionResult<IEnumerable<BorrowingRecordDto>>> GetOverdueRecords()
            {
                return await _context.Borrowing_Records
                    .Where(br => !br.Returned && br.DueDate < DateTime.Now)
                    .Include(br => br.Book)
                    .Include(br => br.Member)
                    .Select(br => new BorrowingRecordDto
                    {
                        RecordId = br.RecordId,
                        BookTitle = br.Book.Title,
                        MemberName = br.Member.FullName,
                        DueDate = br.DueDate,
                        Status = "Overdue",
                        LateFee = CalculateLateFee(br.DueDate)
                    })
                    .ToListAsync();
            }

            // POST: api/BorrowingRecordsApi
            [HttpPost]
            public async Task<ActionResult<Borrowing_Record>> PostBorrowing_Record(Borrowing_Record borrowing_Record)
            {
                // Set default values
                borrowing_Record.BorrowDate = DateTime.Now;
                borrowing_Record.DueDate = DateTime.Now.AddDays(15);
                borrowing_Record.Returned = false;
                borrowing_Record.LateFee = 0;

                // Mark book as unavailable
                var book = await _context.Books.FindAsync(borrowing_Record.BookId);
                if (book != null)
                {
                    book.Available = false;
                }

                _context.Borrowing_Records.Add(borrowing_Record);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBorrowing_Record", new { id = borrowing_Record.RecordId }, borrowing_Record);
            }

            // PUT: api/BorrowingRecordsApi/5/return
            [HttpPut("{id}/return")]
            public async Task<IActionResult> ReturnBook(int id)
            {
                var borrowingRecord = await _context.Borrowing_Records.FindAsync(id);
                if (borrowingRecord == null)
                {
                    return NotFound();
                }

                // Update record
                borrowingRecord.Returned = true;
                borrowingRecord.LateFee = CalculateLateFee(borrowingRecord.DueDate);

                // Mark book as available
                var book = await _context.Books.FindAsync(borrowingRecord.BookId);
                if (book != null)
                {
                    book.Available = true;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }

            // DELETE: api/BorrowingRecordsApi/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteBorrowing_Record(int id)
            {
                var borrowing_Record = await _context.Borrowing_Records.FindAsync(id);
                if (borrowing_Record == null)
                {
                    return NotFound();
                }

                // If book wasn't returned, mark it as available
                if (!borrowing_Record.Returned)
                {
                    var book = await _context.Books.FindAsync(borrowing_Record.BookId);
                    if (book != null)
                    {
                        book.Available = true;
                    }
                }

                _context.Borrowing_Records.Remove(borrowing_Record);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool Borrowing_RecordExists(int id)
            {
                return _context.Borrowing_Records.Any(e => e.RecordId == id);
            }

            private decimal CalculateLateFee(DateTime dueDate)
            {
                if (DateTime.Now <= dueDate) return 0;

                var daysLate = (DateTime.Now - dueDate).Days;
                return daysLate * 0.50m; 
            }
        }
    }