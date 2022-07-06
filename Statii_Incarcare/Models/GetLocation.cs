using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Models
{
    public static class GetLocation
    {
        public static string Locatie(Statii st)
        {
            string link = "https://www.google.com/maps/search/?api=1&query=";
            var adresa = st.Adresa.Split(" ");
            string x = "";
            for(int i=0;i<adresa.Length-1;i++)
            {
                x = x + adresa[i] + "+";
            }
            x = x + adresa[adresa.Length-1];
            return link+"+"+st.Oras+"+"+x;
        }
    }
}
