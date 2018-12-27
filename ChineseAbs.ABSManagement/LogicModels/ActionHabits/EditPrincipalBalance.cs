using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.LogicModels.ActionHabits
{
    //编辑剩余本金
    public class EditPrincipalBalance
    {
        public EditPrincipalBalance()
        {
            AutoSyncPrincipal = false;
        }

        //自动同步本金
        public bool AutoSyncPrincipal { get; set; }
    }
}
