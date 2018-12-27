using ChineseAbs.ABSManagement.LogicModels.ActionHabits;

namespace ChineseAbs.ABSManagement.Framework
{
    public class User
    {
        public User(string userName)
        {
            m_userName = userName;
        }

        public ActionHabitsLogicModel ActionHabits
        {
            get
            {
                if (m_actionHabits == null)
                {
                    m_actionHabits = new ActionHabitsLogicModel(m_userName);
                }

                return m_actionHabits;
            }
        }

        private ActionHabitsLogicModel m_actionHabits;

        private string m_userName;
    }
}
