using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Controllers
{
    public class RezervareController : Controller
    {
        private readonly StatiiIncarcareContext _context;

        public RezervareController(StatiiIncarcareContext context)
        {
            _context = context;
        }

        private List<SelectListItem> GetOrase()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati Orasul", Value = "-" });
            var orase = (from p in _context.Statiis select new { p.Oras }).Distinct();
            foreach (var d in orase)
            {
                list.Add(new SelectListItem { Text = d.Oras, Value = d.Oras.ToString() });
            }
            return list;
        }

        public JsonResult GetAdrese(String oras)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati Adresa", Value = "-" });
            foreach (var d in _context.Statiis)
            {
                if (d.Oras == oras)
                    list.Add(new SelectListItem { Text = d.Adresa, Value = d.Adresa });
            }
            return Json(list);
        }

        public JsonResult GetStatii(String oras, String adresa)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati Statia", Value = "-" });
            foreach (var d in _context.Statiis)
            {
                if (d.Oras == oras && d.Adresa == adresa)
                    list.Add(new SelectListItem { Text = d.Nume, Value = d.StatieId.ToString() });
            }
            return Json(list);
        }

        public JsonResult GetPrize(int id)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati Priza", Value = "-" });
            List<InformatiePriza> d = (from s in _context.Statiis
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
            foreach (var q in d)
            {
                list.Add(new SelectListItem { Text = "Priza " + q.NumarPriza + " de tip " + q.Tip, Value = q.NumarPriza.ToString() });
            }
            return Json(list);
        }
        public JsonResult GetOre(String id, string idPriza)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati ora inceput", Value = "-" });
            List<string> st = new List<string>();
            for (int i = 1; i < 24; i++)
                st.Add(i.ToString());
            foreach (var d in _context.Rezervaris)
            {
                var x = d.StartTime.ToString().Split(' ');
                var z = d.EndTime.ToString().Split(' ');
                if (DateComp(x[0], id) && d.PrizaId == Int32.Parse(idPriza))
                {
                    var dif1 = x[1].Split(':');
                    var dif2 = z[1].Split(':');
                    for (int i = Int32.Parse(dif1[0]); i <= Int32.Parse(dif2[0]); i++)
                    {
                        st.Remove(i.ToString());
                    }
                }
            }
            var time = DateTime.Now.ToString("t").Split(':');
            var dataCurenta = DateTime.Now.ToString().Split(" ");
            if (DateComp(dataCurenta[0], id))
            {
                for (int i = 0; i < Int32.Parse(time[0]); i++)
                {
                    if (st.Contains(i.ToString()))
                    {
                        // Console.WriteLine(i);
                        st.Remove(i.ToString());
                    }
                }
            }
            foreach (var q in st)
            {
                if (q == "0")
                    list.Add(new SelectListItem { Text = "00", Value = q });
                else
                    list.Add(new SelectListItem { Text = q, Value = q });

            }
            return Json(list);
        }

        public JsonResult GetOreSfarsit(string id, string oraInceput)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati ora de sfarsit", Value = "-" });
            int mini = 24;
            int oraI = Int32.Parse(oraInceput);
            foreach (var d in _context.Rezervaris)
            {
                var x = d.StartTime.ToString().Split(' ');
                if (DateComp(x[0], id))
                {
                    var dif1 = x[1].Split(':');
                    if (Int32.Parse(dif1[0]) < mini && Int32.Parse(dif1[0]) >= oraI)
                    {
                        mini = Int32.Parse(dif1[0]);
                    }
                }
            }
            for (int q = oraI + 1; q <= mini; q++)
            {
                if (q == 0)
                    list.Add(new SelectListItem { Text = "00", Value = q.ToString() });
                else
                    list.Add(new SelectListItem { Text = q.ToString(), Value = q.ToString() });
            }
            return Json(list);
        }
        public JsonResult CreareRezervare(string priza, string data, string oi, string of, string masina)
        {
            // Console.WriteLine(oras + " " + adresa + " " + priza + " " + data + " " + oi + ":00" +" "+ of + ":00");
            var userId = HttpContext.Request.Cookies["user_id"];
            Rezervari x = new Rezervari();
            x.BookingId = new Guid();
            x.UtilizatorId = new Guid(HttpContext.Request.Cookies["user_id"]);
            x.NrMasina = masina;
            var d = data.Split('-');
            x.StartTime = new DateTime(Int32.Parse(d[0]), Int32.Parse(d[1]), Int32.Parse(d[2]), Int32.Parse(oi), 0, 0);
            x.EndTime = new DateTime(Int32.Parse(d[0]), Int32.Parse(d[1]), Int32.Parse(d[2]), Int32.Parse(of), 0, 0);
            x.PrizaId = Int32.Parse(priza);
            _context.Add(x);
            _context.SaveChanges();
            return Json(null);
        }

        private bool DateComp(string x, string z)
        {
            var y = x.Split('.');
            var w = z.Split('-');
            if (y[2] == w[0] && y[1] == w[1] && y[0] == w[2])
                return true;
            return false;
        }
        private bool DateComp2(string x, string z)
        {
            var y = x.Split('.');
            var w = z.Split('-');
            if (y[1] == w[2] && y[0] == w[1] && y[2] == w[0])
                return true;
            return false;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.Request.Cookies["user_id"];
            if (userId == null)
                return NotFound();
            List<SelectListItem> items = GetOrase();
            ViewBag.Orase = items;
            return View();
        }
    }
}