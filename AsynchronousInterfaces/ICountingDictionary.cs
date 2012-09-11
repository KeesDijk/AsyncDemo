namespace AsynchronousInterfaces
{
    public interface ICountingDictionary
    {
        void AddOrIncrement(string keyName);

        void ShowResults();
    }
}