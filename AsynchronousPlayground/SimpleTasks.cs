namespace AsynchronousPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;

    public class SimpleTasks
    {
        private readonly IOutputWriter output;

        public SimpleTasks(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            this.output.WriteLine("Simple Tasks");

            this.FirstMethod();

            this.SecondMethod();

            this.output.WriteLine("Simple Tasks Done...");
        }

        private void DoWork(int i)
        {
            Thread.Sleep(500);
            var taskId = i;
            this.output.WriteLine("{0}", taskId);
        }

        private void FirstMethod()
        {
            var cts = new CancellationTokenSource();
            var tasks = new List<Task>();

            try
            {
                for (var i = 0; i < 10; i++)
                {
                    var locali = i;
                    tasks.Add(Task.Factory.StartNew(() => this.DoWork(locali), cts.Token));
                }

                TaskEx.Delay(100).ContinueWith(x => cts.Cancel());

                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                this.output.WriteLine(" Operation cancelled ");
            }
        }

        private void SecondMethod()
        {
            var cts = new CancellationTokenSource();
            var parallelOptions = new ParallelOptions { CancellationToken = cts.Token, MaxDegreeOfParallelism = 2 };
            try
            {
                TaskEx.Delay(10).ContinueWith(x => cts.Cancel());

                Parallel.For(0, 10, parallelOptions, this.DoWork);
            }
            catch (OperationCanceledException e)
            {
                this.output.WriteLine("cancelled");
            }
        }
    }
}