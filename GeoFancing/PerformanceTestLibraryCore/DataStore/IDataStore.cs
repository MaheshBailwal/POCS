namespace PerformanceTestLibrary
{
    public interface IDataStore
    {

    }

    public interface INonQueryableDataStore: IDataStore
    {
        T Get<T>(string key, out double fetchTime);
        void Put<T>(string key, T instance);
    }

    public interface IQueryableDataStore: IDataStore
    {
        T Get<T>(string key, int X, int Y, out double fetchTime);
        void Put<T>(string key, T instance);
    }
}