using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Statii_Incarcare.Models;
using Statii_Incarcare.Models.Db;
using System.Diagnostics;

namespace Statii_Incarcare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly StatiiIncarcareContext _context;

        

        public HomeController(ILogger<HomeController> logger, StatiiIncarcareContext context)
        {
            _logger = logger;
            _context = context;
        }
        private List<SelectListItem> GetOrase()
        {
            var list = new List<SelectListItem>();
            
            var orase = (from p in _context.Statiis select new { p.Oras }).Distinct();
            foreach (var d in orase)
            {
                list.Add(new SelectListItem { Text = d.Oras, Value = d.Oras.ToString() });
            }
            return list;
        }
        public IActionResult Index()
        {

            ViewData["layout"] = "2";
            if (HttpContext.Request.Cookies.ContainsKey("user_id"))
            {
                var userId = HttpContext.Request.Cookies["user_id"];

                if (userId != null)
                    ViewData["layout"]="1";
            }
            List<SelectListItem> items = GetOrase();
            ViewBag.O = items;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public JsonResult GetStatii(String oras)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var statii = new Dictionary<int, string>();
            foreach (var f in _context.Statiis)
            { if(f.Oras==oras)
                if(!statii.ContainsKey(f.StatieId))
                {
                    statii[f.StatieId] = f.Nume;
                }
            }
            for (int i = 0; i < statii.Count; i++)
            {
                List<int?> pr = new List<int?>();
                foreach(var g in _context.Prizes)
                {
                    if(g.StatieId== statii.ElementAt(i).Key)
                    {
                        pr.Add(g.PrizaId);
                    }
                }
                var oraMin = new Dictionary<int, int>();
                for (int j = 0; j <= 24; j++)
                {
                    oraMin[j] = 0;
                }
                foreach (var s in _context.Rezervaris)
                {

                    if(pr.Contains(s.PrizaId))
                    {
                        var u= Int32.Parse(s.StartTime.ToString().Split(' ')[1].Split(':')[0]);
                        oraMin[u] = oraMin[u] + 1;

                    }
                }
                var min1 = oraMin.ElementAt(0).Key; var min2 = oraMin.ElementAt(0).Value;
                var max1 = oraMin.ElementAt(0).Key;
                var max2 = oraMin.ElementAt(0).Value;
                for (int p=0;p<oraMin.Count;p++)
                {
                    if(oraMin.ElementAt(p).Value<=min2)
                    {
                        min2 = oraMin.ElementAt(p).Value;
                        min1 = oraMin.ElementAt(p).Key;
                    }
                    if(oraMin.ElementAt(p).Value>=max2)
                    {
                        max2 = oraMin.ElementAt(p).Value;
                        max1 = oraMin.ElementAt(p).Key;
                    }
                }
                list.Add(new SelectListItem { Text =statii.ElementAt(i).Value, Value = min1+","+max1 });
            }
            return Json(list);
        }

        public JsonResult GetS(String oras)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati Statia", Value = "-" });
            foreach (var d in _context.Statiis)
            {
                if (d.Oras == oras)
                    list.Add(new SelectListItem { Text = d.Nume, Value = d.StatieId.ToString() });
            }
            return Json(list);
        }
        public JsonResult GetGrafic2(String id)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Selectati Statia", Value = "-" });
            var prize = new Dictionary<int, int>();
            foreach (var d in _context.Prizes)
            {
                if (d.StatieId == Int32.Parse(id))
                {
                    if(!prize.ContainsKey(d.PrizaId))
                    {
                        prize[d.PrizaId] = 0;
                    }
                    //list.Add(new SelectListItem { Text = d.Nume, Value = d.StatieId.ToString() });
                }
            }
            var zile = new Dictionary<int, int>();
            for (int i = 0; i < 7; i++)
                zile[i] = 0;
            var dataDuminica = StartOfWeek(DateTime.Now, DayOfWeek.Sunday);
            foreach(var s in _context.Rezervaris)
            {
                if (prize.ContainsKey(s.PrizaId.Value)) {
                    var x = (DateTime)s.StartTime;
                    if (DatesAreInTheSameWeek(dataDuminica, x))
                    {
                        var h = (int)x.DayOfWeek;
                        if (!zile.ContainsKey(h))
                        {
                            zile[h] = 0;
                        }
                        else
                            zile[h] = zile[h] + 1;
                    }
                }
            }
            for(int i=0;i<7;i++)
            {
               list.Add(new SelectListItem { Text = zile.ElementAt(i).Key.ToString(), Value = zile.ElementAt(i).Value.ToString() });
            }
            return Json(list);
        }
        public JsonResult IsAdmin()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var userId = HttpContext.Request.Cookies["user_id"];
            foreach(var d in _context.Administratorii)
            {
                if(d.UtilizatorId.ToString()==userId)
                {
                    list.Add(new SelectListItem { Text = "Admin", Value = "true" });
                }
                else
                {
                    list.Add(new SelectListItem { Text = "Admin", Value = "false" });
                }
            }
            return Json(list);
        }

        public static bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
        {
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

            return d1 == d2;
        }

        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

    }
}