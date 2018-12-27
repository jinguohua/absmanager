namespace ChineseAbs.ABSManagement.AssetEvent
{
    public abstract class AssetEventBase
    {
        public AssetEventBase(string userName)
        {
            m_userName = userName;
        }

        public abstract bool PrevExecute();

        public abstract bool PrevRevert();

        public abstract bool Execute();

        public abstract bool Revert();

        protected string m_userName;

        protected DBAdapter m_dbAdapter
        {
            get { return new DBAdapter(); }
        }
    }
}
