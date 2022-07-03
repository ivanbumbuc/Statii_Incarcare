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
            var orase = (from p in _context.Statiis select new { p.Oras }).Distinct();
            foreach(var d in orase)
            {
                list.Add(new SelectListItem { Text = d.Oras, Value = d.Oras.ToString()});
            }
            return list;
        }
        public void GetStatii(string s)
        {
            var list = new List<SelectListItem>();
            foreach (var d in _context.Statiis)
            {
                if(d.Oras==s)
                list.Add(new SelectListItem { Text = d.Nume, Value = d.StatieId.ToString() });
            }
            ViewBag.Statii = list;
        }
        public IActionResult Index()
        {
            List<SelectListItem> items = GetOrase();
            ViewBag.Orase = items;

            return View();
        }
    }
}
