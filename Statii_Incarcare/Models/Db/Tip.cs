using System;
using System.Collections.Generic;

namespace Statii_Incarcare.Models.Db
{
    public partial class Tip
    {
        public Tip()
        {
            Prizes = new HashSet<Prize>();
        }

        public int TipId { get; set; }
        public string? Nume { get; set; }

        public virtual ICollection<Prize> Prizes { get; set; }
    }
}
