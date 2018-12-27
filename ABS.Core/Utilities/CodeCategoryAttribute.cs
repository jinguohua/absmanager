using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CodeCategoryAttribute : Attribute
    {
        public string Category { get; set; }

        public CodeCategoryAttribute(string category)
        {
            this.Category = category;
        }
    }
}
