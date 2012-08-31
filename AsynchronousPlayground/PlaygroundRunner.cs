namespace AsynchronousPlayground
{
    using System.Threading.Tasks;
    using AsynchronousInterfaces;
    using AsynchronousTools;

    public class PlaygroundRunner : IRunner
    {
        private readonly IOutputWriter output;

        public PlaygroundRunner(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            var menu = new ConsoleMenu();

            menu.Add(
                "Creating Tasks in a loop", 
                () =>
                    {
                        var sample = new ClosuresInCreatingTasksInALoop(this.output);
                        sample.Run();
                    });
            menu.Add("Back", () => { });
            menu.Show();
        }
    }
}