using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.LogicModels.ActionHabits
{
    public class ActionHabitsLogicModel
    {
        public ActionHabitsLogicModel(string userName)
        {
            m_userName = userName;
            m_dbAdapter = new DBAdapter();
            Load();
        }

        public EditAssetCashflow EditAssetCashflow { get; set; }

        public void Save()
        {
            m_dbAdapter.UserActionHabits.DeleteByUserName(m_userName);

            if (EditAssetCashflow.EditPrincipal.AutoSyncPrincipalBalance)
            {
                var actionHabits = new UserActionHabits();
                actionHabits.UserName = m_userName;
                actionHabits.ActionCategoryName = "AssetCashflow_EditPrincipal";
                actionHabits.ActionName = "AutoSyncPrincipalBalance";
                actionHabits.ActionSetting = EditAssetCashflow.EditPrincipal.AutoSyncPrincipalBalance.ToString();
                m_dbAdapter.UserActionHabits.New(actionHabits);
            }

            if (EditAssetCashflow.EditPrincipalBalance.AutoSyncPrincipal)
            {
                var actionHabits = new UserActionHabits();
                actionHabits.UserName = m_userName;
                actionHabits.ActionCategoryName = "AssetCashflow_EditPrincipalBalance";
                actionHabits.ActionName = "AutoSyncPrincipal";
                actionHabits.ActionSetting = EditAssetCashflow.EditPrincipalBalance.AutoSyncPrincipal.ToString();
                m_dbAdapter.UserActionHabits.New(actionHabits);
            }
        }

        private void Load()
        {
            var actionHabits = m_dbAdapter.UserActionHabits.GetByUserName(m_userName);

            EditAssetCashflow = new EditAssetCashflow();

            foreach (var actionHabit in actionHabits)
            {
                if (actionHabit.ActionCategoryName == "AssetCashflow_EditPrincipal")
                {
                    if (actionHabit.ActionName == "AutoSyncPrincipalBalance")
                    {
                        var autoSyncPrincipalBalance = false;
                        bool.TryParse(actionHabit.ActionSetting, out autoSyncPrincipalBalance);
                        EditAssetCashflow.EditPrincipal.AutoSyncPrincipalBalance = autoSyncPrincipalBalance;
                    }
                }

                if (actionHabit.ActionCategoryName == "AssetCashflow_EditPrincipalBalance")
                {
                    if (actionHabit.ActionName == "AutoSyncPrincipal")
                    {
                        var autoSyncPrincipal = false;
                        bool.TryParse(actionHabit.ActionSetting, out autoSyncPrincipal);
                        EditAssetCashflow.EditPrincipalBalance.AutoSyncPrincipal = autoSyncPrincipal;
                    }
                }
            }
        }

        private string m_userName;
        private DBAdapter m_dbAdapter;
    }
}
