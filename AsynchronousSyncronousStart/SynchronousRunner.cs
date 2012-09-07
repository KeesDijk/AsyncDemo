namespace AsynchronousSyncronousStart
{
    using System.IO;
    using System.Threading;
    using AsynchronousInterfaces;

    public class SynchronousRunner : IRunner
    {
        private readonly IOutputWriter output;
        private readonly IProgress progress;
        private readonly string sampleLogFileName; 

        public SynchronousRunner(IOutputWriter output, IProgress progress, string sampleLogFileName)
        {
            this.output = output;
            this.progress = progress;
            this.sampleLogFileName = sampleLogFileName;
        }

        public void Run()
        {
            this.output.WriteLine("Synchronous start");
            const int Max = 1000;

            if (File.Exists(this.sampleLogFileName))
            {
                this.output.WriteLine("File Exists");
            }
            else
            {
                this.output.WriteLine("File Missing !");
            }

            for (int i = 0; i < Max; i++)
            {
                Thread.Sleep(10);
                int percentageDone = (i * 100) / Max;
                this.progress.Progress(percentageDone, "{0} from {1}", i, Max);
            }
            
            this.output.WriteLine();
            this.output.WriteLine("Synchronous done");
        }
    }
}