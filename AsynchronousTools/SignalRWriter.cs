namespace AsynchronousTools
{
    using System;
    using AsynchronousInterfaces;

    public class SignalRWriter : IOutputWriter
    {
        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void WriteLine()
        {
            Console.WriteLine(string.Empty);
        }
    }
}