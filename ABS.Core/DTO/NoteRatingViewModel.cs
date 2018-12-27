using ABS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class NoteRatingViewModel
    {
        public int Id { get; set; }

        public int NoteID { get; set; }

        [StringLength(50)]
        public string Rating { get; set; }

        public int CompanyID { get; set; }
        public string CompanyName
        {
            get
            {
                if (RatingCompany != null)
                    return RatingCompany.Name;
                else
                    return "";
            }
        }

        public DateTime Asofdate { get; set; }

        public CompanyViewModel RatingCompany { get; set; }
    }
}
