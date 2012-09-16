namespace AsynchronousThreadAPM
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using AsynchronousInterfaces;

    public class APMThreadRunner : IRunner
    {
        private const string LogTypeGroupName = "LogType";

        private const RegexOptions RegexOption =
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled;

        private static readonly Regex LogTypeRegex = new Regex(
            "]\\s*(?<" + LogTypeGroupName + ">.*)\\s*AirBender", RegexOption);

        private static readonly EventWaitHandle WaitHandle = new AutoResetEvent(false);

        private readonly ICountingDictionary countingDictionary;

        private readonly IOutputWriter output;

        private readonly IProgress progress;

        private readonly string sampleLogFileName;

        private long fileSizeInBytes;

        private long lineCount;

        private long lineSizeInBytesSoFar;

        public APMThreadRunner(IOutputWriter output, IProgress progress, string sampleLogFileName, ICountingDictionary countingDictionary)
        {
            this.output = output;
            this.progress = progress;
            this.sampleLogFileName = sampleLogFileName;
            this.countingDictionary = countingDictionary;
        }

        public void Run()
        {
            this.output.WriteLine("Thread APM start");
            this.CheckFileExists();

            var f = new FileInfo(this.sampleLogFileName);
            this.fileSizeInBytes = f.Length;
            this.lineSizeInBytesSoFar = 0;
            this.lineCount = 0;

            const int Chunksize = 4096;
            var buffer = new byte[Chunksize];

            var sw = new Stopwatch();
            sw.Start();

            var fs = new FileStream(
                this.sampleLogFileName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                Chunksize,
                FileOptions.Asynchronous);

            fs.BeginRead(buffer, 0, buffer.Length, this.ReadAsyncCallback, new AsyncFileReadInfo(buffer, fs));

            this.output.WriteLine();
            this.output.WriteLine("All threads started, waiting to complete");
            WaitHandle.WaitOne();

            sw.Stop();
            this.ShowResults();

            this.output.WriteLine();
            this.output.WriteLine("Thread APM done in {0}", sw.Elapsed);
        }

        private void CheckFileExists()
        {
            this.output.WriteLine(File.Exists(this.sampleLogFileName) ? "File Exists" : "File Missing !");
        }

        // Incorrect misses matches that are at the breaking points of chunks
        private void ProcessLine(string line)
        {
            var matches = LogTypeRegex.Matches(line);

            foreach (Match match in matches)
            {
                var logTypeValue = match.Groups[LogTypeGroupName].Value;
                this.countingDictionary.AddOrIncrement(logTypeValue);
            }
        }

        private void ReadAsyncCallback(IAsyncResult ar)
        {
            var info = (AsyncFileReadInfo)ar.AsyncState;

            var amountRead = 0;
            try
            {
                amountRead = info.MyStream.EndRead(ar);
            }
            catch (IOException)
            {
                this.output.WriteLine("Unable to complete read.");
                info.MyStream.Close();
                return;
            }

            string line = Encoding.UTF8.GetString(info.ByteArray, 0, amountRead);

            this.ProcessLine(line);

            var byteCount = Encoding.Default.GetByteCount(line);
            Interlocked.Add(ref this.lineSizeInBytesSoFar, byteCount);
            Interlocked.Increment(ref this.lineCount);
            var percentageDone = (int)(((double)this.lineSizeInBytesSoFar / this.fileSizeInBytes) * 100.0);
            this.progress.Progress(
                percentageDone,
                "{0} chunks, {1} bytes from {2} bytes",
                this.lineCount,
                this.lineSizeInBytesSoFar,
                this.fileSizeInBytes);

            if (info.MyStream.Position < info.MyStream.Length)
            {
                try
                {
                    info.MyStream.BeginRead(info.ByteArray, 0, info.ByteArray.Length, this.ReadAsyncCallback, info);
                }
                catch (IOException)
                {
                    this.output.WriteLine("Unable to start next read.");

                    info.MyStream.Close();
                }
            }
            else
            {
                info.MyStream.Close();
                this.output.WriteLine("Done reading!");
                WaitHandle.Set();
            }
        }

        private void ShowResults()
        {
            this.countingDictionary.ShowResults();
        }
    }
}