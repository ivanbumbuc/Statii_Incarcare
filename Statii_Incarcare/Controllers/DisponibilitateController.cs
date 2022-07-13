using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index(int? id)
        {
            //DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 14);


            ViewData["prizaId"] = id;
            NextBackCalendar.zile = 0;
            return View(AfisareRezervari(id, DateTime.Now));
        }
        public IActionResult BackWeek(int? id)
        {
            int zile = NextBackCalendar.Back();
            ViewData["prizaId"] = id;
            DateTime x = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + (zile * 7));
            //Console.WriteLine("-zile= " + zile+" -------- "+x.ToString());
            return View("Index", AfisareRezervari(id, x));
        }
        public IActionResult NextWeek(int? id)
        {
            int zile = NextBackCalendar.Next();
            ViewData["prizaId"] = id;
            Console.WriteLine("+zile= " + zile);
            DateTime x = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + (zile * 7));
            //Console.WriteLine("-zile= " + zile + " -------- " + x.ToString());
            return View("Index", AfisareRezervari(id, x));
        }

        private RezervariSaptamana AfisareRezervari(int? id, DateTime l)
        {
            RezervariSaptamana x = new RezervariSaptamana();
            x.dateZile = new List<string>();
            var luni = StartOfWeek(l, DayOfWeek.Sunday);
            for (int i = 0; i < 7; i++)
            {
                DateTime f = luni.AddDays(-(int)luni.DayOfWeek + i);
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

        public JsonResult GetOre(String id, string idPriza, String oraIn, String oraFin)
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
                    if (Int32.Parse(dif1[0]) != Int32.Parse(oraIn.Split(':')[0]) && Int32.Parse(dif2[0]) != Int32.Parse(oraFin.Split(':')[0]))
                        for (int i = Int32.Parse(dif1[0]); i <= Int32.Parse(dif2[0]); i++)
                        {
                            st.Remove(i.ToString());
                        }
                }
            }
            var time = DateTime.Now.ToString("t").Split(':');
            var dataCurenta = DateTime.Now.ToString("dd.MM.yyyy");
            if (DateComp(dataCurenta, id))
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

        private bool DateComp(string x, string z)
        {
            var y = x.Split('.');
            var w = z.Split('.');
            if (y[0] == w[0] && y[1] == w[1] && y[2] == w[2])
                return true;
            return false;
        }

        public  JsonResult Edit(String data, string idPriza, String oraIn, String oraFin,string nrMasina,String liS,String liF)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Rezervari x = new Rezervari();
            foreach (var d in _context.Rezervaris)
            {
                var dd = d.StartTime.ToString().Split(' ');
                var dd2 = d.EndTime.ToString().Split(' ');
                if(d.PrizaId==Int32.Parse(idPriza))
                    if (DateComp(dd[0],data))
                        if (Int32.Parse(oraIn.Split(':')[0])== Int32.Parse(dd[1].Split(':')[0]) && Int32.Parse(oraFin.Split(':')[0]) == Int32.Parse(dd2[1].Split(':')[0]))
                        {
                   
                           // x.PrizaId = Int32.Parse(idPriza);
                           // x.BookingId = d.BookingId;
                           // x.NrMasina = nrMasina;
                           // x.UtilizatorId = d.UtilizatorId;
                            var f = dd[0].Split(".");
                           // x.StartTime = new DateTime(Int32.Parse(f[2]), Int32.Parse(f[1]), Int32.Parse(f[0]), Int32.Parse(liS), 0, 0);
                           // x.EndTime = new DateTime(Int32.Parse(f[2]), Int32.Parse(f[1]), Int32.Parse(f[0]), Int32.Parse(liF), 0, 0);
                           // x.Priza = d.Priza;
                           // x.Utilizator = d.Utilizator;
                             d.NrMasina = nrMasina; 
                            d.StartTime = new DateTime(Int32.Parse(f[2]), Int32.Parse(f[1]), Int32.Parse(f[0]), Int32.Parse(liS), 0, 0);
                            d.EndTime = new DateTime(Int32.Parse(f[2]), Int32.Parse(f[1]), Int32.Parse(f[0]), Int32.Parse(liF), 0, 0);
                            _context.Update(d);
                            
                        }
            }
            _context.SaveChanges();
            return Json(list);
        }
    }
}
