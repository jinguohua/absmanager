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
    public class CodeCategory : EntityBase<int>
    {
        public CodeCategory()
        {
            Items = new List<CodeItem>();
        }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int Sequence { get; set; }

        public virtual ICollection<CodeItem> Items { get; set; }
    }

    public class CodeItem : EntityBase<int>
    {
        [StringLength(50)]
        public string Key { get; set; }

        [StringLength(50)]
        public string Value { get; set; }

        public int Sequence { get; set; }

        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual CodeCategory Category { get; set; }

        public int? ParentID { get; set; }

        [ForeignKey("ParentID")]
        public virtual CodeItem Parent { get; set; }
    }
}
