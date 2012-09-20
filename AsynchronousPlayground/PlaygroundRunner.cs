namespace AsynchronousPlayground
{
    using System.Threading.Tasks;
    using AsynchronousInterfaces;
    using AsynchronousTools;

    public class PlaygroundRunner : IRunner
    {
        private readonly IOutputWriter output;

        private readonly string sampleLogFileName;

        private readonly ICountingDictionary countingDictionary;

        public PlaygroundRunner(IOutputWriter output, string sampleLogFileName, ICountingDictionary countingDictionary)
        {
            this.output = output;
            this.sampleLogFileName = sampleLogFileName;
            this.countingDictionary = countingDictionary;
        }

        public void Run()
        {
            var menu = new ConsoleMenu();

           menu.Add(
                "Simple Memory consuming example",
                () =>
                {
                    var sample = new SimpleSolution(this.output, this.sampleLogFileName, this.countingDictionary);
                    sample.Run();
                });
           menu.Add(
              "Simple Tasks",
              () =>
              {
                  var sample = new SimpleTasks(this.output);
                  sample.Run();
              });
           menu.Add(
                "Simple Async",
                () =>
                {
                    var sample = new SimpleAsync(this.output);
                    sample.Run();
                });
           menu.Add(
                 "Simple TPL",
                 () =>
                 {
                     var sample = new SimpleTPL(this.output);
                     sample.Run();
                 });
           menu.Add(
             "Simple Plinq",
             () =>
             {
                 var sample = new SimplePlinq(this.output);
                 sample.Run();
             });
            menu.Add(
               "Simple Reactive extensions example",
               () =>
               {
                   var sample = new SimpleRx(this.output);
                   sample.Run();
               });
            menu.Add(
             "Reactive extensions events",
             () =>
             {
                 var sample = new RxEvents(this.output);
                 sample.Run();
             });
            //menu.Add(
            //  "Calculate PI",
            //  () =>
            //  {
            //      var sample = new CalculatePi(this.output);
            //      sample.Run();
            //  });


            menu.Add("Back", () => { });
            menu.Show();
        }
    }
}