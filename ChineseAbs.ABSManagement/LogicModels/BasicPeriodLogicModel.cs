namespace ChineseAbs.ABSManagement.LogicModels
{
    class BasicPeriodLogicModel<T> where T : class
    {
        public System.Lazy<T> Next { get; set; }
        public System.Lazy<T> Previous { get; set; }
    }
}
