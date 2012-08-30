namespace AsynchronousTools
{
    using System;
    using AsynchronousInterfaces;

    public class ConsoleWriter : IOutputWriter
    {
        public void Write(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}