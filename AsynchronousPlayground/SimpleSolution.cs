namespace AsynchronousPlayground
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using AsynchronousInterfaces;

    public class SimpleSolution : IRunner
    {
        private const string LogTypeGroupName = "LogType";

        private const RegexOptions RegexOption =
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled;

        private static readonly Regex LogTypeRegex = new Regex(
            "^.*]\\s*(?<" + LogTypeGroupName + ">.*)\\s*AirBender.*$", RegexOption);

        private readonly ICountingDictionary countingDictionary;

        private readonly IOutputWriter output;

        private readonly string sampleLogFileName;

        public SimpleSolution(
            IOutputWriter output, string sampleLogFileName, ICountingDictionary countingDictionary)
        {
            this.output = output;
            this.sampleLogFileName = sampleLogFileName;
            this.countingDictionary = countingDictionary;
        }

        public void Run()
        {
            this.output.WriteLine("Simple Solution start");
            this.CheckFileExists();

            var sw = new Stopwatch();
            sw.Start();

            var allLines = File.ReadAllLines(this.sampleLogFileName);
            var concatedString = string.Join(Environment.NewLine, allLines);
            this.ProcessLine(concatedString);
            
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
            var matches = LogTypeRegex.Matches(line);

            foreach (Match match in matches)
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