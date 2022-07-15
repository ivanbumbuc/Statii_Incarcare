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

        [HttpGet]
        public  async Task<IActionResult> Cautare(string s)
        {
            ViewData["Search"] = s;
            if (!String.IsNullOrEmpty(s))
            {
                var d = _context.Statiis.Where(x => x.Nume.Contains(s) || x.Oras.Contains(s) || x.Adresa.Contains(s));
                return View("Index",await d.ToListAsync());
            }
            return View("Index",await _context.Statiis.ToListAsync());
        }
        private bool DateComp(string x, string z)
        {
            var y = x.Split('.');
            var w = z.Split('/');
            if (Int32.Parse(y[2]) == Int32.Parse(w[2]) && Int32.Parse(y[1]) == Int32.Parse(w[0]) && Int32.Parse(y[0]) == Int32.Parse(w[1]))
                return true;
            return false;
        }
        public JsonResult GetIntervale(string data, string id)
        {

            List<SelectListItem> list = new List<SelectListItem>();
            List<string> prize = new List<string>();
            var intervaleTotale = new Dictionary<string, string>();
            foreach (var f in _context.Prizes)
            {
                if (id == f.StatieId.ToString())
                    prize.Add(f.PrizaId.ToString());
            }
            List<string> ore = new List<string>();
            List<string> intervale = new List<string>();
            foreach (var g in prize)
            {
                foreach(var f in _context.Rezervaris)
                {
                    var x = f.StartTime.ToString().Split(' ');
                    if (DateComp(x[0], data) && f.PrizaId==Int32.Parse(g))
                    {
                        var a = f.StartTime.ToString().Split(' ');
                        var z = f.EndTime.ToString().Split(' ');
                        var dif1 = x[1].Split(':');
                        var dif2 = z[1].Split(':');
                        ore.Add(dif1[0] + "-" + dif2[0]);
                    }
                }
                for(int i=0;i<ore.Count-1;i++)
                {
                    for(int j=i+1;j<ore.Count;j++)
                    {
                        var r = ore[i].Split('-');
                        var r2 = ore[j].Split('-');
                        if (Int32.Parse(r[0]) > Int32.Parse(r2[0]))
                        {
                            var q = ore[i];
                            ore[i] = ore[j];
                            ore[j] = q;
                        }
                    }
                }
                
                    int n = 0;
                string last = "";
                    for (int h = 0; h < ore.Count; h++)
                    {
                    if(intervale.Count>0)
                        last = intervale[intervale.Count - 1];
                        var k = ore[h].Split('-');
                        if (n != Int32.Parse(k[0]) && (n + "-" + k[0]) != last)
                        {
                            intervale.Add(n + "-" + k[0]);
                        }
                        n = Int32.Parse(k[1]);
                    }
                if (n != 24)
                    intervale.Add(n + "-" + 24);
                foreach (var h in intervale)
                {
                    if (intervaleTotale.ContainsKey(h))
                    {
                        intervaleTotale[h] = intervaleTotale[h] + ", " + g;
                    }
                    else
                        intervaleTotale.Add(h, g);
                }
                intervale.Clear();
                if(ore.Count>0)
                    ore.Clear();

            }
            for(int i=0;i<intervaleTotale.Count;i++)
            {
                //Console.WriteLine(intervaleTotale.ElementAt(i).Key + "-------" + intervaleTotale.ElementAt(i).Value);
                list.Add(new SelectListItem { Text = intervaleTotale.ElementAt(i).Key, Value =intervaleTotale.ElementAt(i).Value });
            }
            
            return Json(list);
        } 


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
                /*List<int> prize = new List<int>();
                foreach (var f in _context.Prizes)
                {
                    if(f.StatieId==id)
                    prize.Add(f.PrizaId);
                }
                foreach(var h in _context.Rezervaris)
                {
                    if(prize.Contains(h.PrizaId.Value))
                    {
                        Console.WriteLine(h.PrizaId);
                        //_context.Remove(h);
                    }
                }
                foreach(var g in _context.Prizes)
                {
                    if (prize.Contains(g.PrizaId))
                        Console.WriteLine(g.PrizaId);
                        //_context.Remove(g);
                }*/
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
