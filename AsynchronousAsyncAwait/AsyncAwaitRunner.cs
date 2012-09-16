namespace AsynchronousAsyncAwait
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;
    using AsynchronousTools;

    public class AsyncAwaitRunner : IRunner
    {
        private const string LogTypeGroupName = "LogType";

        private const RegexOptions RegexOption =
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled;

        private static readonly Regex LogTypeRegex = new Regex(
            "^.*]\\s*(?<" + LogTypeGroupName + ">.*)\\s*AirBender.*$", RegexOption);

        private readonly ICommander commander;

        private readonly ICountingDictionary countingDictionary;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly IOutputWriter output;

        private readonly IProgress progress;

        private readonly string sampleLogFileName;

        private long fileSizeInBytes;

        private long lineCount;

        private long lineSizeInBytesSoFar;

        public AsyncAwaitRunner(
            IOutputWriter output, 
            IProgress progress, 
            string sampleLogFileName, 
            ICountingDictionary countingDictionary, 
            ICommander commander)
        {
            this.output = output;
            this.progress = progress;
            this.sampleLogFileName = sampleLogFileName;
            this.countingDictionary = countingDictionary;
            this.commander = commander;
        }

        public void Run()
        {
            this.output.WriteLine("Synchronous start");
            this.CheckFileExists();

            var f = new FileInfo(this.sampleLogFileName);
            this.fileSizeInBytes = f.Length;
            this.lineSizeInBytesSoFar = 0;
            this.lineCount = 0;

            //Task.Factory.StartNew(this.WaitForCancellation);

            var sw = new Stopwatch();
            sw.Start();

            var readAndProcessLines = this.ReadAndProcessLines(this.cts.Token);
            bool result = false;
            try
            {
                result = readAndProcessLines.Result;
            }
            catch (Exception e)
            {
                this.output.WriteLine(string.Format("Error : {0}", e));
            }

            sw.Stop();
            if (!result)
            {
                this.output.WriteLine("Cancelled");
            }
            else
            {
                this.ShowResults();
            }

            this.output.WriteLine();
            this.output.WriteLine("Async Await done in {0}", sw.Elapsed);
        }

        private void CheckFileExists()
        {
            this.output.WriteLine(File.Exists(this.sampleLogFileName) ? "File Exists" : "File Missing !");
        }

        private void ProcessLine(string line)
        {
            var match = LogTypeRegex.Match(line);
            
            //var delay = TaskEx.Delay(10);
            //var continueWith = delay.ContinueWith(x => { throw new Exception("Paniek"); });

            if (match.Success)
            {
                var logTypeValue = match.Groups[LogTypeGroupName].Value;
                this.countingDictionary.AddOrIncrement(logTypeValue);
            }

            Interlocked.Add(ref this.lineSizeInBytesSoFar, Encoding.Default.GetByteCount(line + Environment.NewLine));
            Interlocked.Increment(ref this.lineCount);
            var percentageDone = (int)(((double)this.lineSizeInBytesSoFar / this.fileSizeInBytes) * 100.0);
            this.progress.Progress(
                percentageDone, 
                "{0} line, {1} bytes from {2} bytes", 
                this.lineCount, 
                this.lineSizeInBytesSoFar, 
                this.fileSizeInBytes);
        }

        private async Task<bool> ReadAndProcessLines(CancellationToken token)
        {
            bool cancelled = false;
            using (TextReader reader = File.OpenText(this.sampleLogFileName))
            {
                string line;
                while (!cancelled && (line = await reader.ReadLineAsync()) != null)
                {
                    //if (token.IsCancellationRequested)
                    //{
                    //    cancelled = true;
                    //}

                    var localLineCopy = line;
                    this.ProcessLine(localLineCopy);
                }
            }

            return !cancelled;
        }

        private void ShowResults()
        {
            this.countingDictionary.ShowResults();
        }

        private void WaitForCancellation()
        {
            var inputChar = this.commander.WaitforSingleChar();
            if (inputChar.Equals('q'))
            {
                this.cts.Cancel();
            }
        }
    }
}