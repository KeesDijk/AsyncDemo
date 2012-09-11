namespace AsynchronousTools
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AsynchronousInterfaces;

    public class LockingConsoleWriter : IOutputWriter
    {
        public void Write(string format, params object[] args)
        {
            var currentManagedThreadId = Thread.CurrentThread.ManagedThreadId;
            var currentTaskId = Task.CurrentId;
            format = AppendThreadingInfo(format, ref args, currentManagedThreadId, currentTaskId);

            lock (LockingClass.ConsoleLock)
            {
                SetConsoleColor(currentManagedThreadId);
                Console.Write(format, args);
                Console.ResetColor();
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            var currentManagedThreadId = Thread.CurrentThread.ManagedThreadId;
            var currentTaskId = Task.CurrentId;
            format = AppendThreadingInfo(format, ref args, currentManagedThreadId, currentTaskId);

            lock (LockingClass.ConsoleLock)
            {
                SetConsoleColor(currentManagedThreadId);
                Console.WriteLine(format, args);
                Console.ResetColor();
            }
        }

        public void WriteLine()
        {
            lock (LockingClass.ConsoleLock)
            {
                Console.WriteLine(string.Empty);
            }
        }

        private static string AppendThreadingInfo(
            string format, ref object[] args, int currentManagedThreadId, int? currentTaskId)
        {
            var baseString = string.Format(format, args);
            var stringToAppend = string.Format("( thread: {0,2} , task: {1,2} )", currentManagedThreadId, currentTaskId);
            var alignedString = string.Format("{0,-30}{1,25}", baseString, stringToAppend);

            var retValue = alignedString;

            return retValue;
        }

        private static void SetConsoleColor(int currentManagedThreadId)
        {
            // 16 colors but 0 is black exclude that.
            var colorValue = (currentManagedThreadId % 15) + 1;
            var newColor = (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), colorValue);
            Console.ForegroundColor = newColor;
        }
    }
}