namespace AsynchronousPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;

    public class SimpleAsync
    {
        private readonly IOutputWriter output;

        public SimpleAsync(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            this.output.WriteLine("Simple Async");

            var cts = new CancellationTokenSource();
            var tasks = new List<Task>();

            try
            {
                cts.CancelAfter(400);
                for (var i = 0; i < 20; i++)
                {
                    var locali = i;
                    tasks.Add(this.DoWork(locali, cts.Token));
                }

                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                this.output.WriteLine(" Operation cancelled ");
            }

            this.output.WriteLine("Simple Async Done...");
        }

        private async Task DoWork(int i, CancellationToken token)
        {
            await TaskEx.Delay(100);
            Thread.Sleep(300);
            token.ThrowIfCancellationRequested();
            
            var taskId = i;
            this.output.WriteLine("{0}", taskId);
        }
    }
}