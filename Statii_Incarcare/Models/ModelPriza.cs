using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Models
{
    public class ModelPriza
    {
        public int? StatieId { get; set; }
        public List<SelectListItem> tipuri { get; set; }
        public int TipId { get; set; }
    }
}
