namespace AsynchronousTools
{
    using System.Collections.Generic;
    using AsynchronousInterfaces;

    public class LockFreeCountingDictionary : ICountingDictionary
    {
        private readonly Dictionary<string, int> countingDictionary = new Dictionary<string, int>();

        private readonly object countingDictionaryLock = new object();

        private readonly IOutputWriter output;

        public LockFreeCountingDictionary(IOutputWriter output)
        {
            this.output = output;
        }

        public void AddOrIncrement(string keyName)
        {
            int currentCount;

            // currentCount will be zero if the key id doesn't exist..
            this.countingDictionary.TryGetValue(keyName, out currentCount);
            this.countingDictionary[keyName] = currentCount + 1;
        }

        public void ShowResults()
        {
            var total = 0;
            foreach (var keyValue in this.countingDictionary)
            {
                this.output.WriteLine("{0} has {1} entries", keyValue.Key, keyValue.Value);
                total += keyValue.Value;
            }

            this.output.WriteLine();
            this.output.WriteLine("Total : {0}", total);
        }
    }
}