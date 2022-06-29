using System;
using System.Collections.Generic;

namespace Statii_Incarcare.Models.Db
{
    public partial class Statii
    {
        public Statii()
        {
            Prizes = new HashSet<Prize>();
        }

        public int StatieId { get; set; }
        public string? Nume { get; set; }
        public string? Oras { get; set; }
        public string? Adresa { get; set; }

        public virtual ICollection<Prize> Prizes { get; set; }
    }
}
