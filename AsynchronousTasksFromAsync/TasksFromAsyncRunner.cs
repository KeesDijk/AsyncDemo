﻿namespace AsynchronousTasksFromAsync
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;

    public class TasksFromAsyncRunner : IRunner
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

        private long lineSizeInBytesSoFar;

        private long lineCount;

        public TasksFromAsyncRunner(
            IOutputWriter output, IProgress progress, string sampleLogFileName, ICountingDictionary countingDictionary)
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
            this.fileSizeInBytes = f.Length;
            this.lineSizeInBytesSoFar = 0;
            this.lineCount = 0;

            const int Chunksize = 4096;
            var buffer = new byte[Chunksize];

            var sw = new Stopwatch();
            sw.Start();
            var runningTasksList = new List<Task>();

            var fs = new FileStream(
               this.sampleLogFileName,
               FileMode.Open,
               FileAccess.Read,
               FileShare.Read,
               Chunksize,
               FileOptions.Asynchronous);

            Task<int> task = Task<int>.Factory.FromAsync(fs.BeginRead, fs.EndRead, buffer, 0, buffer.Length, this.ReadAsyncCallback, new AsyncFileReadInfo(buffer, fs));

            sw.Stop();
            this.ShowResults();

            this.output.WriteLine();
            this.output.WriteLine("Tasks done in {0}", sw.Elapsed);
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

        private void ShowResults()
        {
            this.countingDictionary.ShowResults();
        }
    }
}