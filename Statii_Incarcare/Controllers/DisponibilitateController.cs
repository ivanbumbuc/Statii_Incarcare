using Microsoft.AspNetCore.Mvc;
using Statii_Incarcare.Models;
using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Controllers
{
    public class DisponibilitateController : Controller
    {
        private readonly StatiiIncarcareContext _context;

        public DisponibilitateController(StatiiIncarcareContext context)
        {
            _context = context;
        }
        public IActionResult Index(int ? id)
        {
            //DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 14);

             
            ViewData["prizaId"] = id;
            NextBackCalendar.zile = 0;
            return View(AfisareRezervari(id,DateTime.Now));
        }
        public IActionResult BackWeek(int? id)
        {
            int zile = NextBackCalendar.Back();
            ViewData["prizaId"] = id;
            DateTime x=DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + (zile*7));
            //Console.WriteLine("-zile= " + zile+" -------- "+x.ToString());
            return View("Index", AfisareRezervari(id, x));
        }
        public IActionResult NextWeek(int? id)
        {
            int zile = NextBackCalendar.Next();
            ViewData["prizaId"] = id;
            Console.WriteLine("+zile= " + zile);
            DateTime x=DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + (zile * 7));
            //Console.WriteLine("-zile= " + zile + " -------- " + x.ToString());
            return View("Index", AfisareRezervari(id, x));
        }

        private RezervariSaptamana AfisareRezervari(int ? id,DateTime l)
        {
            RezervariSaptamana x = new RezervariSaptamana();
            x.dateZile = new List<string>();
            var luni = StartOfWeek(l, DayOfWeek.Sunday);
            for(int i=1;i<=7;i++)
            {
                DateTime f=luni.AddDays(-(int)DateTime.Now.DayOfWeek +i);
                x.dateZile.Add(f.ToString("dd/MM/yyyy"));
            }
            foreach (var d in _context.Rezervaris)
            {
                if (d.PrizaId == id)
                {
                    DateTime g = (DateTime)d.StartTime;
                    DateTime q = (DateTime)d.EndTime;
                    DisponibilitateSaptamana disSap = new DisponibilitateSaptamana();
                    disSap.oraInceput = g.ToString("H:mm");
                    disSap.oraSfarsit = q.ToString("H:mm");
                    if (DatesAreInTheSameWeek(luni, g))
                    {
                        disSap.rezerrvare = d;
                        switch ((int)g.DayOfWeek)
                        {
                            case 1:
                                x.luni.Add(disSap);
                                break;
                            case 2:
                                x.marti.Add(disSap);
                                break;
                            case 3:
                                x.miercuri.Add(disSap);
                                break;
                            case 4:
                                x.joi.Add(disSap);
                                break;
                            case 5:
                                x.vineri.Add(disSap);
                                break;
                            case 6:
                                x.sambata.Add(disSap);
                                break;
                            case 0:
                                x.duminica.Add(disSap);
                                break;

                        }
                    }

                }
            }
            return x;
        }

        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
        public static bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
        {
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

            return d1 == d2;
        }

    }
}
