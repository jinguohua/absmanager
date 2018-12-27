using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class NoteRating : EntityBase<int>
    {
        public int NoteID { get; set; }

        [ForeignKey("NoteID")]
        public virtual ProjectNote Note { get; set; }

        [StringLength(50)]
        public string Rating { get; set; }

        public int CompanyID { get; set; }

        [ForeignKey("CompanyID")]
        public virtual Company RatingCompany { get; set; }

        public DateTime Asofdate { get; set; }
    }
}
