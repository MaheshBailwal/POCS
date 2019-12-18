namespace PerformanceTestLibrary
{
    public interface IDataStore
    {
        T Get<T>(string key, ref string res);
        void Put<T>(string key, T instance);
    }
}