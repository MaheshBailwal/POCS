using ConsoleApp2;

namespace RedisTest
{
    public interface IDataStore
    {
        T Get<T>(string key, ref string res);
        void Put<T>(string key, T instance);
    }

    public interface IDataStorebyPoint
    {
        T Get<T>(string key, int X, int Y, ref string res);
        void Put<T>(string key, T instance);
    }
}