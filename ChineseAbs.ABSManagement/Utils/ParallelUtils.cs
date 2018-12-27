using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils
{
    static public class ParallelUtils
    {
        static public IEnumerable<System.Threading.Tasks.Task> Start(params Action[] actionArray)
        {
            return actionArray.Select(System.Threading.Tasks.Task.Factory.StartNew);
        }

        static public void StartUntilFinish(params Action[] actionArray)
        {
            var tasks = Start(actionArray);
            tasks.ToList().ForEach(x => x.Wait());
        }
    }
}
