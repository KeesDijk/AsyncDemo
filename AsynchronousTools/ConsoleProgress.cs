namespace AsynchronousTools
{
    using System;
    using AsynchronousInterfaces;

    public class ConsoleProgress : IProgress
    {
        public void ProgressMessage(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

        public void Progress(int percentage)
        {
            Console.WriteLine("{0} % done.", percentage);
        }
    }
}