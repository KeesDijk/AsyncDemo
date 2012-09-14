namespace AsynchronousTools
{
    using System;

    public class ConsoleCommander : ICommander
    {
        public char WaitforSingleChar()
        {
            var consoleKeyInfo = Console.ReadKey();
            return consoleKeyInfo.KeyChar;
        }
    }
}