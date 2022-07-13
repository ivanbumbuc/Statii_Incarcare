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
                    if(oraMin.ElementAt(p).Value<min2)
                    {
                        min2 = oraMin.ElementAt(p).Value;
                        min1 = oraMin.ElementAt(p).Key;
                    }
                    if(oraMin.ElementAt(p).Value>max2)
                    {
                        max2 = oraMin.ElementAt(p).Value;
                        max1 = oraMin.ElementAt(p).Key;
                    }
                }
                list.Add(new SelectListItem { Text =statii.ElementAt(i).Value, Value = min1+","+max1 });
            }
            return Json(list);
        }
    }
}