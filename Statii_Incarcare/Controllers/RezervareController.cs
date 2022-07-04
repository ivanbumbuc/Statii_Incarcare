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
            foreach(var d in orase)
            {
                list.Add(new SelectListItem { Text = d.Oras, Value = d.Oras.ToString()});
            }
            return list;
        }

        public JsonResult GetAdrese(String oras)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati Adresa", Value ="-" });
            foreach (var d in _context.Statiis)
            {
                if (d.Oras == oras)
                    list.Add(new SelectListItem { Text = d.Adresa, Value = d.Adresa });
            }
            return Json(list);
        }

        public JsonResult GetStatii(String oras,String adresa)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati Statia", Value = "-" });
            foreach (var d in _context.Statiis)
            {
                if( d.Oras==oras && d.Adresa==adresa)
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
                    list.Add(new SelectListItem { Text ="Priza "+q.NumarPriza+" de tip "+q.Tip, Value = q.NumarPriza.ToString() });
            }
            return Json(list);
        }
        public JsonResult GetOre(String id)
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
                if (DateComp(x[0],id))
                {
                    var dif1 = x[1].Split(':');
                    var dif2 = z[1].Split(':');
                    for(int i = Int32.Parse(dif1[0]);i<= Int32.Parse(dif2[0]);i++)
                    {
                        st.Remove(i.ToString());
                    }
                }
            }
            foreach (var q in st)
            {
                if(q=="0")
                list.Add(new SelectListItem { Text = "00", Value = q });
                else
                    list.Add(new SelectListItem { Text = q, Value = q });

            }
            return Json(list);
        }
        public JsonResult GetOreSfarsit()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati ora de sfarsit", Value = "-" });
            for(int q=0;q<24;q++)
            {
                if (q == 0)
                    list.Add(new SelectListItem { Text = "00", Value = q.ToString() });
                else
                    list.Add(new SelectListItem { Text = q.ToString(), Value = q.ToString() });
            }
            return Json(list);
        }
        public JsonResult CreareRezervare(string oras,string adresa,string priza,string data,string oi,string of)
        {
            Console.WriteLine(oras + " " + adresa + " " + priza + " " + data + " " + oi + ":00" +" "+ of + ":00");
            return Json(null);
        }

        private bool DateComp(string x,string z)
        {
            var y = x.Split('.');
            var w = x.Split('/');
            if (y[2] == w[0] && y[1] == w[1] && y[2] == w[0])
                return true;
            return false;
        }
        public IActionResult Index()
        {
            List<SelectListItem> items = GetOrase();
            ViewBag.Orase = items;
            return View();
        }
    }
}
