namespace AsynchronousPlayground
{
    using System;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using AsynchronousInterfaces;

    public class SimpleRx
    {
        private readonly IOutputWriter output;

        private readonly EventWaitHandle wh = new AutoResetEvent(false);

        public SimpleRx(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            this.output.WriteLine("Simple Rx Sample");

            var observable = "Simple character test for Rx".ToObservable();
            observable.Subscribe(x => this.output.WriteLine("rx character : {0}", x));
            
            var query = from i in Enumerable.Range(0, 10) select this.Write(i);
            var observalbleQuery = query.ToObservable();

            observalbleQuery
                .SubscribeOn(NewThreadScheduler.Default)
                .ObserveOn(NewThreadScheduler.Default)
                .Subscribe(
                x => this.output.WriteLine("Observer {0}", x), 
                () =>
                    {
                        this.output.WriteLine("Done");
                        this.wh.Set();
                    });

            this.wh.WaitOne();
        }

        private int Write(int i)
        {
            this.output.WriteLine("Subject {0}", i);
            return i;
        }
    }
}