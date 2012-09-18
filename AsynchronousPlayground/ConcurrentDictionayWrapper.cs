namespace AsynchronousPlayground
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using AsynchronousInterfaces;

    public class ConcurrentDictionayWrapper : ICountingDictionary
    {
        private readonly ConcurrentDictionary<string, int> countingDictionary = new ConcurrentDictionary<string, int>();

        private readonly IOutputWriter output;

        public ConcurrentDictionayWrapper(IOutputWriter output)
        {
            this.output = output;
        }

        public void AddOrIncrement(string keyName)
        {
            this.countingDictionary.AddOrUpdate(keyName, 1, (s, i) => i + 1);
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