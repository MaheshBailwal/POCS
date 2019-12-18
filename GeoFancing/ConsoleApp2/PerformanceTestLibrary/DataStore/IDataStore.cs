namespace PerformanceTestLibrary
{
    public interface IDataStore
    {
        T Get<T>(string key, ref string res, out long fetchTime);
        void Put<T>(string key, T instance);
    }
}