namespace ChineseAbs.ABSManagement.LogicModels.ActionHabits
{
    //编辑本金
    public class EditPrincipal
    {
        public EditPrincipal()
        {
            AutoSyncPrincipalBalance = false;
        }

        //自动同步剩余本金
        public bool AutoSyncPrincipalBalance { get; set; }
    }
}
