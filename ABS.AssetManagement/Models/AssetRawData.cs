using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Models
{
    public class AssetRawData : EntityBase<long>
    {
        public int ConfigID { get; set; }

        [ForeignKey("ConfigID")]
        public virtual AssetRawDataConfig Config { get; set; }

        [StringLength(50)]
        public string BatchNumber { get; set; }

        [StringLength(100)]
        public string Col1 { get; set; }
        [StringLength(100)]
        public string Col2 { get; set; }
        [StringLength(100)]
        public string Col3 { get; set; }
        [StringLength(100)]
        public string Col4 { get; set; }
        [StringLength(100)]
        public string Col5 { get; set; }
        [StringLength(100)]
        public string Col6 { get; set; }
        [StringLength(100)]
        public string Col7 { get; set; }
        [StringLength(100)]
        public string Col8 { get; set; }
        [StringLength(100)]
        public string Col9 { get; set; }
        [StringLength(100)]
        public string Col10 { get; set; }
        [StringLength(100)]
        public string Col11 { get; set; }
        [StringLength(100)]
        public string Col12 { get; set; }
        [StringLength(100)]
        public string Col13 { get; set; }
        [StringLength(100)]
        public string Col14 { get; set; }
        [StringLength(100)]
        public string Col15 { get; set; }
        [StringLength(100)]
        public string Col16 { get; set; }
        [StringLength(100)]
        public string Col17 { get; set; }
        [StringLength(100)]
        public string Col18 { get; set; }
        [StringLength(100)]
        public string Col19 { get; set; }
        [StringLength(100)]
        public string Col20 { get; set; }
        [StringLength(100)]
        public string Col21 { get; set; }
        [StringLength(100)]
        public string Col22 { get; set; }
        [StringLength(100)]
        public string Col23 { get; set; }
        [StringLength(100)]
        public string Col24 { get; set; }
        [StringLength(100)]
        public string Col25 { get; set; }
        [StringLength(100)]
        public string Col26 { get; set; }
        [StringLength(100)]
        public string Col27 { get; set; }
        [StringLength(100)]
        public string Col28 { get; set; }
        [StringLength(100)]
        public string Col29 { get; set; }
        [StringLength(100)]
        public string Col30 { get; set; }
        [StringLength(100)]
        public string Col31 { get; set; }
        [StringLength(100)]
        public string Col32 { get; set; }
        [StringLength(100)]
        public string Col33 { get; set; }
        [StringLength(100)]
        public string Col34 { get; set; }
        [StringLength(100)]
        public string Col35 { get; set; }
        [StringLength(100)]
        public string Col36 { get; set; }
        [StringLength(100)]
        public string Col37 { get; set; }
        [StringLength(100)]
        public string Col38 { get; set; }
        [StringLength(100)]
        public string Col39 { get; set; }
        [StringLength(100)]
        public string Col40 { get; set; }
        [StringLength(100)]
        public string Col41 { get; set; }
        [StringLength(100)]
        public string Col42 { get; set; }
        [StringLength(100)]
        public string Col43 { get; set; }
        [StringLength(100)]
        public string Col44 { get; set; }
        [StringLength(100)]
        public string Col45 { get; set; }
        [StringLength(100)]
        public string Col46 { get; set; }
        [StringLength(100)]
        public string Col47 { get; set; }
        [StringLength(100)]
        public string Col48 { get; set; }
        [StringLength(100)]
        public string Col49 { get; set; }
        [StringLength(100)]
        public string Col50 { get; set; }
        [StringLength(100)]
        public string Col51 { get; set; }
        [StringLength(100)]
        public string Col52 { get; set; }
        [StringLength(100)]
        public string Col53 { get; set; }
        [StringLength(100)]
        public string Col54 { get; set; }
        [StringLength(100)]
        public string Col55 { get; set; }
        [StringLength(100)]
        public string Col56 { get; set; }
        [StringLength(100)]
        public string Col57 { get; set; }
        [StringLength(100)]
        public string Col58 { get; set; }
        [StringLength(100)]
        public string Col59 { get; set; }
        [StringLength(100)]
        public string Col60 { get; set; }
        [StringLength(100)]
        public string Col61 { get; set; }
        [StringLength(100)]
        public string Col62 { get; set; }
        [StringLength(100)]
        public string Col63 { get; set; }
        [StringLength(100)]
        public string Col64 { get; set; }
        [StringLength(100)]
        public string Col65 { get; set; }
        [StringLength(100)]
        public string Col66 { get; set; }
        [StringLength(100)]
        public string Col67 { get; set; }
        [StringLength(100)]
        public string Col68 { get; set; }
        [StringLength(100)]
        public string Col69 { get; set; }
        [StringLength(100)]
        public string Col70 { get; set; }
        [StringLength(100)]
        public string Col71 { get; set; }
        [StringLength(100)]
        public string Col72 { get; set; }
        [StringLength(100)]
        public string Col73 { get; set; }
        [StringLength(100)]
        public string Col74 { get; set; }
        [StringLength(100)]
        public string Col75 { get; set; }
        [StringLength(100)]
        public string Col76 { get; set; }
        [StringLength(100)]
        public string Col77 { get; set; }
        [StringLength(100)]
        public string Col78 { get; set; }
        [StringLength(100)]
        public string Col79 { get; set; }
        [StringLength(100)]
        public string Col80 { get; set; }
        [StringLength(100)]
        public string Col81 { get; set; }
        [StringLength(100)]
        public string Col82 { get; set; }
        [StringLength(100)]
        public string Col83 { get; set; }
        [StringLength(100)]
        public string Col84 { get; set; }
        [StringLength(100)]
        public string Col85 { get; set; }
        [StringLength(100)]
        public string Col86 { get; set; }
        [StringLength(100)]
        public string Col87 { get; set; }
        [StringLength(100)]
        public string Col88 { get; set; }
        [StringLength(100)]
        public string Col89 { get; set; }
        [StringLength(100)]
        public string Col90 { get; set; }
        [StringLength(100)]
        public string Col91 { get; set; }
        [StringLength(100)]
        public string Col92 { get; set; }
        [StringLength(100)]
        public string Col93 { get; set; }
        [StringLength(100)]
        public string Col94 { get; set; }
        [StringLength(100)]
        public string Col95 { get; set; }
        [StringLength(100)]
        public string Col96 { get; set; }
        [StringLength(100)]
        public string Col97 { get; set; }
        [StringLength(100)]
        public string Col98 { get; set; }
        [StringLength(100)]
        public string Col99 { get; set; }
        [StringLength(100)]
        public string Col100 { get; set; }
    }
}
