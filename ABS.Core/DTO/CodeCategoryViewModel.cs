using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class CodeCategoryViewModel 
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public virtual ICollection<CodeItemViewModel> Items { get; set; }

    }

    public class CodeItemViewModel 
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public int Sequence { get; set; }

        public int CategoryID { get; set; }

        public int? ParentID { get; set; }
    }
}
