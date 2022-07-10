using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Models
{
    public class DisponibilitateSaptamana
    {
        public String oraInceput { get; set; }
        public String oraSfarsit { get; set; }
        public Rezervari rezerrvare { get; set; }
    }
}
