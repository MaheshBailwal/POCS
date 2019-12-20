namespace PerformanceTestLibrary
{
    public interface IDataStore
    {
        T Get<T>(string key, out double fetchTime);
        void Put<T>(string key, T instance);
    }

    public interface IQueryableDataStore
    {
        T Get<T>(string key, int X, int Y, out double fetchTime);
        void Put<T>(string key, T instance);
    }
}