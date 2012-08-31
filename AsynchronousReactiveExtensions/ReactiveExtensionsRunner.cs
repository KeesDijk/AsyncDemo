namespace AsynchronousReactiveExtensions
{
    using System;
    using System.Reactive.Linq;
    using AsynchronousInterfaces;

    public class ReactiveExtensionsRunner : IRunner
    {
        private readonly IOutputWriter ouput;

        public ReactiveExtensionsRunner(IOutputWriter ouput)
        {
            this.ouput = ouput;
        }

        public void Run()
        {
            this.ouput.WriteLine("Rx runner");

            var observable = "Simple character test for Rx".ToObservable();
            observable.Subscribe(x => this.ouput.WriteLine("rx character : {0}", x));
        }
    }
}