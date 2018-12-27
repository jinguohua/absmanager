namespace ChineseAbs.ABSManagement.LogicModels.ActionHabits
{
    //编辑资产端现金流
    public class EditAssetCashflow
    {
        public EditAssetCashflow()
        {
            EditPrincipal = new EditPrincipal();
            EditPrincipalBalance = new EditPrincipalBalance();
        }

        public EditPrincipal EditPrincipal { get; set; }

        public EditPrincipalBalance EditPrincipalBalance { get; set; }
    }
}
