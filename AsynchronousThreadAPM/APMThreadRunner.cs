namespace AsynchronousThreadAPM
{
    using AsynchronousInterfaces;

    public class APMThreadRunner : IRunner
    {
        private readonly IOutputWriter output;

        public APMThreadRunner(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            for (var i = 0; i < 10; i++)
            {
                this.output.Write("APMThreadRunner: {0}", i);
            }
        }
    }
}