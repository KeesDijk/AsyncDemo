namespace AsynchronousPlayground
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;

    public class ClosuresInCreatingTasksInALoop
    {
        private readonly IOutputWriter output;

        public ClosuresInCreatingTasksInALoop(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(
                    Task.Factory.StartNew(
                        () =>
                            {
                                var taskId = i;
                                this.output.WriteLine(" id : {0}", taskId);
                            }));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}