using Microsoft.AspNetCore.Mvc;
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
            x.tipuri = (IList<Tip>)_context.Prizes.ToList();
            return View(x);
        }
    }
}
