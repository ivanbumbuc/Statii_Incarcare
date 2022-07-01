using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Statii_Incarcare.Models.Db;
using System.Text.RegularExpressions;

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
           
            var userId = HttpContext.Request.Cookies["user_id"];
            if (userId != null)
            {
                var user = _context.Utilizatoris.FirstOrDefault(x => x.UtilizatorId.ToString() == userId);
                return View("Logat", user);
            }
            else
                return View();
        }

        [HttpPost]
        public IActionResult Login(Utilizatori utilizatori)
        {
            var userId = HttpContext.Request.Cookies["user_id"];
            if (userId != null)
            {
                var user = _context.Utilizatoris.FirstOrDefault(x => x.UtilizatorId.ToString() == userId);
                return View("Logat", user);
            }
            else
            {
                var user = _context.Utilizatoris.FirstOrDefault(x => x.Email == utilizatori.Email && x.Parola == utilizatori.Parola);


                if (user != null)
                {
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddDays(1));
                    HttpContext.Response.Cookies.Append("user_id", user.UtilizatorId.ToString(), cookieOptions);
                    return View("Logat", user);
                }
                else
                {
                    ViewData["Verificare"] = "Parola incorecta!";
                }
            }
            return View("Index");
        }
        
        public IActionResult Deconectare()
        {
            var userId = HttpContext.Request.Cookies["user_id"];
            if (userId != null)
            {
                HttpContext.Response.Cookies.Delete("user_id");
            }
            return View("~/Views/Login/Index.cshtml");
        }

        public IActionResult CreareCont()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreareCont2( Utilizatori utilizator)
        {
            if(utilizator==null || utilizator.Nume==null || utilizator.Email==null || utilizator.Parola==null)
            {
                ViewData["avertisment"] = "Introduceti toate datele!";
                return View("CreareCont");
            }
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            RegexOptions.CultureInvariant | RegexOptions.Singleline);
            bool isValidEmail = regex.IsMatch(utilizator.Email.ToString());
            var userEmail = _context.Utilizatoris.FirstOrDefault(x => x.Email == utilizator.Email);
            if(userEmail != null)
            {
                ViewData["avertisment"] = "Acest email este folosit la un cont existent, introduceti un email valid!";
                return View("CreareCont");
            }

            if (!isValidEmail)
            {
                ViewData["avertisment"] = "Email invalid!";
                return View("CreareCont");
            }

            if (ModelState.IsValid)
            {
                utilizator.UtilizatorId = Guid.NewGuid();
                _context.Add(utilizator);
                await _context.SaveChangesAsync();
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddDays(1));
                HttpContext.Response.Cookies.Append("user_id", utilizator.UtilizatorId.ToString(), cookieOptions);
            }
            return View("Logat", utilizator);
        }

    }

}
