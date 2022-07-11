namespace Statii_Incarcare.Models
{
    public class RezervariSaptamana
    {
        public List<DisponibilitateSaptamana> luni = new List<DisponibilitateSaptamana>();
        public List<DisponibilitateSaptamana> marti = new List<DisponibilitateSaptamana>();
        public List<DisponibilitateSaptamana> miercuri = new List<DisponibilitateSaptamana>();
        public List<DisponibilitateSaptamana> joi = new List<DisponibilitateSaptamana>();

        public List<DisponibilitateSaptamana> vineri = new List<DisponibilitateSaptamana>();
        public List<DisponibilitateSaptamana> sambata = new List<DisponibilitateSaptamana>();
        public List<DisponibilitateSaptamana> duminica = new List<DisponibilitateSaptamana>();

        public List<String> dateZile;
    }
}
