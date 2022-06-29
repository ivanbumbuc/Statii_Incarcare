using System;
using System.Collections.Generic;

namespace Statii_Incarcare.Models.Db
{
    public partial class Rezervari
    {
        public Guid BookingId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? PrizaId { get; set; }
        public string? NrMasina { get; set; }
        public Guid? UtilizatorId { get; set; }

        public virtual Prize? Priza { get; set; }
        public virtual Utilizatori? Utilizator { get; set; }
    }
}
