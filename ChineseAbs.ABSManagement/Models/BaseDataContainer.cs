namespace ChineseAbs.ABSManagement.Models
{
    public abstract class BaseDataContainer<T>
    {
        public BaseDataContainer(T obj) 
        {
            FromTableObject(obj);
        }

        public BaseDataContainer() { }

        public abstract T GetTableObject();

        public abstract void FromTableObject(T obj);
    }
}
