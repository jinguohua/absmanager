using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Services
{
    public class TestServices : ServiceBase
    {
        public TestServices(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Test() {
            var op = SAFS.Core.Context.ApplicationContext.Current.Operator;
        }
    }
}
