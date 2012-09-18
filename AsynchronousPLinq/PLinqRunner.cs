namespace AsynchronousPLinq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using AsynchronousInterfaces;

    public class PLinqRunner : IRunner
    {
        private const string LogTypeGroupName = "LogType";

        private const RegexOptions RegexOption =
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled;

        private static readonly Regex LogTypeRegex = new Regex(
            "^.*]\\s*(?<" + LogTypeGroupName + ">.*)\\s*AirBender.*$", RegexOption);

        private readonly ICountingDictionary countingDictionary;

        private readonly IOutputWriter output;

        private readonly IProgress progress;

        private readonly string sampleLogFileName;

        private long fileSizeInBytes;

        private long lineCount;

        private long lineSizeInBytesSoFar;

        public PLinqRunner(
            IOutputWriter output, IProgress progress, string sampleLogFileName, ICountingDictionary countingDictionary)
        {
            this.output = output;
            this.progress = progress;
            this.sampleLogFileName = sampleLogFileName;
            this.countingDictionary = countingDictionary;
        }

        public void Run()
        {
            this.output.WriteLine("Plinq start");
            this.CheckFileExists();

            var f = new FileInfo(this.sampleLogFileName);
            this.fileSizeInBytes = f.Length;
            this.lineSizeInBytesSoFar = 0;
            this.lineCount = 0;

            var sw = new Stopwatch();
            sw.Start();

            var typedSequence = from line in ReadFrom(this.sampleLogFileName).AsParallel()  
                                let result = this.ProcessLine(line)
                                select result;

            var resultcount = typedSequence.Count();
            sw.Stop();
            this.ShowResults();

            this.output.WriteLine();
            this.output.WriteLine("PLinq done in {0}", sw.Elapsed);
        }

        private static IEnumerable<string> ReadFrom(string file)
        {
            using (var reader = File.OpenText(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private void CheckFileExists()
        {
            this.output.WriteLine(File.Exists(this.sampleLogFileName) ? "File Exists" : "File Missing !");
        }

        private bool ProcessLine(string line)
        {
            var match = LogTypeRegex.Match(line);

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
            return true;
        }

        private void ShowResults()
        {
            this.countingDictionary.ShowResults();
        }
    }
}