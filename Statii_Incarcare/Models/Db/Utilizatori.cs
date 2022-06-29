using System;
using System.Collections.Generic;

namespace Statii_Incarcare.Models.Db
{
    public partial class Utilizatori
    {
        public Utilizatori()
        {
            Rezervaris = new HashSet<Rezervari>();
        }

        public Guid UtilizatorId { get; set; }
        public string? Nume { get; set; }
        public string Email { get; set; } = null!;
        public string Parola { get; set; } = null!;

        public virtual ICollection<Rezervari> Rezervaris { get; set; }
    }
}
