namespace AsynchronousPlayground
{
    using System.Threading;
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

            var tb = new TransformBlock<int, int>(i => i * 2);
            var ab = new ActionBlock<int>(i => this.Compute(i), options);
            tb.LinkTo(ab);

            for (var i = 0; i < 10; i++)
            {
                tb.Post(i);
            }

            tb.Complete();
            tb.Completion.Wait();

            Thread.Sleep(500);
        }

        private void Compute(int i)
        {
            this.output.WriteLine("TplDataflow: {0}", i);
        }
    }
}