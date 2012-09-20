namespace AsynchronousPlayground
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;

    public class SimplePlinq
    {
        private readonly IOutputWriter output;

        public SimplePlinq(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            this.output.WriteLine("Simple Plinq Sample");

            var range = Enumerable.Range(0, 10).ToArray();

            var cts = new CancellationTokenSource();

            try
            {
                var query = from i in range.AsParallel()
                                .WithDegreeOfParallelism(2)
                                .WithCancellation(cts.Token)
                            select this.Write(i);

                TaskEx.Delay(1000).ContinueWith((x) => cts.Cancel());

                var sum = query.Sum();
                this.output.WriteLine("Sum : {0}", sum);
            }
            catch (OperationCanceledException e)
            {
                this.output.WriteLine("The operation was cancelled");
            }
        }

        private int Write(int i)
        {
            Thread.Sleep(500);
            this.output.WriteLine("Working Method {0}", i);
            return i;
        }
    }
}