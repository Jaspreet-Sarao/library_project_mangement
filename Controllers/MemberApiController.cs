
using LibraryManagementProject.Models;
using LibraryManagementProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagementProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MembersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MembersApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers()
        {
            return await _context.Members
                .Select(m => new MemberDto
                {
                    MemberId = m.MemberId,
                    FullName = m.FullName,
                    Email = m.Email,
                    Phone = m.Phone
                })
                .ToListAsync();
        }

        // GET: api/MembersApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // GET: api/MembersApi/5/borrowings
        [HttpGet("{id}/borrowings")]
        public async Task<ActionResult<IEnumerable<BorrowingRecordDto>>> GetMemberBorrowings(int id)
        {
            return await _context.Borrowing_Records
                .Where(br => br.MemberId == id)
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

        // POST: api/MembersApi
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMember", new { id = member.MemberId }, member);
        }

        // PUT: api/MembersApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.MemberId)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // DELETE: api/MembersApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }
    }
}
