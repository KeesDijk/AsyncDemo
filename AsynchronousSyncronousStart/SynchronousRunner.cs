namespace AsynchronousSyncronousStart
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using AsynchronousInterfaces;

    public class SynchronousRunner : IRunner
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

        public SynchronousRunner(IOutputWriter output, IProgress progress, string sampleLogFileName, ICountingDictionary countingDictionary)
        {
            this.output = output;
            this.progress = progress;
            this.sampleLogFileName = sampleLogFileName;
            this.countingDictionary = countingDictionary;
        }

        public void Run()
        {
            this.output.WriteLine("Synchronous start");
            this.CheckFileExists();

            var f = new FileInfo(this.sampleLogFileName);
            var fileSizeInBytes = f.Length;
            var lineSizeInBytesSoFar = 0;
            var lineCount = 0;

            var sw = new Stopwatch();
            sw.Start();

            using (TextReader reader = File.OpenText(this.sampleLogFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    this.ProcessLine(line);
                    lineSizeInBytesSoFar += Encoding.Default.GetByteCount(line + Environment.NewLine);
                    lineCount++;
                    var percentageDone = (int)(((double)lineSizeInBytesSoFar / fileSizeInBytes) * 100.0);
                    this.progress.Progress(
                        percentageDone, 
                        "{0} line, {1} bytes from {2} bytes", 
                        lineCount, 
                        lineSizeInBytesSoFar, 
                        fileSizeInBytes);
                }
            }

            sw.Stop();
            this.ShowResults();

            this.output.WriteLine();
            this.output.WriteLine("Synchronous done in {0}", sw.Elapsed);
        }

        private void CheckFileExists()
        {
            this.output.WriteLine(File.Exists(this.sampleLogFileName) ? "File Exists" : "File Missing !");
        }

        private void ProcessLine(string line)
        {
            var match = LogTypeRegex.Match(line);

            if (match.Success)
            {
                var logTypeValue = match.Groups[LogTypeGroupName].Value;
                this.countingDictionary.AddOrIncrement(logTypeValue);
            }
        }

        private void ShowResults()
        {
            this.countingDictionary.ShowResults();
        }
    }
}