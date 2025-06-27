
using LibraryManagementProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using LibraryManagementProject.Models;

namespace LibraryManagementProject.Controllers
{
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MemberController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = await _context.Members
            .Include(m => m.Borrowing_Records)
            .ThenInclude(br => br.Book)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);

        }

        // Creation of  members through GET
        public IActionResult Create()
        {
            return View();
        }
        // Using Post to create Members
        [HttpPost]

        public async Task<IActionResult> Create([Bind("MemberId, FirstName, LastName, Email, Phone")] Member member)
        {
            member.Borrowing_Records = new List<Borrowing_Record>();
            {


                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            return View(member);
        }


        // Editing members using GET 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }
        // Editing members by POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId, FirstName, LastName, Email, Phone")] Member member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        // GET: Member/Borrowings/5
        [HttpGet("Borrowings/{id}")]
        public async Task<IActionResult> Borrowings(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Borrowing_Records)
                    .ThenInclude(br => br.Book)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        //  Get Deleting Member
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            return View("Delete1", member);
        }
        // Post: Deleting Members

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }
    }
}