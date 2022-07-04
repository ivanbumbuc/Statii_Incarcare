using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Models
{
    public class ModelPriza
    {
        public int? StatieId { get; set; }
        public IList<Tip> tipuri { get; set; }
        public int PrizaId { get; set; }
    }
}
