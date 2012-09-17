namespace AsynchronousConsole
{
    using System;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;
    using AsynchronousTools;

    internal class Program
    {
        private static DIConfigurationName GetMainMenuChoice()
        {
            var menu = new ConsoleMenu();

            // illigal menu options will result in illigal configuration selected
            var configChoice = DIConfigurationName.Illegal;
            menu.Add("Synchronous", () => configChoice = DIConfigurationName.Synchronous);
            menu.Add("Playground", () => configChoice = DIConfigurationName.Playground);
            menu.Add("ThreadsAPM", () => configChoice = DIConfigurationName.ThreadsAPM);
            menu.Add("ThreadsEAP", () => configChoice = DIConfigurationName.ThreadsEAP);
            menu.Add("Tasks", () => configChoice = DIConfigurationName.Tasks);
            menu.Add("Async/Await", () => configChoice = DIConfigurationName.AsyncAwait);
            menu.Add("Plinq", () => configChoice = DIConfigurationName.PLinq);
            menu.Add("Tpl DataFlow", () => configChoice = DIConfigurationName.TPLDataflow);
            menu.Add("Reactive extensions", () => configChoice = DIConfigurationName.Rx);
            menu.Add("SignalR (also start website)", () => configChoice = DIConfigurationName.SignalR);
            menu.Add("Quit", () => configChoice = DIConfigurationName.None);
            menu.Show();
            return configChoice;
        }

        private static void Main(string[] args)
        {
            var configChoice = DIConfigurationName.Illegal;
            do
            {
                try
                {
                    configChoice = RunAsync().Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            while (configChoice != DIConfigurationName.None);
        }

        private static async Task<DIConfigurationName> RunAsync()
        {
            var configChoice = GetMainMenuChoice();

            if (configChoice != DIConfigurationName.None && configChoice != DIConfigurationName.Illegal)
            {
                GlobalSetup.OverRideDIConfiguration(configChoice);
                var runner = GlobalSetup.Resolve<IRunner>();
                await Task.Factory.StartNew(runner.Run);
            }

            return configChoice;
        }
    }
}