namespace AsynchronousPlayground
{
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using AsynchronousInterfaces;

    public class FindingMethodsThatSupportRxSchedulers
    {
        private readonly IOutputWriter output;

        public FindingMethodsThatSupportRxSchedulers(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            var withScheduler = (from m in typeof(Observable).GetMethods()
                                 from p in m.GetParameters()
                                 where p.ParameterType == typeof(IScheduler)
                                 orderby m.Name
                                 select m.Name).Distinct();

            foreach (var method in withScheduler)
            {
                this.output.WriteLine(method);
            }
        }
    }
}