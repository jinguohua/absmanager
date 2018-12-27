using ABS.Core.Models;
using ABS.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABS.Core.DTO
{
    public class DateRuleViewModel
    {
        public int Id { get; set; }

        public EDateRuleType Type { get; set; }

        public  AdjustDateRule AdjustDateRule { get; set; }

        public SpecificDateRuleViewModel SpecificDateRule { get; set; }

        public string Creator { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}