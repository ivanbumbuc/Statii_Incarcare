using System;
using System.Collections.Generic;

namespace Statii_Incarcare.Models.Db
{
    public partial class Prize
    {
        public Prize()
        {
            Rezervaris = new HashSet<Rezervari>();
        }

        public int PrizaId { get; set; }
        public int? StatieId { get; set; }
        public int? TipId { get; set; }

        public virtual Statii? Statie { get; set; }
        public virtual Tip? Tip { get; set; }
        public virtual ICollection<Rezervari> Rezervaris { get; set; }
    }
}
