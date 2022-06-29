using Microsoft.AspNetCore.Mvc;
using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Controllers
{
    public class LoginController : Controller
    {
        private readonly StatiiIncarcareContext _context;

        public LoginController(StatiiIncarcareContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
      

        [HttpPost]
        public IActionResult Login(Utilizatori utilizatori)
        {
            var user = _context.Utilizatoris.FirstOrDefault(x => x.Email == utilizatori.Email && x.Parola == utilizatori.Parola);


            if (user!=null)
            {
                Console.WriteLine("Exista contul!!");
            }    
            else
            {
                Console.WriteLine("Nu exista conutul!");
            }
            return null;
        }
    }
}
