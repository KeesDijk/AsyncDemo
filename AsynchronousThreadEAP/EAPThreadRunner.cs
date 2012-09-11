namespace AsynchronousThreadEAP
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using AsynchronousInterfaces;

    public class EAPThreadRunner : IRunner
    {
        private const string LogTypeGroupName = "LogType";

        private const RegexOptions RegexOption =
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled;

        private static readonly Regex LogTypeRegex = new Regex(
            ".*]\\s*(?<" + LogTypeGroupName + ">.*)\\s*AirBender.*", RegexOption);

        private static readonly EventWaitHandle WaitHandle = new AutoResetEvent(false);

        private readonly ICountingDictionary countingDictionary;

        private readonly IOutputWriter output;

        private readonly IProgress progress;

        private readonly HybridDictionary runningTasksDictionary = new HybridDictionary();

        private readonly string sampleLogFileName;

        private long fileSizeInBytes;

        private int lineNr = 0;

        private long lineSizeInBytesSoFar;

        private SendOrPostCallback onProgressReportDelegate;

        public EAPThreadRunner(
            IOutputWriter output, IProgress progress, string sampleLogFileName, ICountingDictionary countingDictionary)
        {
            this.output = output;
            this.progress = progress;
            this.sampleLogFileName = sampleLogFileName;
            this.countingDictionary = countingDictionary;
            this.InitializeDelegates();
        }

        private delegate void WorkerEventHandler(string singleLine, AsyncOperation asyncOperation);

        public void Run()
        {
            this.output.WriteLine("Thread EAP start");
            this.CheckFileExists();

            var sw = new Stopwatch();
            sw.Start();

            var f = new FileInfo(this.sampleLogFileName);
            this.fileSizeInBytes = f.Length;
            this.lineSizeInBytesSoFar = 0;

            using (TextReader reader = File.OpenText(this.sampleLogFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var asyncOpId = Guid.NewGuid();
                    var asyncOp = AsyncOperationManager.CreateOperation(asyncOpId);

                    lock (this.runningTasksDictionary.SyncRoot)
                    {
                        this.runningTasksDictionary[asyncOpId] = asyncOp;
                    }

                    WorkerEventHandler workerDelegate = this.ProcessLine;

                    workerDelegate.BeginInvoke(line, asyncOp, this.ProcessingCompleted, asyncOp.UserSuppliedState);
                }
            }

            this.output.WriteLine();
            this.output.WriteLine("All threads started, waiting to complete");
            WaitHandle.WaitOne();
            sw.Stop();

            this.ShowResults();

            this.output.WriteLine();
            this.output.WriteLine("Thread EAP done in {0}", sw.Elapsed);
        }

        private void CheckFileExists()
        {
            this.output.WriteLine(File.Exists(this.sampleLogFileName) ? "File Exists" : "File Missing !");
        }

        private void InitializeDelegates()
        {
            this.onProgressReportDelegate = this.ReportProgress;
        }

        // Incorrect misses matches that are at the breaking points of chunks
        private void ProcessLine(string line, AsyncOperation asyncOperation)
        {
            var matches = LogTypeRegex.Matches(line);

            foreach (Match match in matches)
            {
                var logTypeValue = match.Groups[LogTypeGroupName].Value;
                this.countingDictionary.AddOrIncrement(logTypeValue);
            }

            Interlocked.Add(ref this.lineSizeInBytesSoFar, Encoding.Default.GetByteCount(line + Environment.NewLine));
            Interlocked.Increment(ref this.lineNr);

            var percentageDone = (int)(((double)this.lineSizeInBytesSoFar / this.fileSizeInBytes) * 100.0);
            ProgressChangedEventArgs progressChangedEventArgs = new ProcessFileProgressChangedEventArgs(
                this.lineNr, percentageDone, asyncOperation.UserSuppliedState);

            asyncOperation.Post(this.onProgressReportDelegate, progressChangedEventArgs);
        }

        private void ProcessingCompleted(object state)
        {
            var syncresult = state as IAsyncResult;
            var asyncOperationState = syncresult.AsyncState;
            bool allTasksDone;
            lock (this.runningTasksDictionary.SyncRoot)
            {
                this.runningTasksDictionary.Remove(asyncOperationState);
                allTasksDone = this.runningTasksDictionary.Count == 0;
            }

            if (allTasksDone)
            {
                WaitHandle.Set();
            }
        }

        private void ReportProgress(object state)
        {
            var args = state as ProcessFileProgressChangedEventArgs;
            if (args != null)
            {
                this.progress.Progress(
                    args.ProgressPercentage, 
                    "{0} Lines, {1} bytes from {2} bytes", 
                    args.LastReadLineNr, 
                    this.lineSizeInBytesSoFar, 
                    this.fileSizeInBytes);
            }
        }

        private void ShowResults()
        {
            this.countingDictionary.ShowResults();
        }
    }
}