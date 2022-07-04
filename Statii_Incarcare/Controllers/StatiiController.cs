using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Controllers
{
    public class StatiiController : Controller
    {
        private readonly StatiiIncarcareContext _context;

        public StatiiController(StatiiIncarcareContext context)
        {
            _context = context;
        }
        private static int? idStatie;
        private bool Verificare()
        {
            var userId = HttpContext.Request.Cookies["user_id"];
            if (userId == null)
            {
                return true;
            }
            return false;
        }

        private List<InformatiePriza> Prize(int? id)
        {
            /*var statii = _context.Statiis.Join(_context.Prizes, a => a.StatieId,
                b => b.StatieId, (a, b) => new { Prizaid = b.PrizaId }).Select(x => x.Prizaid);
            */
          /*  var d1 = _context.Statiis
                .Include(x=> x.Prizes)
                .ThenInclude(x=> x.Tip)
                .FirstOrDefault(x => x.StatieId == id);*/

            var d = (from s in _context.Statiis
                     join p in _context.Prizes
                     on s.StatieId equals p.StatieId
                     join t in _context.Tips on p.TipId equals t.TipId
                     where (s.StatieId == id)
                     select new InformatiePriza
                     {
                         NumarPriza = p.PrizaId,
                         Tip = t.Nume
                     }
                     ).ToList();
            return d;
        }

        // GET: Statii
        public async Task<IActionResult> Index()
        {
                Console.WriteLine("null");
            if (Verificare())
                return NotFound();
            return _context.Statiis != null ?
                        View(await _context.Statiis.ToListAsync()) :
                        Problem("Entity set 'StatiiIncarcareContext.Statiis'  is null.");
        }

        // GET: Statii/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (Verificare())
                return NotFound();

            if (id == null || _context.Statiis == null)
            {
                return NotFound();
            }

            var statii = await _context.Statiis
                .FirstOrDefaultAsync(m => m.StatieId == id);
            if (statii == null)
            {
                return NotFound();
            }
            var x = new Detalii { statii = statii, InformatiiPrize = Prize(id) };
            
            return View(x);
        }
        public IActionResult AddPriza(int? id)
        {
            return RedirectToAction("Index", "AdaugarePrize", new { id = id.Value });
        }

        // GET: Statii/Create
        public IActionResult Create()
        {
            if (Verificare())
                return NotFound();
            return View();
        }

        // POST: Statii/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatieId,Nume,Oras,Adresa")] Statii statii)
        {
            if (Verificare())
                return NotFound();
            if (ModelState.IsValid)
            {
                _context.Add(statii);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(statii);
        }

        // GET: Statii/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (Verificare())
                return NotFound();
            if (id == null || _context.Statiis == null)
            {
                return NotFound();
            }

            var statii = await _context.Statiis.FindAsync(id);
            if (statii == null)
            {
                return NotFound();
            }
            return View(statii);
        }

        // POST: Statii/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StatieId,Nume,Oras,Adresa")] Statii statii)
        {
            if (Verificare())
                return NotFound();
            if (id != statii.StatieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statii);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatiiExists(statii.StatieId))
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
            return View(statii);
        }

        // GET: Statii/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (Verificare())
                return NotFound();
            if (id == null || _context.Statiis == null)
            {
                return NotFound();
            }

            var statii = await _context.Statiis
                .FirstOrDefaultAsync(m => m.StatieId == id);
            if (statii == null)
            {
                return NotFound();
            }

            return View(statii);
        }

        // POST: Statii/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (Verificare())
                return NotFound();
            if (_context.Statiis == null)
            {
                return Problem("Entity set 'StatiiIncarcareContext.Statiis'  is null.");
            }
            var statii = await _context.Statiis.FindAsync(id);
            if (statii != null)
            {
                _context.Statiis.Remove(statii);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatiiExists(int id)
        {
            return (_context.Statiis?.Any(e => e.StatieId == id)).GetValueOrDefault();
        }
    }
}
