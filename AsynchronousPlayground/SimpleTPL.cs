namespace AsynchronousPlayground
{
    using System.Threading.Tasks.Dataflow;
    using AsynchronousInterfaces;

    public class SimpleTPL
    {
        private readonly IOutputWriter output;

        public SimpleTPL(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 8 };

            var ab = new ActionBlock<int>(i => this.Compute(i), options);

            for (var i = 0; i < 10; i++)
            {
                ab.Post(i);
            }

            ab.Complete();
            ab.Completion.Wait();
        }

        private void Compute(int i)
        {
            this.output.WriteLine("TplDataflow: {0}", i);
        }
    }
}