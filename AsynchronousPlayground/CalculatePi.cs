namespace AsynchronousPlayground
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;

    public class CalculatePi
    {
        private const int NumSteps = 100000000;

        private readonly IOutputWriter output;

        public CalculatePi(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            this.Time(SerialLinqPi);
            this.Time(ParallelLinqPi);
            this.Time(SerialPi);
            this.Time(ParallelPi);
            this.Time(ParallelPartitionerPi);

            this.output.WriteLine("----");
        }

        /// <summary>Estimates the value of PI using a PLINQ-based implementation.</summary>
        private static double ParallelLinqPi()
        {
            const double Step = 1.0 / (double)NumSteps;
            return
                (from i in ParallelEnumerable.Range(0, NumSteps) let x = (i + 0.5) * Step select 4.0 / (1.0 + x * x)).
                    Sum() * Step;
        }

        /// <summary>Estimates the value of PI using a Parallel.ForEach and a range partitioner.</summary>
        private static double ParallelPartitionerPi()
        {
            var sum = 0.0;
            const double Step = 1.0 / (double)NumSteps;
            var monitor = new object();
            Parallel.ForEach(
                Partitioner.Create(0, NumSteps), 
                () => 0.0, 
                (range, state, local) =>
                    {
                        for (int i = range.Item1; i < range.Item2; i++)
                        {
                            var x = (i + 0.5) * Step;
                            local += 4.0 / (1.0 + x * x);
                        }

                        return local;
                    },
                local => { lock (monitor) sum += local; });
            return Step * sum;
        }

        /// <summary>Estimates the value of PI using a Parallel.For.</summary>
        private static double ParallelPi()
        {
            var sum = 0.0;
            const double Step = 1.0 / (double)NumSteps;
            var monitor = new object();
            Parallel.For(
                0, 
                NumSteps, 
                () => 0.0, 
                (i, state, local) =>
                    {
                        var x = (i + 0.5) * Step;
                        return local + 4.0 / (1.0 + x * x);
                    }, 
                local => { lock (monitor) sum += local; });
            return Step * sum;
        }

        /// <summary>Estimates the value of PI using a LINQ-based implementation.</summary>
        private static double SerialLinqPi()
        {
            const double Step = 1.0 / (double)NumSteps;
            return (from i in Enumerable.Range(0, NumSteps) let x = (i + 0.5) * Step select 4.0 / (1.0 + x * x)).Sum()
                   * Step;
        }

        /// <summary>Estimates the value of PI using a for loop.</summary>
        private static double SerialPi()
        {
            var sum = 0.0;
            const double Step = 1.0 / (double)NumSteps;
            for (int i = 0; i < NumSteps; i++)
            {
                var x = (i + 0.5) * Step;
                sum = sum + 4.0 / (1.0 + x * x);
            }

            return Step * sum;
        }

        /// <summary>
        /// Times the execution of a function and outputs both the elapsed time and the function's result.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="work">
        /// </param>
        private void Time<T>(Func<T> work)
        {
            var sw = Stopwatch.StartNew();
            var result = work();
            this.output.WriteLine(sw.Elapsed + ": " + result);
        }
    }
}