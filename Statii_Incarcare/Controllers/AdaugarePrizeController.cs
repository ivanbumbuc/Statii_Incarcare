using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Statii_Incarcare.Models;
using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Controllers
{
    public class AdaugarePrizeController : Controller
    {
        private readonly StatiiIncarcareContext _context;

        public AdaugarePrizeController(StatiiIncarcareContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? id)
        {
            ModelPriza x = new ModelPriza();
            x.StatieId = id.Value;
            List<SelectListItem> c = new List<SelectListItem>();
            foreach(var d in _context.Tips)
                c.Add(new SelectListItem { Text = d.Nume, Value = d.TipId.ToString() });
            x.tipuri = c;
            return View(x);
        }
        [HttpPost]
        public async Task<IActionResult> Add(ModelPriza model)
        {
            Prize x = new Prize();
            x.StatieId=model.StatieId;
            x.TipId=model.TipId;
            _context.Add(x);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Statii", new { id = model.StatieId });
        }
    }
}
